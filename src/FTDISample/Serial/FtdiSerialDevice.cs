using System;
using System.Threading.Tasks;
using FTDI.D2xx.WinRT.Device;

namespace FTDISample.Serial
{
    public class FtdiSerialDevice : ISerialDevice
    {
        private readonly IFTDevice ftDevice;

        public FtdiSerialDevice(IFTDevice ftDevice)
        {
            this.ftDevice = ftDevice;
        }

        public async Task SetConnectionSettings(DeviceConnection.ConnectionSettings connectionSettings)
        {
            await ftDevice.SetBaudRateAsync(connectionSettings.BaudRate);
            await ftDevice.SetDataCharacteristicsAsync(connectionSettings.WordLength, connectionSettings.StopBits, connectionSettings.Parity);
            await ftDevice.SetFlowControlAsync(connectionSettings.FlowControl, connectionSettings.XOn, connectionSettings.XOff);
        }

        public Task<uint> WriteAsync(byte[] bytesToWrite, uint nrBytesToWrite)
        {
            return ftDevice.WriteAsync(bytesToWrite, nrBytesToWrite).AsTask();
        }

        public uint GetQueueStatus()
        {
            return ftDevice.GetQueueStatus();
        }

        public Task<uint> ReadAsync(byte[] buffer, uint bytesInQueue)
        {
            return ftDevice.ReadAsync(buffer, bytesInQueue).AsTask();
        }
    }
}