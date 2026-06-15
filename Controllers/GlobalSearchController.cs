using CabManagementSystem.Data;
using CabManagementSystem.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CabManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,FinanceManager,HRManager,TravelManager")]
    public class GlobalSearchController : Controller
    {
        private const int PerTypeLimit = 5;
        private readonly ApplicationDbContext _context;

        public GlobalSearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            q = (q ?? string.Empty).Trim();
            if (q.Length < 2)
            {
                return Json(new { results = Array.Empty<object>() });
            }

            var results = new List<SearchResult>();

            var trips = await _context.Trips
                .Where(t => EF.Functions.Like(t.CustomerName, $"%{q}%")
                            || EF.Functions.Like(t.CustomerEmail, $"%{q}%")
                            || EF.Functions.Like(t.CustomerPhone, $"%{q}%"))
                .OrderByDescending(t => t.BookingDate)
                .Take(PerTypeLimit)
                .Select(t => new { t.Id, t.CustomerName, t.Status, t.TripDate })
                .ToListAsync();
            foreach (var t in trips)
            {
                results.Add(new SearchResult
                {
                    Type = "Trip",
                    Icon = "bi-geo-alt",
                    Label = t.CustomerName,
                    Sublabel = $"Trip #{t.Id} · {t.Status} · {t.TripDate:dd MMM yyyy}",
                    Url = Url.Action("Details", "Trip", new { area = "Travel", id = t.Id }) ?? "#"
                });
            }

            var customers = await _context.Customers
                .Where(c => EF.Functions.Like(c.Name, $"%{q}%")
                            || EF.Functions.Like(c.Email, $"%{q}%")
                            || EF.Functions.Like(c.Phone, $"%{q}%"))
                .OrderBy(c => c.Name)
                .Take(PerTypeLimit)
                .Select(c => new { c.Id, c.Name, c.Email })
                .ToListAsync();
            foreach (var c in customers)
            {
                results.Add(new SearchResult
                {
                    Type = "Customer",
                    Icon = "bi-person",
                    Label = c.Name,
                    Sublabel = c.Email,
                    Url = Url.Action("Details", "Customer", new { area = "Admin", id = c.Id }) ?? "#"
                });
            }

            var vehicles = await _context.Vehicles
                .Where(v => EF.Functions.Like(v.RegistrationNumber, $"%{q}%")
                            || EF.Functions.Like(v.Make, $"%{q}%")
                            || EF.Functions.Like(v.Model, $"%{q}%"))
                .OrderBy(v => v.RegistrationNumber)
                .Take(PerTypeLimit)
                .Select(v => new { v.Id, v.RegistrationNumber, v.Make, v.Model, v.Status })
                .ToListAsync();
            foreach (var v in vehicles)
            {
                results.Add(new SearchResult
                {
                    Type = "Vehicle",
                    Icon = "bi-truck",
                    Label = $"{v.Make} {v.Model}",
                    Sublabel = $"{v.RegistrationNumber} · {v.Status}",
                    Url = Url.Action("Details", "Vehicle", new { area = "Admin", id = v.Id }) ?? "#"
                });
            }

            var routes = await _context.Routes
                .Where(r => EF.Functions.Like(r.Origin, $"%{q}%")
                            || EF.Functions.Like(r.Destination, $"%{q}%"))
                .OrderBy(r => r.Origin)
                .Take(PerTypeLimit)
                .Select(r => new { r.Id, r.Origin, r.Destination })
                .ToListAsync();
            foreach (var r in routes)
            {
                results.Add(new SearchResult
                {
                    Type = "Route",
                    Icon = "bi-signpost-2",
                    Label = $"{r.Origin} → {r.Destination}",
                    Sublabel = "Route",
                    Url = Url.Action("Details", "Route", new { area = "Admin", id = r.Id }) ?? "#"
                });
            }

            return Json(new { results });
        }

        private sealed class SearchResult
        {
            public string Type { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
            public string Label { get; set; } = string.Empty;
            public string? Sublabel { get; set; }
            public string Url { get; set; } = "#";
        }
    }
}
