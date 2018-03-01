using System;
using System.Collections.Generic;

namespace EN.RockBlockIridium
{
    public class ISUConfiguration
    {
        public string Name { get; private set; }

        public bool EchoEnabled { get; private set; }
        public QuietModes QuietMode { get; private set; }
        public VerbosityModes VerbosityMode { get; private set; }
        public bool DTROption { get; private set; }
        public FlowControls FlowControl { get; private set; }
        public List<SRegister> Registers { get; private set; }


        internal ISUConfiguration(
            string name,
            bool echoEnabled, QuietModes quietMode,
            VerbosityModes verbosityMode, bool dtrOption,
            FlowControls flowControl,
            List<SRegister> registers)
        {
            Name = name;
            EchoEnabled = echoEnabled;
            QuietMode = quietMode;
            VerbosityMode = verbosityMode;
            DTROption = dtrOption;
            FlowControl = flowControl;
            Registers = registers;
        }
    }
}
