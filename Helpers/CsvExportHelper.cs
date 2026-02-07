using System.Text;

namespace Cab_Management_System.Helpers
{
    public static class CsvExportHelper
    {
        public static byte[] ExportToCsv<T>(IEnumerable<T> data, Dictionary<string, Func<T, object>> columns)
        {
            var sb = new StringBuilder();

            // Header row
            sb.AppendLine(string.Join(",", columns.Keys.Select(EscapeCsvField)));

            // Data rows
            foreach (var item in data)
            {
                var values = columns.Values.Select(func =>
                {
                    var value = func(item);
                    return EscapeCsvField(value?.ToString() ?? "");
                });
                sb.AppendLine(string.Join(",", values));
            }

            return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        }

        private static string EscapeCsvField(string field)
        {
            if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }
    }
}
