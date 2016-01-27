using System.Collections.Generic;

namespace FTDISample.Serial
{
    public interface ISerialDeviceManager
    {
        ISerialDevice OpenByDeviceId(string deviceId);
        IEnumerable<DeviceNode> GetDeviceList();
    }
}