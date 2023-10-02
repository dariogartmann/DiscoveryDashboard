using Microsoft.AspNetCore.Components;
using Plugin.BLE;
using RoverCopilot.Data.Models;
using RoverCopilot.Services;

namespace RoverCopilot.UI.Pages
{
    public partial class Index
    {
        private List<MaintenanceEntry> _dueMaintenances = new();
        private List<string> _batteryMessages = new();

        public SmartBatteryState BatteryState { get; set; } = SmartBatteryState.StandBy;
        public int BatteryChargePercentage { get; set; } = 76;

        public string BatteryChargeCss => $"{BatteryChargePercentage}%";

        [Inject]
        protected IMaintenanceService MaintenanceService { get; set; }

        [Inject]
        protected IBatteryStateService BatteryStateService { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _dueMaintenances = await MaintenanceService.GetDueMaintenancesAsync();
            await base.OnInitializedAsync();
        }

        public async Task ConnectBattery()
        {
            _batteryMessages.Clear();
            StateHasChanged();

            bool connectionSuccessful = await BatteryStateService.ConnectToBattery();

            if (connectionSuccessful)
            {
                _batteryMessages.Add("Battery connected.");
                StateHasChanged();

                var msgs = await BatteryStateService.GetBatteryData();
                _batteryMessages.AddRange(msgs);
            }
            else
            {
                _batteryMessages.Add("Battery-Connection could not be initialized.");
            }
        }

        public void UpdateBatteryMessages()
        {
            _batteryMessages.AddRange(BatteryStateService.GetCurrentLocalData());
            StateHasChanged();
        }

        public async void CompleteItem(int id)
        {
            await MaintenanceService.CompleteItemForCurrentPeriod(id);
            StateHasChanged();
            _dueMaintenances = await MaintenanceService.GetDueMaintenancesAsync();
        }

        public async Task OpenMapAsync()
        {
            await Map.Default.OpenAsync(1, 2);
        }
    }
}