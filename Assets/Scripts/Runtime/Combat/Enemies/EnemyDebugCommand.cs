using System.Collections.Generic;
using qASIC.Console.Commands;

namespace Game.Runtime.Combat.Enemies
{
    public class EnemyDebugCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "enemydebug";
        public override string Description { get; } = "toggles enemy debug information";
        public override string[] Aliases { get; } = new string[] { "ed" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0, 1)) return;

            bool state = false;
            switch (args.Count)
            {
                //no args
                case 1:
                    state = !Enemy.ShowDebug;
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

            Enemy.ShowDebug = state;
            Log($"Enemy debug information has been {(state ? "enabled" : "disabled")}!", "cheat");
        }
    }
}
