namespace CompoundV24.Powers.Superpowers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;
    using MEC;
    using PlayerRoles.PlayableScps.Scp096;
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
            Log.Debug("initialized robinkillcomponent");
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
            if (victim is null || victim == Player || victim.IsGodModeEnabled || !SuperspeedInstance.PlayerHasSpeedEnabled(Player) || Player.Velocity == Vector3.zero)
            {
                return;
            }

            Player.ShowHitMarker();
            Timing.CallDelayed(0.1f, () => victim.Hurt(150, Exiled.API.Enums.DamageType.Crushed));
        }
    }
}
