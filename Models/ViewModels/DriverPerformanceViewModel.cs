namespace Cab_Management_System.Models.ViewModels
{
    public class DriverPerformanceViewModel
    {
        public Driver Driver { get; set; } = null!;
        public double AverageRating { get; set; }
        public int TotalTrips { get; set; }
        public int CompletedTrips { get; set; }
        public double CompletionRate { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalRatings { get; set; }
        public IEnumerable<DriverRating> RecentRatings { get; set; } = new List<DriverRating>();
        public List<int> RatingDistribution { get; set; } = new List<int> { 0, 0, 0, 0, 0 };
        public List<string> MonthLabels { get; set; } = new();
        public List<int> MonthlyCounts { get; set; } = new();
    }
}
