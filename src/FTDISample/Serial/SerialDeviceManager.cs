using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using WindowsSerialDevice = Windows.Devices.SerialCommunication.SerialDevice;

namespace FTDISample.Serial
{
    public class SerialDeviceManager : ISerialDeviceManager
    {
        public async Task<ISerialDevice> OpenByDeviceId(string deviceId)
        {
            var device = await WindowsSerialDevice.FromIdAsync(deviceId);
            return new SerialDevice(device);
        }

        public async Task<IEnumerable<DeviceNode>> GetDeviceList()
        {
            var deviceSelector = WindowsSerialDevice.GetDeviceSelector();
            var devices = await DeviceInformation.FindAllAsync(deviceSelector);

            return devices.Select(x => new DeviceNode(x.Id, x.Name));
        }
    }
}