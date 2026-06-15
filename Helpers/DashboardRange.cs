namespace CabManagementSystem.Helpers
{
    // Allowed month-range presets for dashboard time-series charts.
    public static class DashboardRange
    {
        public static readonly int[] Allowed = { 3, 6, 12, 24 };

        public static int Normalize(int months) =>
            Allowed.Contains(months) ? months : 6;
    }
}
