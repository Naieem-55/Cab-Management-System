namespace CabManagementSystem.Models.Enums;

[Flags]
public enum DayOfWeekFlags
{
    None = 0,
    Sunday = 1 << 0,
    Monday = 1 << 1,
    Tuesday = 1 << 2,
    Wednesday = 1 << 3,
    Thursday = 1 << 4,
    Friday = 1 << 5,
    Saturday = 1 << 6,
    Weekdays = Sunday | Monday | Tuesday | Wednesday | Thursday,
    Weekend = Friday | Saturday,
    All = Weekdays | Weekend
}

public static class DayOfWeekFlagsExtensions
{
    public static DayOfWeekFlags ToFlag(this DayOfWeek day) => (DayOfWeekFlags)(1 << (int)day);

    public static bool Includes(this DayOfWeekFlags flags, DayOfWeek day) => (flags & day.ToFlag()) != 0;

    public static string ToDisplayString(this DayOfWeekFlags flags)
    {
        if (flags == DayOfWeekFlags.All) return "Every day";
        if (flags == DayOfWeekFlags.Weekdays) return "Sun–Thu";
        if (flags == DayOfWeekFlags.Weekend) return "Fri–Sat";
        if (flags == DayOfWeekFlags.None) return "No days";

        return string.Join(", ", Enum.GetValues<DayOfWeek>()
            .Where(d => flags.Includes(d))
            .Select(d => d.ToString()[..3]));
    }
}
