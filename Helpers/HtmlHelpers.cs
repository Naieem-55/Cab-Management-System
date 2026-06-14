using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CabManagementSystem.Helpers
{
    public static class HtmlHelpers
    {
        public static IHtmlContent SortableHeader(this IHtmlHelper html, string displayName, string sortKey)
        {
            var ctx = html.ViewContext.HttpContext;
            var currentSort = html.ViewContext.ViewData["CurrentSort"] as string ?? string.Empty;

            bool isAsc = currentSort == sortKey;
            bool isDesc = currentSort == sortKey + "_desc";
            string nextSort = isAsc ? sortKey + "_desc" : sortKey;

            var qb = new QueryBuilder();
            foreach (var kv in ctx.Request.Query)
            {
                if (string.Equals(kv.Key, "page", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(kv.Key, "sortOrder", StringComparison.OrdinalIgnoreCase))
                    continue;
                foreach (var v in kv.Value)
                    qb.Add(kv.Key, v ?? string.Empty);
            }
            qb.Add("sortOrder", nextSort);

            var href = ctx.Request.Path + qb.ToQueryString().ToString();

            string icon = isAsc
                ? " <i class=\"bi bi-caret-up-fill small\"></i>"
                : isDesc
                    ? " <i class=\"bi bi-caret-down-fill small\"></i>"
                    : " <i class=\"bi bi-arrow-down-up small text-muted opacity-50\"></i>";

            var enc = HtmlEncoder.Default;
            var markup =
                $"<a href=\"{enc.Encode(href)}\" class=\"text-decoration-none text-reset d-inline-flex align-items-center gap-1\">" +
                $"{enc.Encode(displayName)}{icon}</a>";

            return new HtmlString(markup);
        }
    }
}
