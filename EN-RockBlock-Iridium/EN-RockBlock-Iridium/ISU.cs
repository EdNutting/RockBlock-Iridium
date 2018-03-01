using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace EN.RockBlockIridium
{
    public class ISU : IISU, IDisposable
    {
        // TODO

        private SerialPort Port;
        private System.Threading.AutoResetEvent ResponseReadyEvent;
        private System.Threading.AutoResetEvent SBDRingEvent;
        private System.Threading.AutoResetEvent AutoRegEvent;
        private System.Threading.AutoResetEvent ServAvailChngEvent;

        private List<string> BuildingResponseLines;
        private List<List<string>> BufferedResponseLines;

        public event EventHandler<List<string>> OnSBDRing;
        public event EventHandler<List<string>> OnAutoRegistration;
        public event EventHandler<List<string>> OnServiceAvailabilityChange;

        public ISU(string COMPort)
        {
            Port = new SerialPort(COMPort, 19200, Parity.None, 8, StopBits.One);
            Port.Handshake = Handshake.RequestToSend;
            Port.NewLine = "\r\n";
            Port.Encoding = Encoding.ASCII;
            Port.DiscardNull = false;
            Port.DataReceived += Port_DataReceived;

            OnSBDRing += ISU_OnSBDRing;
            OnServiceAvailabilityChange += ISU_OnServiceAvailabilityChange;
        }

        public void Dispose()
        {
            if (Port != null && Port.IsOpen)
            {
                Port.Close();
                Port = null;
            }
        }

        public void Open()
        {
            if (!Port.IsOpen)
            {
                Port.Open();
                ResponseReadyEvent = new System.Threading.AutoResetEvent(false);
                SBDRingEvent = new System.Threading.AutoResetEvent(false);
                AutoRegEvent = new System.Threading.AutoResetEvent(false);
                ServAvailChngEvent = new System.Threading.AutoResetEvent(false);
                BuildingResponseLines = new List<string>();
                BufferedResponseLines = new List<List<string>>();
            }
        }

        public void Close()
        {
            if (Port.IsOpen)
            {
                Port.Close();
            }
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // TODO: Handle non-verbose &/ quiet modes (i.e. binary/non-text data)

            byte[] buffer = new byte[370];
            int bytesRead = Port.Read(buffer, 0, buffer.Length);
            string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            ProcessIncomingData(data);
        }

        private void ProcessIncomingData(string data)
        {
            //Console.WriteLine(data);
            string[] lines = data.Split("\r\n".ToCharArray());
            if (BuildingResponseLines.Count > 0)
            {
                BuildingResponseLines[BuildingResponseLines.Count - 1] += lines[0];
            }
            else
            {
                BuildingResponseLines.Add(lines[0]);
            }
            for (int i = 1; i < lines.Length; i++)
            {
                BuildingResponseLines.Add(lines[i]);
            }
            BuildingResponseLines.RemoveAll(x => string.IsNullOrWhiteSpace(x));

            string lastLine = BuildingResponseLines[BuildingResponseLines.Count - 1].Trim();
            if (lastLine.StartsWith("OK") ||
                lastLine.StartsWith("ERROR") ||
                lastLine.StartsWith("HARDWARE FAILURE"))
            {
                BufferedResponseLines.Add(BuildingResponseLines);
                BuildingResponseLines = new List<string>();
                ResponseReadyEvent.Set();
            }
            else if (lastLine.StartsWith("SBDRING"))
            {
                BufferedResponseLines.Add(BuildingResponseLines);
                OnSBDRing.Invoke(this, BuildingResponseLines);
                BuildingResponseLines = new List<string>();
                SBDRingEvent.Set();
            }
            else if (lastLine.StartsWith("+AREG"))
            {
                BufferedResponseLines.Add(BuildingResponseLines);
                OnAutoRegistration.Invoke(this, BuildingResponseLines);
                BuildingResponseLines = new List<string>();
                AutoRegEvent.Set();
            }
            else if (lastLine.StartsWith("+CIEV"))
            {
                BufferedResponseLines.Add(BuildingResponseLines);
                OnServiceAvailabilityChange.Invoke(this, BuildingResponseLines);
                BuildingResponseLines = new List<string>();
                ServAvailChngEvent.Set();
            }
            else
            {
                if (data.EndsWith("\r") || data.EndsWith("\n"))
                {
                    BuildingResponseLines.Add("");
                }
            }
        }

        private void ISU_OnSBDRing(object sender, List<string> e)
        {
            // TODO: Any internal handling required?
        }

        private void ISU_OnServiceAvailabilityChange(object sender, List<string> e)
        {
            // TODO: Any internal handling required?
        }

        private void IssueCommand(IATCommand command)
        {
            string CommandString = command.GetCommandString();
            CommandString = CommandString.ToUpper();
            Port.WriteLine("AT" + CommandString);
            Port.BaseStream.Flush();
        }

        public void AbortCommand()
        {
            Port.Write("\x30");
            ResponseReadyEvent.Set();
            BuildingResponseLines.Clear();
            BufferedResponseLines.Clear();
        }

        private void WaitForCommandResponse()
        {
            ResponseReadyEvent.WaitOne();
        }

        public void WaitForSBDRing()
        {
            SBDRingEvent.WaitOne();
        }

        public void WaitForAutoRegistration()
        {
            AutoRegEvent.WaitOne();
        }

        public void WaitForServiceAvailabilityChange()
        {
            ServAvailChngEvent.WaitOne();
        }

        private ATCommandResponse<T> ParseResponse<T>(List<string> lines, Parse<T> parser)
        {
            lines.RemoveAll(x => string.IsNullOrWhiteSpace(x));
            if (lines[0].StartsWith("AT"))
            {
                lines.RemoveAt(0);
            }
            bool OK = lines[lines.Count - 1].StartsWith("OK") || lines[lines.Count - 1].StartsWith("READY");
            lines.RemoveAt(lines.Count - 1);
            T data = parser(lines, OK);
            return new ATCommandResponse<T>(data, !OK, lines);
        }

        private List<string> PopOldestLines()
        {
            List<string> lines = BufferedResponseLines[0];
            BufferedResponseLines.RemoveAt(0);
            return lines;
        }

        public ATCommandResponse<object> IssueEmptyCommand()
        {
            IssueCommand(new BasicATCommand(BasicATCommand.Commands.Empty, 0));
            WaitForCommandResponse();
            return ParseResponse<object>(PopOldestLines(), (x, y) => null);
        }

        public ATCommandResponse<T> RepeatLastCommand<T>(Parse<T> parser)
        {
            IssueCommand(new BasicATCommand(BasicATCommand.Commands.RepeatLastCommand, 0));
            WaitForCommandResponse();
            return ParseResponse(PopOldestLines(), parser);
        }

        public ATCommandResponse<bool> RestoreFactorySettings(int configuration)
        {
            IssueCommand(new BasicATCommand(BasicATCommand.Commands.RestoreFactorySettings, configuration));
            WaitForCommandResponse();
            return ParseResponse(PopOldestLines(), (x, OK) => OK);
        }
        public ATCommandResponse<bool> SoftReset(int configuration)
        {
            IssueCommand(new BasicATCommand(BasicATCommand.Commands.SoftReset, configuration));
            WaitForCommandResponse();
            return ParseResponse(PopOldestLines(), (x, OK) => OK);
        }

        public List<ISUConfiguration> GetConfigurations()
        {
            IssueCommand(new BasicATCommand(BasicATCommand.Commands.ViewConfigs, 0));
            WaitForCommandResponse();
            return ParseResponse(PopOldestLines(), (lines, OK) =>
            {
                List<string> ConfigLines = new List<string>();
                string CurrentLine = "";
                foreach (string line in lines)
                {
                    string tLine = line.Trim();
                    if (tLine.EndsWith(":") && !string.IsNullOrWhiteSpace(CurrentLine))
                    {
                        ConfigLines.Add(CurrentLine);
                        CurrentLine = "";
                    }
                    CurrentLine += line;
                }
                if (!string.IsNullOrWhiteSpace(CurrentLine))
                {
                    ConfigLines.Add(CurrentLine);
                }

                List<ISUConfiguration> configs = new List<ISUConfiguration>();
                foreach (string confLine in ConfigLines)
                {
                    string[] colonSplitFirst = confLine.Split(":".ToCharArray());
                    string name = colonSplitFirst[0];
                    string[] parts = string.Join(":", colonSplitFirst, 1, colonSplitFirst.Length - 1).Split(" ".ToCharArray());
                    bool echoEnabled = false;
                    QuietModes quietMode = QuietModes.Unset;
                    VerbosityModes verbosityMode = VerbosityModes.Unset;
                    bool dtrOption = false;
                    FlowControls flowControl = FlowControls.Disable;
                    List<SRegister> registers = new List<SRegister>();
                    foreach (string part in parts)
                    {
                        if (string.IsNullOrWhiteSpace(part))
                        {
                            continue;
                        }

                        if (part[0] == 'E')
                        {
                            echoEnabled = part[1] == '1';
                        }
                        else if (part[0] == 'Q')
                        {
                            quietMode = (QuietModes)(part[1] - '0');
                        }
                        else if (part[0] == 'V')
                        {
                            verbosityMode = (VerbosityModes)(part[1] - '0');
                        }
                        else if (part.StartsWith("&D"))
                        {
                            dtrOption = part[2] != '0';
                        }
                        else if (part.StartsWith("&K"))
                        {
                            flowControl = (FlowControls)(part[2] - '0');
                        }
                        else if (part[0] == 'S')
                        {
                            string[] regParts = part.Split(":".ToCharArray());
                            string regIdxStr = regParts[0].Substring(1);
                            string regValStr = regParts[1];
                            registers.Add(new SRegister(int.Parse(regIdxStr), int.Parse(regValStr)));
                        }
                    }
                    configs.Add(new ISUConfiguration(name, echoEnabled, quietMode, verbosityMode, dtrOption, flowControl, registers));
                }
                return configs;
            }).Data;
        }

        public ATCommandResponse<bool> StoreCurrentConfiguration(int configuration)
        {
            IssueCommand(new BasicATCommand(BasicATCommand.Commands.StoreActiveConfig, configuration));
            WaitForCommandResponse();
            return ParseResponse(PopOldestLines(), (x, OK) => OK);
        }
        public ATCommandResponse<bool> SetDefaultResetProfile(int configuration)
        {
            IssueCommand(new BasicATCommand(BasicATCommand.Commands.DesignateDefaultResetProfile, configuration));
            WaitForCommandResponse();
            return ParseResponse(PopOldestLines(), (x, OK) => OK);
        }

        public ATCommandResponse<List<SRegister>> GetAllRegisters()
        {
            IssueCommand(new BasicATCommand(BasicATCommand.Commands.DisplayRegisters, 0));
            WaitForCommandResponse();
            return ParseResponse(PopOldestLines(), (lines, OK) =>
            {
                List<SRegister> registers = new List<SRegister>();
                for (int i = 1; i < lines.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(lines[i]))
                    {
                        string[] lineParts = lines[i].Split(" ".ToCharArray());
                        if (lineParts.Length > 1)
                        {
                            registers.Add(new SRegister(int.Parse(lineParts[0].Substring(1)), int.Parse(lineParts[1])));
                        }
                        if (lineParts.Length > 4)
                        {
                            registers.Add(new SRegister(int.Parse(lineParts[4].Substring(1)), int.Parse(lineParts[5])));
                        }
                    }
                }
                return registers;
            });
        }

        public ATCommandResponse<bool> FlushToEEPROM()
        {
            IssueCommand(new BasicATCommand(BasicATCommand.Commands.FlushToEEPROM, 0));
            WaitForCommandResponse();
            return ParseResponse(PopOldestLines(), (x, OK) => OK);
        }

        public int TrafficChannelRate
        {
            get
            {
                IssueCommand(new BasicATCommand(BasicATCommand.Commands.Identification, 0));
                WaitForCommandResponse();
                return ParseResponse(PopOldestLines(), (lines, OK) => int.Parse(lines[0])).Data;
            }
        }
        public int ROMChecksum
        {
            get
            {
                IssueCommand(new BasicATCommand(BasicATCommand.Commands.Identification, 1));
                WaitForCommandResponse();
                return ParseResponse(PopOldestLines(), (lines, OK) => int.Parse(lines[0])).Data;
            }
        }
        public bool ROMChecksumOK
        {
            get
            {
                IssueCommand(new BasicATCommand(BasicATCommand.Commands.Identification, 2));
                WaitForCommandResponse();
                return ParseResponse(PopOldestLines(), (lines, OK) => lines[0] == "OK").Data;
            }
        }
        public int SoftwareRevisionLevel
        {
            get
            {
                IssueCommand(new BasicATCommand(BasicATCommand.Commands.Identification, 3));
                WaitForCommandResponse();
                return ParseResponse(PopOldestLines(), (lines, OK) => int.Parse(lines[0])).Data;
            }
        }
        public string ProductDescription
        {
            get
            {
                IssueCommand(new BasicATCommand(BasicATCommand.Commands.Identification, 4));
                WaitForCommandResponse();
                return ParseResponse(PopOldestLines(), (lines, OK) => lines[0]).Data;
            }
        }
        public string CountryCode
        {
            get
            {
                IssueCommand(new BasicATCommand(BasicATCommand.Commands.Identification, 5));
                WaitForCommandResponse();
                return ParseResponse(PopOldestLines(), (lines, OK) => lines[0]).Data;
            }
        }
        public string FactoryIdentity
        {
            get

            {
                IssueCommand(new BasicATCommand(BasicATCommand.Commands.Identification, 6));
                WaitForCommandResponse();
                return ParseResponse(PopOldestLines(), (lines, OK) => lines[0]).Data;
            }
        }
        public string HardwareSpecification
        {
            get
            {
                IssueCommand(new BasicATCommand(BasicATCommand.Commands.Identification, 7));
                WaitForCommandResponse();
                return ParseResponse(PopOldestLines(), (lines, OK) => lines[0]).Data;
            }
        }

        public bool EchoEnabled
        {
            get
            {
                var conf = GetConfigurations().Find(x => x.Name == "ACTIVE PROFILE");
                return conf.EchoEnabled;
            }
            set
            {
                IssueCommand(new BasicATCommand(BasicATCommand.Commands.Echo, value ? 1 : 0));
                WaitForCommandResponse();
                PopOldestLines();
            }
        }
        public QuietModes QuietMode
        {
            get
            {
                var conf = GetConfigurations().Find(x => x.Name == "ACTIVE PROFILE");
                return conf.QuietMode;
            }
            set
            {
                IssueCommand(new BasicATCommand(BasicATCommand.Commands.QuietMode, (int)value));
                WaitForCommandResponse();
                PopOldestLines();
            }
        }
        public VerbosityModes VerbosityMode
        {
            get
            {
                var conf = GetConfigurations().Find(x => x.Name == "ACTIVE PROFILE");
                return conf.VerbosityMode;
            }
            set
            {
                IssueCommand(new BasicATCommand(BasicATCommand.Commands.VerboseMode, (int)value));
                WaitForCommandResponse();
                PopOldestLines();
            }
        }
        public bool DTROption
        {
            get
            {
                var conf = GetConfigurations().Find(x => x.Name == "ACTIVE PROFILE");
                return conf.DTROption;
            }
            set
            {
                IssueCommand(new BasicATCommand(BasicATCommand.Commands.DTROption, value ? 2 : 0));
                WaitForCommandResponse();
                PopOldestLines();
            }
        }
        public FlowControls FlowControl
        {
            get
            {
                var conf = GetConfigurations().Find(x => x.Name == "ACTIVE PROFILE");
                return conf.FlowControl;
            }
            set
            {
                IssueCommand(new BasicATCommand(BasicATCommand.Commands.FlowControl, (int)value));
                WaitForCommandResponse();
                PopOldestLines();
            }
        }

        private bool _RadioEnabled = false;
        public bool RadioEnabled
        {
            get
            {
                return _RadioEnabled;
            }
            set
            {
                _RadioEnabled = value;
                IssueCommand(new BasicATCommand(BasicATCommand.Commands.RadioActivity, value ? 1 : 0));
                WaitForCommandResponse();
                PopOldestLines();
            }
        }

        public int? RealTimeClock
        {
            get
            {
                IssueCommand(new ExtendedATCommand(ExtendedATCommand.Groups.Cellular, "CLK", false));
                WaitForCommandResponse();
                return int.Parse(PopOldestLines()[0]);
            }
        }
        public string ManufacturerIdentity
        {
            get
            {
                IssueCommand(new ExtendedATCommand(ExtendedATCommand.Groups.Cellular, "GMI", false));
                WaitForCommandResponse();
                return PopOldestLines()[0];
            }
        }
        public string ModelIdentity
        {
            get
            {
                IssueCommand(new ExtendedATCommand(ExtendedATCommand.Groups.Cellular, "GMM", false));
                WaitForCommandResponse();
                return PopOldestLines()[0];
            }
        }
        public string Revision
        {
            get
            {
                IssueCommand(new ExtendedATCommand(ExtendedATCommand.Groups.Cellular, "GMR", false));
                WaitForCommandResponse();
                return PopOldestLines()[0];
            }
        }
        public string SerialNumber
        {
            get
            {
                IssueCommand(new ExtendedATCommand(ExtendedATCommand.Groups.Cellular, "GSN", false));
                WaitForCommandResponse();
                return PopOldestLines()[0];
            }
        }

        public ATCommandResponse<bool> SetIndicatorEventReporting(IndicatorEventReporting value)
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<IndicatorEventReporting> GetIndicatorEventReporting()
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<IndicatorEventReporting.Support> TestIndicatorEventReporting()
        {
            throw new NotImplementedException();
        }

        public ATCommandResponse<RingIndicationStatus> GetRingIndicationStatus()
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<int> GetSignalQuality()
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<List<int>> TestSignalQuality()
        {
            throw new NotImplementedException();
        }

        public ATCommandResponse<bool> ExecUnlock(string unlockKey)
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<LockStatuses> ReadUnlock()
        {
            throw new NotImplementedException();
        }

        public ATCommandResponse<bool> SetFixedDTERate(FixedDTERates rate)
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<FixedDTERates> GetFixedDTERate()
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<List<FixedDTERates>> TestFixedDTERate()
        {
            throw new NotImplementedException();
        }

        public ATCommandResponse<SBDWriteBinaryResponses> SBDWriteBinary(byte[] data)
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<byte[]> SBDReadBinary()
        {
            throw new NotImplementedException();
        }

        public ATCommandResponse<bool> SBDWriteText(string message)
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<string> SBDReadText()
        {
            throw new NotImplementedException();
        }

        public ATCommandResponse<SBDSession> InitiateSBDSession()
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<SBDSession> InitiateSBDSessionExtended(bool automaticNotification)
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<SBDSession> InitiateSBDSessionExtended(bool automaticNotification, Location location)
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<SBDDetachResponses> Detach()
        {
            throw new NotImplementedException();
        }

        public ATCommandResponse<bool> SetSBDDeliveryShortCode(byte value)
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<byte> GetSBDDeliveryShortCode()
        {
            throw new NotImplementedException();
        }

        public ATCommandResponse<bool> SetSBDMobileTerminatedAlert(bool enable)
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<bool> GetSBDMobileTerminatedAlert()
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<List<int>> TestSBDMobileTerminatedAlert()
        {
            throw new NotImplementedException();
        }

        public ATCommandResponse<NetworkRegistrationResponse> ExecSBDNetworkRegistration()
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<NetworkRegistrationResponse> ExecSBDNetworkRegistration(Location location)
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<NetworkRegistrationStatuses> GetSBDNetworkRegistration()
        {
            throw new NotImplementedException();
        }

        public ATCommandResponse<SBDAutomaticRegistrationResponse> SetSBDAutomaticRegistration(SBDAutomaticRegistrationModes mode)
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<SBDAutomaticRegistrationModes> GetSBDAutomaticRegistration()
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<SBDAutomaticRegistrationResponse.Support> TestSBDAutomaticRegistration()
        {
            throw new NotImplementedException();
        }

        public ATCommandResponse<bool> ClearSBDMessageBuffers(SBDBuffers buffers)
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<bool> ClearSBDMOMSN()
        {
            throw new NotImplementedException();
        }

        public ATCommandResponse<SBDStatus> GetSBDStatus()
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<SBDStatusExtended> GetSBDStatusExtended()
        {
            throw new NotImplementedException();
        }

        public ATCommandResponse<int> SBDTransferMOToMT()
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<string> SBDGateway()
        {
            throw new NotImplementedException();
        }

        public ATCommandResponse<uint> RequestSystemTime()
        {
            throw new NotImplementedException();
        }

        public ATCommandResponse<bool> DirectSRegisterReference(int r)
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<int> DirectSRegisterRead(int r)
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<bool> DirectSRegisterWrite(int r, int value)
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<int> ReferencedSRegisterRead()
        {
            throw new NotImplementedException();
        }
        public ATCommandResponse<bool> ReferencedSRegisterWrite(int value)
        {
            throw new NotImplementedException();
        }
    }
}
