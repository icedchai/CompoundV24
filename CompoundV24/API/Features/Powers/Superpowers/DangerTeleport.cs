using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompoundV24.API.Features.Powers.Superpowers;

/// <summary>
/// Teleports the player to a safe spot when they encounter danger.
/// </summary>
public class DangerTeleport : Superpower
{
    /// <inheritdoc/>
    public override string Name { get; set; } = "instinctual_teleportation";

    /// <inheritdoc/>
    public override string Description { get; set; } = "Teleport away when you sense danger. Cannot do it on command.";

    /// <inheritdoc/>
    public override bool IsCompoundV { get; set; } = true;
}
