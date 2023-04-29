using System.Collections.Generic;
using qASIC.Console.Commands;

namespace Game.Runtime.Movement
{
    public class NoclipCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "noclip";
        public override string Description { get; } = "toggles noclip";
        public override string[] Aliases { get; } = new string[] { "nc" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0, 1)) return;

            bool state = false;
            switch (args.Count)
            {
                //no args
                case 1:
                    state = !PlayerMovement.Noclip;
                    break;
                //1 args
                case 2:
                    if (!bool.TryParse(args[1], out state))
                    {
                        ParseException(args[1], "bool");
                        return;
                    }

                    break;
            }

            PlayerMovement.Noclip = state;
            Log($"Noclip has been {(state ? "enabled" : "disabled")}!", "cheat");
        }
    }
}