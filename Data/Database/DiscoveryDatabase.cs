using RoverCopilot.Data.Models;
using SQLite;

namespace RoverCopilot.Data.Database
{
    public class DiscoveryDatabase
    {
        private SQLiteAsyncConnection dbConnection;

        public async Task InitOnFirstTime()
        {
            if (dbConnection != null)
                return;

            dbConnection = new SQLiteAsyncConnection(DbConstants.DatabasePath, DbConstants.Flags);

            await dbConnection.CreateTableAsync<MaintenanceEntry>();
        }

        public async Task<List<MaintenanceEntry>> GetItemsAsync()
        {
            await InitOnFirstTime();
            return await dbConnection.Table<MaintenanceEntry>().ToListAsync();
        }

        public async Task<MaintenanceEntry> GetItemAsync(int id)
        {
            await InitOnFirstTime();
            return await dbConnection.Table<MaintenanceEntry>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }
        public async Task<int> SaveItemAsync(MaintenanceEntry item)
        {
            await InitOnFirstTime();
            if (item.Id != 0)
                return await dbConnection.UpdateAsync(item);
            else
                return await dbConnection.InsertAsync(item);
        }

        public async Task<int> DeleteItemAsync(int id)
        {
            await InitOnFirstTime();
            return await dbConnection.DeleteAsync(id);
        }
    }
}
