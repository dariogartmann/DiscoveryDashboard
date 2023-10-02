using RoverCopilot.Data.Database;
using RoverCopilot.Data.Models;

namespace RoverCopilot.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly DiscoveryDatabase _db;

        public MaintenanceService(DiscoveryDatabase db)
        {
            _db = db;
        }

        public async Task<List<MaintenanceEntry>> GetDueMaintenancesAsync()
        {
            List<MaintenanceEntry> entries = await _db.GetItemsAsync();

            return entries.Where(item => item.IsMaintenanceDue(DateTime.UtcNow)).ToList();
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            int result = await _db.DeleteItemAsync(id);
            return result > 0;
        }

        public async Task<bool> SaveItemAsync(MaintenanceEntry item)
        {
            int result = await _db.SaveItemAsync(item);
            return result < 0;
        }

        public async Task<bool> UpdateItemAsync(int id, MaintenanceEntry item)
        {
            int result = await _db.SaveItemAsync(item);
            return result < 0;
        }

        public async Task<bool> CompleteItemForCurrentPeriod(int id)
        {
            var item = await _db.GetItemAsync(id);
            int result = 0;

            if (item != null)
            {
                item.LastCompletedOn = DateTime.UtcNow;
                result = await _db.SaveItemAsync(item);
            }

            return result > 0;
        }

        public async Task<List<MaintenanceEntry>> GetAllMaintenancesAsync()
        {
            return await _db.GetItemsAsync();
        }
    }
}
