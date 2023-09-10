using Microsoft.AspNetCore.Components;
using RoverCopilot.Data.Models;
using RoverCopilot.Data.Services;

namespace RoverCopilot.UI.Pages
{
    public partial class Index
    {
        private List<MaintenanceEntry> _dueMaintenances = new();

        [Inject]
        protected IMaintenanceService MaintenanceService { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _dueMaintenances = await MaintenanceService.GetDueMaintenancesAsync();
            await base.OnInitializedAsync();
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