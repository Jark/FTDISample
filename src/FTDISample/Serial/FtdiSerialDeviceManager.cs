using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public Task<ISerialDevice> OpenByDeviceId(string deviceId)
        {
            return Task.FromResult<ISerialDevice>(new FtdiSerialDevice(ftManager.OpenByDeviceID(deviceId))) ;
        }

        public Task<IEnumerable<DeviceNode>> GetDeviceList()
        {
            return Task.FromResult(ftManager.GetDeviceList().Select(x => new DeviceNode(x.DeviceId, x.DeviceType.ToString())));
        }
    }
}