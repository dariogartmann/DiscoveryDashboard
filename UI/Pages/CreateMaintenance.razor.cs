using Microsoft.AspNetCore.Components;
using RoverCopilot.Data.Models;
using RoverCopilot.Data.Services;

namespace RoverCopilot.UI.Pages
{
    public partial class CreateMaintenance
    {
        private MaintenanceEntry _model = new();

        [Inject]
        protected IMaintenanceService MaintenanceService { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        public async Task SubmitForm()
        {
            if(_model != null) 
            {
                if (string.IsNullOrEmpty(_model.Title))
                {
                    return;
                }

                await MaintenanceService.SaveItemAsync(_model);

                NavigationManager.NavigateTo("/");
            }
        }
    }
}