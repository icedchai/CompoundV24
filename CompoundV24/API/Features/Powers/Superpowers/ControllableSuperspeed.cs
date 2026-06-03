using System.Collections.Generic;
using CustomPlayerEffects;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace CompoundV24.API.Features.Powers.Superpowers;

/// <summary>
/// Controllable superspeed.
/// </summary>
public class ControllableSuperspeed : ToggleablePower
{
    /// <inheritdoc/>
    public override string Name { get; set; } = "superspeed_controlled";

    /// <summary>
    /// Gets or sets the lookup table between <see cref="Player"/>'s and the intensity of their <see cref="MovementBoost"/> without the power enabled.
    /// </summary>
    protected Dictionary<Player, byte> SavedSpeedIntensity { get; set; } = new Dictionary<Player, byte>();

    /// <summary>
    /// Gets or sets the lookup table between <see cref="Player"/>'s and the duration of their <see cref="MovementBoost"/> without the power enabled.
    /// </summary>
    protected Dictionary<Player, float> SavedSpeedDuration { get; set; } = new Dictionary<Player, float>();

    /// <summary>
    /// Gets or sets the movement speed intensity to add to a <see cref="Player"/> with the power enabled.
    /// </summary>
    public byte MovementSpeedIntensity { get; set; } = 255;

    /// <inheritdoc/>
    protected override void DisposeVariablesOnRestart()
    {
        base.DisposeVariablesOnRestart();
        SavedSpeedIntensity = new ();
        SavedSpeedDuration = new ();
    }

    /// <inheritdoc/>
    protected override void OnChangingRole(PlayerChangingRoleEventArgs e)
    {
        base.OnChangingRole(e);
        SavedSpeedIntensity.Remove(e.Player);
        SavedSpeedDuration.Remove(e.Player);
    }

    /// <inheritdoc/>
    protected override void ApplyProperties(Player player)
    {
        base.ApplyProperties(player);
        GameObject objct = GameObject.CreatePrimitive(PrimitiveType.Cube);
        objct.transform.parent = player.GameObject.transform;
        objct.transform.localPosition = Vector3.zero;
        RobinKillComponent robinkill = objct.AddComponent<RobinKillComponent>();
        robinkill.SuperspeedInstance = this;
        robinkill.Player = player;
        robinkill.Init();
    }

    /// <inheritdoc/>
    protected override void RemoveProperties(Player player)
    {
        base.RemoveProperties(player);

        RobinKillComponent robinkill = player.GameObject.GetComponentInChildren<RobinKillComponent>();
        Object.Destroy(robinkill.gameObject);
    }

    /// <inheritdoc/>
    protected override void EnablePower(Player player)
    {
        base.EnablePower(player);

        SavedSpeedIntensity[player] = player.GetEffect<MovementBoost>()?.Intensity ?? 0;
        SavedSpeedDuration[player] = player.GetEffect<MovementBoost>().Duration;

        player.EnableEffect<Invigorated>();
        player.GetEffect<MovementBoost>().Intensity = MovementSpeedIntensity;
    }

    /// <inheritdoc/>
    protected override void DisablePower(Player player)
    {
        base.DisablePower(player);
        player.EnableEffect<MovementBoost>(SavedSpeedIntensity[player], SavedSpeedDuration[player]);
        player.DisableEffect<Invigorated>();
    }
}
