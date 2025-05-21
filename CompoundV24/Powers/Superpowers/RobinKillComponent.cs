namespace CompoundV24.Powers.Superpowers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using MEC;
    using PlayerStatsSystem;
    using UnityEngine;

    public class RobinKillComponent : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the <see cref="ControllableSuperspeed"/> instance to use.
        /// </summary>
        public ControllableSuperspeed SuperspeedInstance { get; set; }

        public Player Player { get; set; }

        internal void Init()
        {
            BoxCollider trigger = gameObject.GetComponent<BoxCollider>();
            trigger.isTrigger = true;
        }

        private void OnDestroyed()
        {
            if (gameObject.TryGetComponent(out BoxCollider trigger))
            {
                Destroy(trigger);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Player victim = Player.Get(other);
            if (victim is null || victim == Player || victim.IsGodModeEnabled || !SuperspeedInstance.PlayerHasPowerEnabled(Player) || Player.Velocity.magnitude < 10)
            {
                return;
            }

            Player.ShowHitMarker();
            SoundHelper.PlaySound(victim.Position, "gore");
            Timing.CallDelayed(0.01f, () =>
            {
                StandardDamageHandler h;

                h = new JailbirdDamageHandler(Player.ReferenceHub, 150, Player.Velocity);
                /*
                h = new CustomReasonDamageHandler("Liquification suggests that subject was struck by high speed object.", 150);
                typeof(StandardDamageHandler).GetField("StartVelocity", BindingFlags.NonPublic | BindingFlags.Instance)
                    .SetValue(h, Player.Velocity * 20);*/
                victim.Hurt(h);

                // victim.ReferenceHub.playerStats.DealDamage(h);
            });
        }
    }
}
