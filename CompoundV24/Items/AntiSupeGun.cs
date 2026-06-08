using CompoundV24.API.Features.Powers;
using CustomItemsAPI.Items;
using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompoundV24.Items;

/// <summary>
/// A tranquilizer designed for keeping supes down.
/// </summary>
public class AntiSupeGun : CustomFirearmBase
{
    /// <inheritdoc/>
    public override string CustomItemName => "SupeTranquilizer";

    /// <inheritdoc/>
    public override string Description => "Tranquilizer that latches onto the V in bloodstream.";

    /// <inheritdoc/>
    public override ItemType Type => ItemType.GunRevolver;

    /// <inheritdoc/>
    public override float Damage => 1f;

    /// <inheritdoc/>
    public override void OnHurt(Player player, Player attacker, FirearmDamageHandler firearmDamage)
    {
        base.OnHurt(player, attacker, firearmDamage);

        if (PowerManager.Instance.CompoundVPowers.Any(p => p.Check(player)))
        {
            player.EnableEffect<Ensnared>(1, 5f, false);
            player.EnableEffect<Blindness>(255, 5f, false);
            player.EnableEffect<AmnesiaItems>(255, 5f, false);
            player.EnableEffect<Deafened>(100, 5f, false);
            player.CurrentItem = null;
        }
    }

    /// <inheritdoc/>
    public override void OnReloaded(Player player, FirearmItem weapon)
    {
        base.OnReloaded(player, weapon);

        weapon.StoredAmmo = 1;
    }
}
