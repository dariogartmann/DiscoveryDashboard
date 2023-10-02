using RoverCopilot.Data.Models;

namespace RoverCopilot.Services
{
    public interface IMaintenanceService
    {
        Task<List<MaintenanceEntry>> GetDueMaintenancesAsync();
        Task<List<MaintenanceEntry>> GetAllMaintenancesAsync();
        Task<bool> SaveItemAsync(MaintenanceEntry item);
        Task<bool> UpdateItemAsync(int id, MaintenanceEntry item);
        Task<bool> DeleteItemAsync(int id);
        Task<bool> CompleteItemForCurrentPeriod(int id);
    }
}
