using System;
using System.Collections.Generic;

namespace EN.RockBlockIridium
{
    public delegate T Parse<T>(List<string> lines, bool OK);

    public interface IISU
    {
        ATCommandResponse<T> RepeatLastCommand<T>(Parse<T> parser);

        ATCommandResponse<bool> RestoreFactorySettings(int configuration);
        ATCommandResponse<bool> SoftReset(int configuration);

        List<ISUConfiguration> GetConfigurations();
        ATCommandResponse<bool> StoreCurrentConfiguration(int configuration);
        ATCommandResponse<bool> SetDefaultResetProfile(int configuration);

        ATCommandResponse<List<SRegister>> GetAllRegisters();

        ATCommandResponse<bool> FlushToEEPROM();
        
        int    TrafficChannelRate { get; }
        int    ROMChecksum { get; }
        bool   ROMChecksumOK { get; }
        int    SoftwareRevisionLevel { get; }
        string ProductDescription { get; }
        string CountryCode { get; }
        string FactoryIdentity { get; }
        string HardwareSpecification { get; }

        bool EchoEnabled { get; set; }
        QuietModes QuietMode { get; set; }
        VerbosityModes VerbosityMode { get; set; }
        bool DTROption { get; set; }
        FlowControls FlowControl { get; set; }

        bool RadioEnabled { get; set; }

        int? RealTimeClock { get; }
        string ManufacturerIdentity { get; }
        string ModelIdentity { get; }
        string Revision { get; }
        string SerialNumber { get; }

        ATCommandResponse<bool> SetIndicatorEventReporting(IndicatorEventReporting value);
        ATCommandResponse<IndicatorEventReporting> GetIndicatorEventReporting();
        ATCommandResponse<IndicatorEventReporting.Support> TestIndicatorEventReporting();

        ATCommandResponse<RingIndicationStatus> GetRingIndicationStatus();
        ATCommandResponse<int> GetSignalQuality();
        ATCommandResponse<List<int>> TestSignalQuality();

        ATCommandResponse<bool> ExecUnlock(string unlockKey);
        ATCommandResponse<LockStatuses> ReadUnlock();

        ATCommandResponse<bool> SetFixedDTERate(FixedDTERates rate);
        ATCommandResponse<FixedDTERates> GetFixedDTERate();
        ATCommandResponse<List<FixedDTERates>> TestFixedDTERate();

        ATCommandResponse<SBDWriteBinaryResponses> SBDWriteBinary(byte[] data);
        ATCommandResponse<byte[]> SBDReadBinary();

        ATCommandResponse<bool> SBDWriteText(string message);
        ATCommandResponse<string> SBDReadText();

        ATCommandResponse<SBDSession> InitiateSBDSession();
        ATCommandResponse<SBDSession> InitiateSBDSessionExtended(bool automaticNotification);
        ATCommandResponse<SBDSession> InitiateSBDSessionExtended(bool automaticNotification, Location location);
        ATCommandResponse<SBDDetachResponses> Detach();

        ATCommandResponse<bool> SetSBDDeliveryShortCode(byte value);
        ATCommandResponse<byte> GetSBDDeliveryShortCode();

        ATCommandResponse<bool> SetSBDMobileTerminatedAlert(bool enable);
        ATCommandResponse<bool> GetSBDMobileTerminatedAlert();
        ATCommandResponse<List<int>> TestSBDMobileTerminatedAlert();

        ATCommandResponse<NetworkRegistrationResponse> ExecSBDNetworkRegistration();
        ATCommandResponse<NetworkRegistrationResponse> ExecSBDNetworkRegistration(Location location);
        ATCommandResponse<NetworkRegistrationStatuses> GetSBDNetworkRegistration();

        ATCommandResponse<SBDAutomaticRegistrationResponse> SetSBDAutomaticRegistration(SBDAutomaticRegistrationModes mode);
        ATCommandResponse<SBDAutomaticRegistrationModes> GetSBDAutomaticRegistration();
        ATCommandResponse<SBDAutomaticRegistrationResponse.Support> TestSBDAutomaticRegistration();

        ATCommandResponse<bool> ClearSBDMessageBuffers(SBDBuffers buffers);
        ATCommandResponse<bool> ClearSBDMOMSN();

        ATCommandResponse<SBDStatus> GetSBDStatus();
        ATCommandResponse<SBDStatusExtended> GetSBDStatusExtended();

        ATCommandResponse<int> SBDTransferMOToMT();
        ATCommandResponse<string> SBDGateway();

        ATCommandResponse<uint> RequestSystemTime();

        ATCommandResponse<bool> DirectSRegisterReference(int r);
        ATCommandResponse<int>  DirectSRegisterRead(int r);
        ATCommandResponse<bool> DirectSRegisterWrite(int r, int value);
        ATCommandResponse<int>  ReferencedSRegisterRead();
        ATCommandResponse<bool> ReferencedSRegisterWrite(int value);
    }
}
