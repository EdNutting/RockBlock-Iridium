using System;
using System.Collections.Generic;

namespace EN.RockBlockIridium
{
    internal class BasicATCommand : IATCommand
    {
        public enum Commands
        {
            Empty,
            RepeatLastCommand,
            Echo,
            Identification,
            QuietMode,
            VerboseMode,
            SoftReset,
            DTROption,
            RestoreFactorySettings,
            FlowControl,
            ViewConfigs,
            StoreActiveConfig,
            DesignDefaultResetProfile,
            DisplayRegisters,
            FlushToEEPROM,
            RadioActivity,
            RealTimeClock
        }

        private readonly static string[] CommandStrings =
        {
            "",
            "A/",
            "E{0}",
            "I{0}",
            "Q{0}",
            "V{0}",
            "Z{0}",
            "&D{0}",
            "&F{0}",
            "&K{0}",
            "&V",
            "&W{0}",
            "&Y{0}",
            "%R",
            "*F",
            "*R{0}"
        };

        private Commands Command;
        private int Param;

        internal BasicATCommand(Commands command, int param)
        {
            Command = command;
            Param = param;
        }

        public string GetCommandString()
        {
            return String.Format(CommandStrings[(int)Command], Param);
        }
    }
}
