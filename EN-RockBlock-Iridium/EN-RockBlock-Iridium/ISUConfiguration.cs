using System;
using System.Collections.Generic;

namespace EN.RockBlockIridium
{
    public class ISUConfiguration
    {
        public bool EchoEnabled { get; private set; }
        public QuietModes QuietMode { get; private set; }
        public VerbosityModes VerbosityMode { get; private set; }
        public bool DTROption { get; private set; }
        public FlowControls FlowControl { get; private set; }
        
        internal ISUConfiguration(
            bool echoEnabled, QuietModes quietMode,
            VerbosityModes verbosityMode, bool dtrOption,
            FlowControls flowControl)
        {
            EchoEnabled = echoEnabled;
            QuietMode = quietMode;
            VerbosityMode = verbosityMode;
            DTROption = dtrOption;
            FlowControl = flowControl;
        }
    }
}
