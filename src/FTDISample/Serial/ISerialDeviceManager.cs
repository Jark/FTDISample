using System.Collections.Generic;
using System.Threading.Tasks;

namespace FTDISample.Serial
{
    public interface ISerialDeviceManager
    {
        Task<ISerialDevice> OpenByDeviceId(string deviceId);
        Task<IEnumerable<DeviceNode>> GetDeviceList();
    }
}