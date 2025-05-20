namespace CompoundV24.Powers.Superpowers
{
    using System.Collections.Generic;
    using System.Linq;
    using AdminToys;
    using Exiled.API.Features;
    using Exiled.API.Features.Toys;
    using MEC;
    using Mirror;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.FirstPersonControl.Thirdperson;
    using UnityEngine;
    using Speaker = Speaker;

    /// <summary>
    /// The laser vision superpower.
    /// </summary>
    public class LaserVision : Superpower
    {
        /// <inheritdoc/>
        public override string Name => "laser_vision";

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
            laser.Color = Color.red;
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

                laser.Transform.localScale = new Vector3(0.025f, 0.025f, hit.distance);
                laser.Position = head.position + (Vector3.up * 0.1f) + (head.forward * 0.1f) + (head.right * (left ? -0.04f : 0.04f));
                laser.Flags = PrimitiveFlags.Visible;
                laser.Transform.LookAt(hit.point);
                laser.Position += laser.Transform.forward * (hit.distance / 2);
                yield return Timing.WaitForOneFrame;
            }

            laser.Destroy();
            yield break;
        }

        private IEnumerator<float> LaserLogic(Player player)
        {
            SoundHelper.PlaySound(player.Position, "burn", out AudioPlayer burnPlayer, out Speaker speaker, true);

            player.EnableEffect(Exiled.API.Enums.EffectType.Flashed);
            player.EnableEffect(Exiled.API.Enums.EffectType.SeveredEyes);
            Timing.CallDelayed(1f, () => player.DisableEffect(Exiled.API.Enums.EffectType.SeveredEyes));

            speaker.transform.parent = player.Transform;
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
                    if (victim.Health <= 6)
                    {
                        victim.Kill("Deep, concentrated burns on the body suggest death by superpowered lasers.");
                    }

                    victim.Hurt(6);
                }

                yield return Timing.WaitForOneFrame;
            }

            player.DisableEffect(Exiled.API.Enums.EffectType.Flashed);
            burnPlayer.Destroy();
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

            Physics.Raycast(player.CameraTransform.position, player.CameraTransform.forward, out RaycastHit hit, Mathf.Infinity, ~(1 << 8 | 1 << 13 | 1 << 9), QueryTriggerInteraction.Ignore);

            CharacterModel characterModel = fpcrole.FpcModule.CharacterModelInstance;
            List<HitboxIdentity> matchedHeadHitbox = characterModel.Hitboxes.Where(hbox => hbox.name.ToLower().Contains("head")).ToList();
            Transform head = matchedHeadHitbox.FirstOrDefault().transform;

            LaserPlayers.Add(player);

            Timing.RunCoroutine(LaserLogic(player));
            Timing.RunCoroutine(LaserRender(head, player, false));
            Timing.RunCoroutine(LaserRender(head, player, true));
        }
    }
}
