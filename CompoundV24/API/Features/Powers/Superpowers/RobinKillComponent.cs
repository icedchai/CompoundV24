namespace CompoundV24.API.Features.Powers.Superpowers
{
    using ColdWaterLibrary.Audio.Features.Helpers;
    using Exiled.API.Features;
    using MEC;
    using PlayerStatsSystem;
    using UnityEngine;

    /// <summary>
    /// The script used to kill people by running into them.
    /// </summary>
    public class RobinKillComponent : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the <see cref="ControllableSuperspeed"/> instance to use.
        /// </summary>
        public ControllableSuperspeed SuperspeedInstance { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Player"/> who owns this kill component.
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// Initializes a trigger.
        /// </summary>
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
