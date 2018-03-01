using System;
using System.Collections.Generic;

namespace EN.RockBlockIridium
{
    public class IndicatorEventReporting
    {
        public bool Enable { get; private set; }
        public bool ReportSignalQuality { get; private set; }
        public bool ReportServiceAvailabilityChanges { get; private set; }

        public IndicatorEventReporting(bool enable, bool reportSigQual, bool reportServAvail)
        {
            Enable = enable;
            ReportSignalQuality = reportSigQual;
            ReportServiceAvailabilityChanges = reportServAvail;
        }

        public class Support
        {
            public int[] Modes { get; private set; }
            public int[] SignalQuality { get; private set; }
            public int[] ServiceAvailability { get; private set; }

            public Support(int[] modes, int[] signalQuality, int[] serviceAvailability)
            {
                Modes = modes;
                SignalQuality = signalQuality;
                ServiceAvailability = serviceAvailability;
            }
        }
    }
}
