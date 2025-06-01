namespace CompoundV24.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CommandSystem;
    using CompoundV24.API.Features.Powers;
    using CompoundV24.API.Features.Powers.Interfaces;
    using Exiled.API.Features;

    /// <summary>
    /// The command to grant and test powers.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class PowerCommand : ICommand
    {
        private PowerManager PowerManager => PowerManager.Instance;

        /// <inheritdoc/>
        public string Command => "compoundvpowers";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "power", "superpowers", "compoundv" };

        /// <inheritdoc/>
        public string Description => "Lists or gives powers by name or index.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = string.Empty;
            Superpower power = null;
            Player player = Player.Get(sender);

            if (player is null)
            {
                return false;
            }

            if (arguments.Count == 0)
            {
                for (int i = 0; i < PowerManager.Registered.Count; i++)
                {
                    power = PowerManager.Registered[i];
                    response += $"\n{power.Name} ({i}): {power.Description}";
                }

                return true;
            }

            if (arguments.Count != 1 && Player.TryGet(arguments.At(1), out Player dummy) && PowerManager.Instance.Registered.TryGet(int.Parse(arguments.At(0)), out Superpower test))
            {
                test.Grant(dummy);
                if (test is IAbilityPower p)
                {
                    p.OnUsedAbility(dummy);
                }

                return false;
            }

            if (int.TryParse(arguments.At(0), out int number))
            {
                PowerManager.Registered.TryGet(number, out power);
            }
            else
            {
                power = PowerManager.Registered.First(p => p.Name == arguments.At(0)) ?? null;
            }

            if (power is not null)
            {
                if (power.Check(player))
                {
                    power.Revoke(player);
                    response = $"Revoked from you power {power.Name}";
                }
                else
                {
                    power.Grant(player);
                    response = $"Granted you power {power.Name}";
                }

                return true;
            }

            return false;
        }
    }
}
