using System;
using System.Collections.Generic;

namespace EN.RockBlockIridium
{
    public class RingIndicationStatus
    {
        public bool TelephonyRingAlertReceived { get; private set; } = false;
        public bool SBDRingAlertReceived { get; private set; }

        public RingIndicationStatus(bool sbdRingAlertReceived)
        {
            SBDRingAlertReceived = sbdRingAlertReceived;
        }
    }
}
