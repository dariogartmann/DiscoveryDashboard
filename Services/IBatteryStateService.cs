namespace RoverCopilot.Services
{
    public interface IBatteryStateService
    {
        Task<bool> ConnectToBattery();
        bool IsConnectedToBattery();
        Task<List<string>> GetBatteryData();
        List<string> GetCurrentLocalData();
    }
}
