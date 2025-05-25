namespace CompoundV24.Commands
{
    using CommandSystem;
    using CompoundV24.API.Features.Powers.Interfaces;
    using CompoundV24.API.Features.Powers;
    using CompoundV24.API.Features.Powers.Superpowers;
    using Exiled.API.Features;
    using System;
    using System.Linq;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class PowerCommand : ICommand
    {
        private PowerManager PowerManager => PowerManager.Instance;

        public string Command => "compoundvpowers";

        public string[] Aliases => new string[] { "power", "superpowers", "compoundv" };

        public string Description => "Lists or gives powers by name or index.";

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

            if (Player.TryGet(arguments.At(0), out Player dummy) && dummy.IsNPC && PowerManager.Instance.Registered.TryGet(int.Parse(arguments.At(1)), out Superpower test))
            {
                test.Grant(dummy);
                if (test is IAbilityPower p)
                {
                    p.OnUsedAbility(player);
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
