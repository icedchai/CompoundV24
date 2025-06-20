﻿namespace CompoundV24.API.Features.Powers.Superpowers
{
    using System.Collections.Generic;
    using System.Linq;
    using AdminToys;
    using ColdWaterLibrary.Audio.Features.Helpers;
    using CompoundV24.API.Features.Powers.Interfaces;
    using Exiled.API.Features;
    using Exiled.API.Features.Toys;
    using MEC;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.FirstPersonControl.Thirdperson;
    using PlayerStatsSystem;
    using UnityEngine;
    using Light = Exiled.API.Features.Toys.Light;
    using Speaker = Speaker;

    /// <summary>
    /// The laser vision superpower.
    /// </summary>
    public class LaserVision : ToggleablePower
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "laser_vision";

        /// <inheritdoc/>
        public override bool IsCompoundV { get; set; } = true;

        /// <summary>
        /// Gets or sets the color of the laser.
        /// </summary>
        // public Color LaserColor { get; set; } = new Color(10, 0, 0);

        /// <summary>
        /// Gets or sets the amount of damage to deal per tick.
        /// </summary>
        public float DamagePerTick { get; set; } = 6f;

        private Dictionary<Player, RaycastHit> PlayersToRaycasts { get; set; } = new Dictionary<Player, RaycastHit>();

        private static Color GetColor(Player player)
        {
            string nick = (player.CustomName ?? player.Nickname).ToLower();
            if (nick.Contains("homelander"))
            {
                return new Color(100f, 0f, 0f);
            }

            if (nick.Contains("butcher") || nick.Contains("billy") || nick.Contains("william"))
            {
                return new Color(100f, 70f, 0f);
            }

            System.Random random = new System.Random(player.Id * player.RoleManager.CurrentRole.UniqueLifeIdentifier);

            return new Color((float)random.NextDouble() * 100, (float)random.NextDouble() * 100f, (float)random.NextDouble() * 100f);
        }

        private IEnumerator<float> LaserRender(Transform head, Player player, bool left)
        {
            yield return Timing.WaitForOneFrame;
            Color LaserColor = GetColor(player);
            RaycastHit hit;
            if (!PlayersToRaycasts.TryGetValue(player, out hit))
            {
                yield break;
            }

            var laser = Primitive.Create(PrimitiveType.Cube, Vector3.zero, Vector3.zero, Vector3.zero, false);
            laser.Color = new Color(LaserColor.r, LaserColor.g, LaserColor.b, 0.9f);
            laser.Flags = PrimitiveFlags.Visible;
            laser.MovementSmoothing = 60;

            laser.Transform.localScale = new Vector3(0.025f * player.Scale.x, 0.025f * player.Scale.y, hit.distance + 0.3f * player.Scale.z);
            TrackToEye(head, laser.Transform, left, player.Scale);
            laser.Transform.LookAt(hit.point);
            laser.Position += laser.Transform.forward * (hit.distance / 2);

            laser.Spawn();

            // player.Connection.Send(new ObjectDestroyMessage { netId = laser.AdminToyBase.netId });
            while (PlayerHasPowerEnabled(player) && !Round.IsLobby)
            {
                if (!PlayersToRaycasts.TryGetValue(player, out hit))
                {
                    laser.Destroy();
                    yield break;
                }

                laser.Transform.localScale = new Vector3(0.025f * player.Scale.x, 0.025f * player.Scale.y, hit.distance + 0.3f * player.Scale.z);
                TrackToEye(head, laser.Transform, left, player.Scale);
                laser.Transform.LookAt(hit.point);
                laser.Position += laser.Transform.forward * (hit.distance / 2);
                yield return Timing.WaitForOneFrame;
            }

            laser.Destroy();
            yield break;
        }

        private IEnumerator<float> LaserSound(Player player)
        {
            SoundHelper.PlaySound(player.Position, "laser_start", out _, out Speaker speaker1, false, true, 15, 30);
            speaker1.transform.parent = player.Transform;
            yield return Timing.WaitForSeconds(0.3f);
            SoundHelper.PlaySound(player.Position, "laser", out AudioPlayer burnPlayer, out Speaker speaker, true, minDistance: 15, maxDistance: 30);
            speaker.transform.parent = player.Transform;

            while (PlayerHasPowerEnabled(player) && !Round.IsLobby)
            {
                yield return Timing.WaitForOneFrame;
            }

            SoundHelper.PlaySound(player.Position, "laser_end", out _, out speaker1, false, true, 15, 30);
            speaker1.transform.parent = player.Transform;
            yield return Timing.WaitForSeconds(0.2f);
            burnPlayer.Destroy();
        }

        private IEnumerator<float> LaserLogic(Player player)
        {
            // player.EnableEffect(Exiled.API.Enums.EffectType.Flashed);

            while (PlayerHasPowerEnabled(player) && !Round.IsLobby)
            {
                Physics.Raycast(player.CameraTransform.position, player.CameraTransform.forward, out RaycastHit hit, Mathf.Infinity, ~(1 << 8 | 1 << 13 | 1 << 9), QueryTriggerInteraction.Ignore);
                if (!PlayersToRaycasts.TryGetValue(player, out RaycastHit _))
                {
                    PlayersToRaycasts.Add(player, hit);
                }

                PlayersToRaycasts[player] = hit;
                if (Player.TryGet(hit.collider, out Player victim))
                {
                    if (victim is not null && victim != player && !victim.IsGodModeEnabled)
                    {
                        player.ShowHitMarker();

                        var dh = new CustomReasonDamageHandler("Deep, concentrated burns in the flesh suggest that subject was struck by high heat projectile.", DamagePerTick);

                        victim.Hurt(dh);
                    }
                }

                yield return Timing.WaitForOneFrame;
            }

            // player.DisableEffect(Exiled.API.Enums.EffectType.Flashed);
        }

        private IEnumerator<float> LaserEyeGlow(Transform head, Player player, bool left)
        {
            yield break;
            Color LaserColor = GetColor(player);
            Light eyeGlow = Light.Create(position: Vector3.zero, rotation: Vector3.zero, spawn: false);
            eyeGlow.Color = LaserColor;
            eyeGlow.Intensity = 1;
            eyeGlow.Range = 0.02f;
            eyeGlow.ShadowType = LightShadows.None;
            eyeGlow.MovementSmoothing = 60;
            eyeGlow.Spawn();

            while (PlayerHasPowerEnabled(player) && !Round.IsLobby)
            {
                TrackToEye(head, eyeGlow.Transform, left, player.Scale);
                yield return Timing.WaitForOneFrame;
            }

            int i = 0;
            while (!PlayerHasPowerEnabled(player) && i < 600)
            {
                TrackToEye(head, eyeGlow.Transform, left, player.Scale);
                i++;
                yield return Timing.WaitForOneFrame;
            }

            while (eyeGlow.Intensity > 0)
            {
                TrackToEye(head, eyeGlow.Transform, left, player.Scale);
                eyeGlow.Intensity -= 0.1f;
                yield return Timing.WaitForOneFrame;
            }

            eyeGlow.Destroy();
        }

        private void TrackToEye(Transform head, Transform tracker, bool left, Vector3 playerScale)
        {
            tracker.position = head.position + Vector3.up * 0.1f * playerScale.y + head.forward * 0.1f * playerScale.z + head.right * 0.04f * playerScale.x * (left ? -1 : 1);
        }

        /// <inheritdoc/>
        protected override void RemoveProperties(Player player)
        {
            base.RemoveProperties(player);
            PlayersToRaycasts.Remove(player);
        }

        /// <inheritdoc/>
        protected override void DisposeVariablesOnRestart()
        {
            base.DisposeVariablesOnRestart();
            PlayersToRaycasts.Clear();
        }

        /// <inheritdoc/>
        protected override void EnablePower(Player player)
        {
            if (player.Role.Base is not IFpcRole fpcrole)
            {
                return;
            }

            base.EnablePower(player);

            // Physics.Raycast(player.CameraTransform.position, player.CameraTransform.forward, out RaycastHit hit, Mathf.Infinity, ~(1 << 8 | 1 << 13 | 1 << 9), QueryTriggerInteraction.Ignore);
            CharacterModel characterModel = fpcrole.FpcModule.CharacterModelInstance;
            List<HitboxIdentity> matchedHeadHitbox = characterModel.Hitboxes.Where(hbox => hbox.name.ToLower().Contains("head")).ToList();
            Transform head = matchedHeadHitbox.FirstOrDefault().transform;

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
