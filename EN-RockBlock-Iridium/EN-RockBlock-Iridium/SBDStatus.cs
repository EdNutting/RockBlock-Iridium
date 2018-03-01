using System;
using System.Collections.Generic;

namespace EN.RockBlockIridium
{
    public class SBDStatus
    {
        public bool MobileOriginatedFlag { get; private set; }
        public bool MobileTerminatedFlag { get; private set; }

        public string MobileOriginatedMSN { get; private set; }
        public string MobileTerminatedMSN { get; private set; }

        public SBDStatus(bool moFlag, bool mtFlag, string moMSN, string mtMSN)
        {
            MobileOriginatedFlag = moFlag;
            MobileTerminatedFlag = mtFlag;
            MobileOriginatedMSN = moMSN;
            MobileTerminatedMSN = mtMSN;
        }
    }

    public class SBDStatusExtended : SBDStatus
    {
        public bool RingAlert { get; private set; }
        public bool MessageWaiting { get; private set; }

        public SBDStatusExtended(bool moFlag, bool mtFlag, string moMSN, string mtMSN, 
            bool ra, bool msgWaiting)
            : base(moFlag, mtFlag, moMSN, mtMSN)
        {
            RingAlert = ra;
            msgWaiting = MessageWaiting;
        }
    }
}
