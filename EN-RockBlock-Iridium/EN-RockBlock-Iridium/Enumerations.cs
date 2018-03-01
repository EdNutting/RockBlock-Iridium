using System;
using System.Collections.Generic;

namespace EN.RockBlockIridium
{
    public enum VerbosityModes
    {
        Unset = -1,
        Numeric = 0,
        Text = 1
    }

    public enum QuietModes
    {
        Unset = -1,
        Responses = 0,
        NoResponse = 1
    }

    public enum FlowControls
    {
        Disable = 0,
        RTS_CTS = 3
    }

    public enum LockStatuses
    {
        Unlocked = 0,
        Locked = 1,
        PermanentlyLocked = 2
    }

    public enum FixedDTERates
    {
        BPS600 = 1,
        BPS1200 = 2,
        BPS2400 = 3,
        BPS4800 = 4,
        BPS9600 = 5,
        BPS19200 = 6,
        BPS38400 = 7,
        BPS57600 = 8,
        BPS115200 = 9
    }

    public enum SBDWriteBinaryResponses
    {
        Success = 0,
        Timeout = 1,
        ChecksumIncorrect = 2,
        MessageSizeIncorrect = 3
    }

    public enum SBDDetachResponses
    {
        NoError = 0,
        Success_MTTooBig = 1,
        Success_LocationUpdateRejected = 2,
        Success_Reserved3 = 3,
        Success_Reserved4 = 4,
        Failure_Reserved1 = 5,
        Failure_Reserved2 = 6,
        Failure_Reserved3 = 7,
        Failure_Reserved4 = 8,
        Failure_Reserved5 = 9,
        GatewayTimeout = 10,
        GatewayFull = 11,
        MOTooManySegments = 12,
        GatewaySessionIncomplete = 13,
        InvalidSegmentSize = 14,
        AccessDenied = 15,
        TranseiverLocked = 16,
        LocalSessionTimeout = 17,
        ConnectionLost = 18,
        Failure_Reserved11 = 19,
        Failure_Reserved12 = 20,
        Failure_Reserved13 = 21,
        Failure_Reserved14 = 22,
        Failure_Reserved15 = 23,
        Failure_Reserved16 = 24,
        Failure_Reserved17 = 25,
        Failure_Reserved18 = 26,
        Failure_Reserved19 = 27,
        Failure_Reserved20 = 28,
        Failure_Reserved21 = 29,
        Failure_Reserved22 = 31,
        NoNetwork = 32,
        AntennaFault = 33,
        RadioDisabled = 34,
        TranseiverBusy = 35,
        Failure_Other = 255
    }

    public enum NetworkRegistrationStatuses
    {
        Detached = 0,
        NotRegistered = 1,
        Registered = 2,
        RegistrationDenied = 3,
        Unknown = 4
    }

    public enum SBDAutomaticRegistrationModes
    {
        Disable = 0,
        Automatic = 1,
        Ask = 2
    }

    public enum SBDBuffers
    {
        MobileOriginated = 0,
        MobileTerminated = 1,
        Both = 2
    }
    
    public enum ResultCodes
    {
        OK = 0,
        SBDRing = 126,
        Error = 4,
        HardwareFailure = 127,
        Ready = 65536,
        AutoRegistrationEventReport = 65537,
        IndicatorEventReport = 65538
    }
}
