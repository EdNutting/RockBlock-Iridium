using System;
using System.Collections.Generic;

namespace EN.RockBlockIridium
{
    public class SBDSession
    {
        public enum ReceiveStatuses
        {
            None = 0,
            Success = 1,
            Error = 2
        }

        public SBDDetachResponses   MobileOriginatedStatus { get; private set; }
        public string               MobileOriginatedMSN { get; private set; }

        public ReceiveStatuses  MobileTerminatedStatus { get; private set; }
        public string           MobileTerminatedMSN { get; private set; }
        public int              MobileTerminatedMessageLength { get; private set; }
        public int              MobileTerminatedMessagesQueued { get; private set; }

        public SBDSession(
            SBDDetachResponses mobileOriginatedStatus,
            string mobileOriginatedMSN,
            ReceiveStatuses mobileTerminatedStatus,
            string mobileTerminatedMSN,
            int mobileTerminatedMessageLength,
            int mobileTerminatedMessagesQueued)
        {
            MobileOriginatedStatus = mobileOriginatedStatus;
            MobileOriginatedMSN = mobileOriginatedMSN;
            MobileTerminatedStatus = mobileTerminatedStatus;
            MobileTerminatedMSN = mobileTerminatedMSN;
            MobileTerminatedMessageLength = mobileTerminatedMessageLength;
            MobileTerminatedMessagesQueued = mobileTerminatedMessagesQueued;
        }
    }
}
