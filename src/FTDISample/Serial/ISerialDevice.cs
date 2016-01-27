using System.Collections.Generic;
using System.Threading.Tasks;

namespace FTDISample.Serial
{
    public interface ISerialDevice
    {
        Task SetConnectionSettings(DeviceConnection.ConnectionSettings connectionSettings);
        Task<uint> WriteAsync(byte[] bytesToWrite, uint nrBytesToWrite);
        uint GetQueueStatus();
        Task<IEnumerable<byte>> ReadAsync(byte[] buffer, uint bytesInQueue);
    }
}