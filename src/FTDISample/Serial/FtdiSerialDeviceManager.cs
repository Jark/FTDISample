using System.Collections.Generic;
using System.Linq;
using FTDI.D2xx.WinRT;

namespace FTDISample.Serial
{
    public class FtdiSerialDeviceManager : ISerialDeviceManager
    {
        private readonly FTManager ftManager;

        public FtdiSerialDeviceManager()
        {
            ftManager = new FTManager();
        }
        public ISerialDevice OpenByDeviceId(string deviceId)
        {
            return new FtdiSerialDevice(ftManager.OpenByDeviceID(deviceId));
        }

        public IEnumerable<DeviceNode> GetDeviceList()
        {
            return ftManager.GetDeviceList().Select(x => new DeviceNode(x));
        }
    }
}