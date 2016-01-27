using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.UI.Xaml;
using FTDISample.Helpers;
using FTDISample.Serial;

namespace FTDISample
{
    public class FtdiSampleViewModel : INotifyPropertyChanged
    {
        private readonly ISerialDeviceManager ftManager;
        private readonly DispatcherTimer deviceStatusWatcher;
        private DeviceNode selectedDevice;
        private readonly object locker = new object();
        private DeviceConnection deviceConnection;

        public ObservableCollection<DeviceNode> Devices { get; } = new ObservableCollection<DeviceNode>();
        public ICommand SelectDeviceCommand { get; private set; }

        public DeviceConnection DeviceConnection
        {
            get { return deviceConnection; }
            set { deviceConnection = value; OnPropertyChanged(); }
        }

        public DeviceNode SelectedDevice
        {
            get { return selectedDevice; }
            set { selectedDevice = value; OnPropertyChanged(); }
        }

        public FtdiSampleViewModel(ISerialDeviceManager serialDeviceManager)
        {
            ftManager = serialDeviceManager;
            SelectDeviceCommand = new DelegateCommand<DeviceNode>(OnSelectDevice);

            // for some reason the ftManager returns 0 devices at startup, so poll for new devices
            deviceStatusWatcher = new DispatcherTimer();
            deviceStatusWatcher.Tick += OnTick;
            deviceStatusWatcher.Interval = TimeSpan.FromMilliseconds(1000);
            deviceStatusWatcher.Start();            
        }

        private async void OnSelectDevice(DeviceNode deviceNode)
        {
            DeviceConnection newConnection;
            lock (locker) // make sure we don't create multiple device connections by a trigger happy user
            {
                if (DeviceConnection != null)
                    return;

                var device = ftManager.OpenByDeviceId(deviceNode.DeviceId);
                newConnection = new DeviceConnection(deviceNode, device);
                DeviceConnection = newConnection;
            }

            var defaultSettings = DeviceConnection.DefaultSettings;
            await newConnection.InitializeSettings(defaultSettings);
        }

        private void OnTick(object sender, object e)
        {
            var devicesList = ftManager.GetDeviceList().ToList();
            
            // add devices we don't have yet
            var devicesToAdd = devicesList.Where(x => Devices.All(y => y.DeviceId != x.DeviceId)).ToList();
            foreach (var device in devicesToAdd)
                Devices.Add(device);

            // remove any devices that are no longer connected
            var devicesToDelete = Devices.Where(x => devicesList.All(y => y.DeviceId != x.DeviceId)).ToList();
            foreach (var deviceNode in devicesToDelete)
                Devices.Remove(deviceNode);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}