using System.ComponentModel;
using System.Runtime.CompilerServices;
using FTDI.D2xx.WinRT.Device;

namespace FTDISample
{
    public class DeviceNode : INotifyPropertyChanged
    {
        public DEVICE_TYPE DeviceType { get; }
        public string DeviceId { get; }

        public DeviceNode(IFTDeviceInfoNode device)
        {
            DeviceType = device.DeviceType;
            DeviceId = device.DeviceId;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", DeviceType, DeviceId);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}