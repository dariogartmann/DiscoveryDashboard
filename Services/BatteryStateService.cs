using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using System.Text;

namespace RoverCopilot.Services
{
    public class BatteryStateService : IBatteryStateService
    {
        private const string BATTERY_NAME = "SmartBat-A06331"; //  SmartBat-A06331   [Monitor] Samsung M7 (32)
        private const string CHA_UUID1 = "0000fff4-0000-1000-8000-00805f9b34fb";
        private const string CHA_UUID2 = "0000fff6-0000-1000-8000-00805f9b34fb"; // RX
        private const string DES_UUID1 = "00002902-0000-1000-8000-00805f9b34fb";
        private const string SERVICE_UUID1 = "0000fff0-0000-1000-8000-00805f9b34fb";

        private readonly IBluetoothLE _bluetooth;
        private readonly IAdapter _adapter;
        private List<IDevice> _devices = new();
        private IDevice _battery;
        private bool _batteryConnected = false;
        private List<string> _infotexts = new();

        private List<byte[]> sendValueByte1 = new List<byte[]>()
        {
            Encoding.Default.GetBytes("+RAA0202"),
            Encoding.Default.GetBytes("+RAA0A03"),
            Encoding.Default.GetBytes("+RAA0802"),
            Encoding.Default.GetBytes("+RAA2C02"),
            Encoding.Default.GetBytes("+RAA1002"),
        };
        private List<byte[]> sendValueByte2 = new List<byte[]>();
        private List<byte[]> sendValueByte3 = new List<byte[]>();

        public BatteryStateService()
        {
            _bluetooth = CrossBluetoothLE.Current;
            _adapter = _bluetooth.Adapter;
        }

        public async Task<bool> ConnectToBattery()
        {
            await StartScanningForDevices();

            foreach (var device in _devices)
            {
                if(device.Name == BATTERY_NAME)
                {
                    await _adapter.StopScanningForDevicesAsync();

                    try
                    {
                        var battery = await _adapter.ConnectToKnownDeviceAsync(device.Id);

                        if (battery != null)
                        {
                            _battery = battery;
                            _batteryConnected = true;
                        }
                    }
                    catch (Exception)
                    {
                        _batteryConnected = false;
                    }
                }
            }
            return _batteryConnected;
        }

        public bool IsConnectedToBattery()
        {
            return _batteryConnected;
        }

        public async Task<List<string>> GetBatteryData()
        {
            if (!IsConnectedToBattery())
            {
                bool connected = await ConnectToBattery();

                if (!connected)
                {
                    return new List<string> { "Battery not connected." };
                }
            }

            try
            {
                var service = await _battery.GetServiceAsync(Guid.Parse(SERVICE_UUID1));

                var characteristic1 = await service.GetCharacteristicAsync(Guid.Parse(CHA_UUID1));
                var characteristic2 = await service.GetCharacteristicAsync(Guid.Parse(CHA_UUID2));

                var descriptor1 = await characteristic1.GetDescriptorAsync(Guid.Parse(DES_UUID1));

                if (characteristic1 != null && characteristic1.CanWrite)
                {
                    characteristic1.ValueUpdated -= OnCharacteristic1ValueChanged;
                    characteristic1.ValueUpdated += OnCharacteristic1ValueChanged;
                    await characteristic1.StartUpdatesAsync();

                    // await characteristic1.WriteAsync(sendValueByte1[0]);
                }
            }
            catch (Exception exception)
            {
                // TODO Connection failed.
                return new List<string> { exception.Message };
            }

            return _infotexts;
        }

        public List<string>  GetCurrentLocalData()
        {
            return _infotexts;
        }

        private void OnCharacteristic1ValueChanged(object sender, CharacteristicUpdatedEventArgs characteristicUpdatedEventArgs)
        {
            var bytes = characteristicUpdatedEventArgs.Characteristic.Value;

            if (bytes != null)
            {
                _infotexts.Add(Encoding.Default.GetString(bytes));
            }
        }

        private async Task StartScanningForDevices()
        {
            if (_bluetooth == null)
            {
                // TODO Exception Handling
                return;
            }
            if (_adapter == null)
            {
                return;
            }

            _adapter.DeviceDiscovered -= OnDeviceDiscovered;
            _adapter.DeviceAdvertised -= OnDeviceDiscovered;
            _adapter.ScanTimeoutElapsed -= Adapter_ScanTimeoutElapsed;

            _adapter.DeviceDiscovered += OnDeviceDiscovered;
            _adapter.DeviceAdvertised += OnDeviceDiscovered;
            _adapter.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;
            _adapter.ScanMode = ScanMode.LowLatency;
            try
            {
                await _adapter.StartScanningForDevicesAsync();
            }
            catch (Exception ex)
            {
                throw;
                // handle
            }
        }

        private void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            // CleanupCancellationToken();
        }

        private void OnDeviceDiscovered(object sender, DeviceEventArgs args)
        {
            if (!_devices.Contains(args.Device))
            {
                _devices.Add(args.Device);
            }
        }
        private static byte[] GetBytes(string text)
        {
            return text.Split(' ').Where(token => !string.IsNullOrEmpty(token)).Select(token => Convert.ToByte(token, 16)).ToArray();
        }
    }
}

/*
 *         BluStaValue.sendValueByte1.clear();
        BluStaValue.sendValueByte2.clear();
        BluStaValue.sendValueByte3.clear();
        if (BluStaValue.deviceType == 1) {
            BluStaValue.sendValueByte1.add("+RAA0202".getBytes());
            BluStaValue.sendValueByte1.add("+RAA0A03".getBytes());
            BluStaValue.sendValueByte1.add("+RAA0802".getBytes());
            BluStaValue.sendValueByte1.add("+RAA2C02".getBytes());
            BluStaValue.sendValueByte1.add("+RAA1002".getBytes());
            BluStaValue.sendValueByte2.add("+RAA0C02".getBytes());
            BluStaValue.sendValueByte2.add("+RAA0403".getBytes());
            BluStaValue.sendValueByte2.add("+RAA3C03".getBytes());
            BluStaValue.sendValueByte2.add("+RAA0603".getBytes());
            BluStaValue.sendValueByte2.add("+RAA1802".getBytes());
            BluStaValue.sendValueByte2.add("+RAA1A02".getBytes());
            BluStaValue.sendValueByte2.add("+RAA2802".getBytes());
            BluStaValue.sendValueByte2.add("+RAA4802".getBytes());
            BluStaValue.sendValueByte2.add("+RAA0202".getBytes());
        } else {
            BluStaValue.sendValueByte1.add("+R160D01".getBytes());
            BluStaValue.sendValueByte1.add("+R160A03".getBytes());
            BluStaValue.sendValueByte1.add("+R160902".getBytes());
            BluStaValue.sendValueByte1.add("+R161702".getBytes());
            BluStaValue.sendValueByte2.add("+R160802".getBytes());
            BluStaValue.sendValueByte2.add("+R160F03".getBytes());
            BluStaValue.sendValueByte2.add("+R161803".getBytes());
            BluStaValue.sendValueByte2.add("+R161003".getBytes());
            BluStaValue.sendValueByte2.add("+R161202".getBytes());
            BluStaValue.sendValueByte2.add("+R161302".getBytes());
            BluStaValue.sendValueByte2.add("+R161C02".getBytes());
            BluStaValue.sendValueByte2.add("+R161B02".getBytes());
            BluStaValue.sendValueByte2.add("+R160D01".getBytes());
            BluStaValue.sendValueByte3.add("+R160D01".getBytes());
            BluStaValue.sendValueByte3.add("+R160902".getBytes());
            BluStaValue.sendValueByte3.add("+R163F02".getBytes());
            BluStaValue.sendValueByte3.add("+R163E02".getBytes());
            BluStaValue.sendValueByte3.add("+R163D02".getBytes());
            BluStaValue.sendValueByte3.add("+R163C02".getBytes());
            BluStaValue.sendValueByte3.add("+R163B02".getBytes());
            BluStaValue.sendValueByte3.add("+R163A02".getBytes());
            BluStaValue.sendValueByte3.add("+R163902".getBytes());
            BluStaValue.sendValueByte3.add("+R163802".getBytes());
            BluStaValue.sendValueByte3.add("+R163702".getBytes());
            BluStaValue.sendValueByte3.add("+R163602".getBytes());
            BluStaValue.sendValueByte3.add("+R163502".getBytes());
            BluStaValue.sendValueByte3.add("+R163402".getBytes());
            BluStaValue.sendValueByte3.add("+R163302".getBytes());
            BluStaValue.sendValueByte3.add("+R163202".getBytes());
            BluStaValue.sendValueByte3.add("+R163102".getBytes());
        }
*/