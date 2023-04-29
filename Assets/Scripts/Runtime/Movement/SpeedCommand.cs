using System.Collections.Generic;
using qASIC.Console.Commands;

namespace Game.Runtime.Movement
{
    public class SpeedCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "speed";
        public override string Description { get; } = "changes player's speed multiplier";
        public override string Help { get; } = "Use speed; speed <value>";

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0, 1)) return;

            switch (args.Count)
            {
                //no args
                case 1:
                    Log($"Current speed multiplier: {PlayerMovement.SpeedMultiplier}", "info");
                    return;
                //1 args
                case 2:
                    if (!float.TryParse(args[1], out float speed))
                    {
                        ParseException(args[1], "float");
                        return;
                    }

                    PlayerMovement.SpeedMultiplier = speed;
                    Log($"Speed multiplier has been changed to {speed}!", "cheat");
                    return;
            }
        }
    }
}