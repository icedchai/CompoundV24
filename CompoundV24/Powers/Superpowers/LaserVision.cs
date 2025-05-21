namespace CompoundV24.Powers.Superpowers
{
    using System.Collections.Generic;
    using System.Linq;
    using AdminToys;
    using Exiled.API.Features;
    using Exiled.API.Features.DamageHandlers;
    using Exiled.API.Features.Toys;
    using Exiled.Events.Patches.Events.Player;
    using MEC;
    using Mirror;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.FirstPersonControl.Thirdperson;
    using PlayerStatsSystem;
    using UnityEngine;
    using Light = Exiled.API.Features.Toys.Light;
    using Speaker = Speaker;

    /// <summary>
    /// The laser vision superpower.
    /// </summary>
    public class LaserVision : Superpower
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "laser_vision";

        /// <inheritdoc/>
        public override bool IsCompoundV { get; set; } = true;

        public Color LaserColor { get; set; } = new Color(10, 0, 0);

        public float DamagePerTick { get; set; } = 6f;

        private List<Player> LaserPlayers { get; set; } = new List<Player>();

        private Dictionary<Player, RaycastHit> PlayersToRaycasts { get; set; } = new Dictionary<Player, RaycastHit>();

        private IEnumerator<float> LaserRender(Transform head, Player player, bool left)
        {
            yield return Timing.WaitForOneFrame;

            RaycastHit hit;
            if (!PlayersToRaycasts.TryGetValue(player, out hit))
            {
                yield break;
            }

            var laser = Primitive.Create(PrimitiveType.Cube, Vector3.zero, Vector3.zero, Vector3.zero, false);
            laser.Color = new Color(LaserColor.r, LaserColor.g, LaserColor.b, 0.9f);
            laser.MovementSmoothing = 60;
            laser.Spawn();

            player.Connection.Send(new ObjectDestroyMessage { netId = laser.AdminToyBase.netId });
            while (LaserPlayers.Contains(player) && !Round.IsLobby)
            {
                if (!PlayersToRaycasts.TryGetValue(player, out hit))
                {
                    laser.Destroy();
                    yield break;
                }

                laser.Transform.localScale = new Vector3(0.025f, 0.025f, hit.distance + 0.3f);
                TrackToEye(head, laser.Transform, left);
                laser.Flags = PrimitiveFlags.Visible;
                laser.Transform.LookAt(hit.point);
                laser.Position += laser.Transform.forward * (hit.distance / 2);
                yield return Timing.WaitForOneFrame;
            }

            laser.Destroy();
            yield break;
        }

        private IEnumerator<float> LaserSound(Player player)
        {
            SoundHelper.PlaySound(player.Position, "laser_start");

            SoundHelper.PlaySound(player.Position, "laser", out AudioPlayer burnPlayer, out Speaker speaker, true);
            speaker.transform.parent = player.Transform;
            speaker.Volume = 2;

            while (LaserPlayers.Contains(player) && !Round.IsLobby)
            {
                yield return Timing.WaitForOneFrame;
            }

            SoundHelper.PlaySound(player.Position, "laser_end");
            burnPlayer.Destroy();
        }

        private IEnumerator<float> LaserLogic(Player player)
        {

            player.EnableEffect(Exiled.API.Enums.EffectType.Flashed);

            while (LaserPlayers.Contains(player) && !Round.IsLobby)
            {
                Physics.Raycast(player.CameraTransform.position, player.CameraTransform.forward, out RaycastHit hit, Mathf.Infinity, ~(1 << 8 | 1 << 13 | 1 << 9), QueryTriggerInteraction.Ignore);
                if (!PlayersToRaycasts.TryGetValue(player, out RaycastHit _))
                {
                    PlayersToRaycasts.Add(player, hit);
                }

                PlayersToRaycasts[player] = hit;
                if (Player.TryGet(hit.collider, out Player victim))
                {
                    if (victim is null || victim == player || victim.IsGodModeEnabled)
                    {
                        yield return Timing.WaitForOneFrame;
                    }

                    player.ShowHitMarker();

                    var dh = new CustomReasonDamageHandler("Deep, concentrated burns in the flesh suggest that subject was struck by high heat projectile.", DamagePerTick);
                    victim.Hurt(dh);
                }

                yield return Timing.WaitForOneFrame;
            }

            player.DisableEffect(Exiled.API.Enums.EffectType.Flashed);
        }

        private IEnumerator<float> LaserEyeGlow(Transform head, Player player, bool left)
        {
            Light red = Light.Create(position: Vector3.zero, rotation: Vector3.zero, spawn: false);
            red.Color = new Color(1, 0, 0);
            red.Intensity = 1;
            red.Range = 0.1f;
            red.ShadowType = LightShadows.None;
            red.MovementSmoothing = 60;
            red.Spawn();

            while (LaserPlayers.Contains(player) && !Round.IsLobby)
            {
                TrackToEye(head, red.Transform, left);
                yield return Timing.WaitForOneFrame;
            }

            int i = 0;
            while (!LaserPlayers.Contains(player) && i < 600)
            {
                TrackToEye(head, red.Transform, left);
                i++;
                yield return Timing.WaitForOneFrame;
            }

            while (red.Intensity > 0)
            {
                TrackToEye(head, red.Transform, left);
                red.Intensity -= 0.1f;
                yield return Timing.WaitForOneFrame;
            }

            red.Destroy();
        }

        private void TrackToEye(Transform head, Transform tracker, bool left)
        {
            tracker.position = head.position + (Vector3.up * 0.1f) + (head.forward * 0.1f) + (head.right * (left ? -0.04f : 0.04f));
        }

        /// <inheritdoc/>
        protected override void RemoveProperties(Player player)
        {
            base.RemoveProperties(player);
            LaserPlayers.Remove(player);
            PlayersToRaycasts.Remove(player);
        }

        /// <inheritdoc/>
        protected override void DisposeVariablesOnRestart()
        {
            base.DisposeVariablesOnRestart();
            LaserPlayers.Clear();
            PlayersToRaycasts.Clear();
        }

        /// <inheritdoc/>
        public override void OnUsedAbility(Player player)
        {
            if (player.Role.Base is not IFpcRole fpcrole)
            {
                return;
            }

            if (LaserPlayers.Contains(player))
            {
                LaserPlayers.Remove(player);
                return;
            }

            // Physics.Raycast(player.CameraTransform.position, player.CameraTransform.forward, out RaycastHit hit, Mathf.Infinity, ~(1 << 8 | 1 << 13 | 1 << 9), QueryTriggerInteraction.Ignore);
            CharacterModel characterModel = fpcrole.FpcModule.CharacterModelInstance;
            List<HitboxIdentity> matchedHeadHitbox = characterModel.Hitboxes.Where(hbox => hbox.name.ToLower().Contains("head")).ToList();
            Transform head = matchedHeadHitbox.FirstOrDefault().transform;

            LaserPlayers.Add(player);

            Timing.RunCoroutine(LaserSound(player));
            Timing.RunCoroutine(LaserEyeGlow(head, player, false));
            Timing.RunCoroutine(LaserEyeGlow(head, player, true));
            Timing.CallDelayed(0.8f, () =>
            {
                Timing.RunCoroutine(LaserLogic(player));
                Timing.RunCoroutine(LaserRender(head, player, false));
                Timing.RunCoroutine(LaserRender(head, player, true));
            });
        }
    }
}
