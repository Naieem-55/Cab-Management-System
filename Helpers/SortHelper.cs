using System.Reflection;

namespace CabManagementSystem.Helpers
{
    public static class SortHelper
    {
        public static IEnumerable<T> ApplySort<T>(IEnumerable<T> source, string? sortOrder)
        {
            if (string.IsNullOrWhiteSpace(sortOrder))
                return source;

            bool desc = sortOrder.EndsWith("_desc", StringComparison.OrdinalIgnoreCase);
            string key = desc ? sortOrder[..^5] : sortOrder;

            var prop = typeof(T).GetProperty(key,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
                return source;

            return desc
                ? source.OrderByDescending(x => prop.GetValue(x))
                : source.OrderBy(x => prop.GetValue(x));
        }
    }
}
