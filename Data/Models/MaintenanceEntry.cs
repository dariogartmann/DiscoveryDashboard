using SQLite;

namespace RoverCopilot.Data.Models
{
    public enum ServiceInterval
    {
        Weekly,
        Monthly,
        Quarterly,
        Yearly
    }

    public class MaintenanceEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ServiceInterval Interval { get; set; }
        public DateTime DueDate { get; set; } = DateTime.UtcNow;
        public DateTime LastCompletedOn { get; set; } = DateTime.MinValue;
        public bool IsMaintenanceDue(DateTime currentDate)
        {
            switch (Interval)
            {
                case ServiceInterval.Yearly:
                    return currentDate.Year >= DueDate.Year && currentDate.Date >= DueDate.Date && LastCompletedOn.AddYears(1) <= currentDate;

                case ServiceInterval.Monthly:
                    return currentDate.Year >= DueDate.Year && currentDate.Month >= DueDate.Month && currentDate.Date >= DueDate.Date && LastCompletedOn.Date.AddMonths(1) <= currentDate;

                case ServiceInterval.Weekly:
                    return currentDate.Year >= DueDate.Year && currentDate.Month >= DueDate.Month && currentDate.DayOfWeek >= DueDate.DayOfWeek && currentDate.Date >= DueDate.Date && LastCompletedOn.AddDays(7) <= currentDate;

                case ServiceInterval.Quarterly:
                    var currentQuarter = (int)Math.Ceiling(currentDate.Month / 3.0);
                    var dueQuarter = (int)Math.Ceiling(DueDate.Month / 3.0);
                    return currentDate.Year >= DueDate.Year && currentQuarter >= dueQuarter && currentDate.Date >= DueDate.Date && LastCompletedOn.AddMonths(3) <= currentDate;
                default:
                    return false;
            }
        }
    }
}

