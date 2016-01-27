using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.SerialCommunication;
using FTDISample.Helpers;
using FTDISample.Serial;

namespace FTDISample
{
    public class DeviceConnection : INotifyPropertyChanged
    {
        public DeviceNode DeviceNode { get; set; }
        private readonly ISerialDevice device;
        private ConnectionState state;
        private byte[] readBuffer;
        private string messageLog;

        public ICommand WriteASCIICommand { get; private set; }
        public ICommand WriteBytesCommand { get; private set; }

        public ConnectionState State
        {
            get { return state; }
            private set { state = value; OnPropertyChanged(); }
        }

        public byte[] ReadBuffer
        {
            get { return readBuffer; }
            private set { readBuffer = value; OnPropertyChanged(); }
        }

        public string MessageLog
        {
            get { return messageLog; }
            private set { messageLog = value; OnPropertyChanged(); }
        }

        public static ConnectionSettings DefaultSettings => new ConnectionSettings(38400, 8, SerialStopBitCount.One, SerialParity.None, SerialHandshake.None);

        public DeviceConnection(DeviceNode deviceNode, ISerialDevice device)
        {
            DeviceNode = deviceNode;
            this.device = device;
            State = ConnectionState.Initializing;

            ReadBuffer = new byte[] { };
            WriteASCIICommand = new DelegateCommand<string>(OnWriteASCII, _ => State == ConnectionState.Ready);
            WriteBytesCommand = new DelegateCommand<string>(OnWriteBytes, _ => State == ConnectionState.Ready);
        }

        public async Task InitializeSettings(ConnectionSettings connectionSettings)
        {
            WriteToLog("Initializing connection.");

            try
            {
                await device.SetConnectionSettings(connectionSettings);

                WriteToLog("Connection settings succesfully applied: {0}.", connectionSettings);
            }
            catch (Exception ex)
            {
                WriteToLog("Unable to apply connection settings, communication may not function properly. Exception={0}.", ex.Message);
            }

            State = ConnectionState.Ready;

            StartReadingData();
            WriteToLog("Connection ready.");
        }

        private async void OnWriteASCII(string message)
        {
            var bytesToWrite = Encoding.ASCII.GetBytes(message + "\r\n").ToArray();
            await WriteBytes(bytesToWrite);
        }

        private async void OnWriteBytes(string message)
        {
            try
            {
                var bytesToWrite = StringToByteArray(message);
                await WriteBytes(bytesToWrite);
            }
            catch (Exception ex)
            {
                WriteToLog("Exception occurred whilst trying to parse hex string: {0}.", ex.Message);
            }
        }

        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private async Task WriteBytes(byte[] bytesToWrite)
        {
            try
            {
                var nrBytesToWrite = bytesToWrite.Length;
                var bytesWritten = await device.WriteAsync(bytesToWrite, (uint)nrBytesToWrite);

                if (bytesWritten != nrBytesToWrite)
                    WriteToLog("Write failed, bytes written: '{0}', count: {1}.", BitConverter.ToString(bytesToWrite), nrBytesToWrite);
                else 
                    WriteToLog("Written: '{0}' to device, count: {1}.", BitConverter.ToString(bytesToWrite), nrBytesToWrite);
            }
            catch (Exception ex)
            {
                WriteToLog("Failed to write: '{0}' to device, Exception={1}.", BitConverter.ToString(bytesToWrite), ex.Message);
            }
        }

        private void WriteToLog(string message, params object[] args)
        {
            MessageLog += string.Format("{0:HH':'mm':'ss'.'fff}: {1}\r\n", DateTime.Now, string.Format(message, args));
        }

        private async void StartReadingData()
        {
            while (true) // todo: build in cancellation support
            {
                try
                {
                    var bytesInQueue = device.GetQueueStatus();
                    bytesInQueue = Math.Max(bytesInQueue, 1); // to make sure we don't create a cpu eating loop

                    var buffer = new byte[bytesInQueue];
                    var bytesRead = await device.ReadAsync(buffer, bytesInQueue);
                    ReadBuffer = ReadBuffer.Concat(bytesRead).ToArray();
                }
                catch (Exception ex)
                {
                    WriteToLog(string.Format("Exception occurred: {0}", ex.Message));
                }
            }
        }

        public enum ConnectionState
        {
            Initializing,
            Ready
        }

        public class ConnectionSettings
        {
            public uint BaudRate { get; }
            public ushort DataBits { get; }
            public SerialStopBitCount StopBits { get; }
            public SerialParity Parity { get; }
            public SerialHandshake Handshake { get; }
            public byte XOn { get; }
            public byte XOff { get; }

            public ConnectionSettings(uint baudRate, ushort dataBits, SerialStopBitCount stopBits, 
                                      SerialParity parity, SerialHandshake handshake, 
                                      byte xOn = 0x00, byte xOff = 0x00)
            {
                BaudRate = baudRate;
                DataBits = dataBits;
                StopBits = stopBits;
                Parity = parity;
                Handshake = handshake;
                XOn = xOn;
                XOff = xOff;
            }

            public override string ToString()
            {
                return $"BaudRate={BaudRate}, WordLength={DataBits}, StopBits={StopBits}, Parity={Parity}, FlowControl={Handshake}, XOn={XOn:X2}, XOff={XOff:X2}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}