using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace FTDISample.Serial
{
    public class SerialDevice : ISerialDevice
    {
        private readonly Windows.Devices.SerialCommunication.SerialDevice device;

        public SerialDevice(Windows.Devices.SerialCommunication.SerialDevice device)
        {
            this.device = device;
        }

        public Task SetConnectionSettings(DeviceConnection.ConnectionSettings connectionSettings)
        {
            device.BaudRate = connectionSettings.BaudRate;
            device.Handshake = connectionSettings.Handshake;
            device.Parity = connectionSettings.Parity;
            device.StopBits = connectionSettings.StopBits;
            device.DataBits = connectionSettings.DataBits;

            if (connectionSettings.XOn != 0x00 || connectionSettings.XOff != 0x00)
                throw new Exception("Setting the XOn / XOff bytes is not supported for the WindowsSerialDevice.");

            return Task.CompletedTask;
        }

        public async Task<uint> WriteAsync(byte[] bytesToWrite, uint nrBytesToWrite)
        {
            // might need to call FlushAsync as well, but should trust underlying logic to take care of that
            return await device.OutputStream.WriteAsync(bytesToWrite.AsBuffer());
        }

        public uint GetQueueStatus()
        {
            return device.BytesReceived;
        }

        public async Task<IEnumerable<byte>> ReadAsync(byte[] bytesBuffer, uint bytesInQueue)
        {
            var buffer = bytesBuffer.AsBuffer();
            var result = await device.InputStream.ReadAsync(buffer, bytesInQueue, InputStreamOptions.Partial);
            return result.ToArray();
        }
    }
}