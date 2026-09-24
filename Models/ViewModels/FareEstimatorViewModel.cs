using Microsoft.AspNetCore.Mvc.Rendering;

namespace CabManagementSystem.Models.ViewModels
{
    /// <summary>Public fare estimator shown on the landing page (no login required).</summary>
    public class FareEstimatorViewModel
    {
        public SelectList? AvailableRoutes { get; set; }

        public DateTime DefaultTripDate { get; set; } = DateTime.Now.AddHours(1);

        public bool HasRoutes => AvailableRoutes != null && AvailableRoutes.Any();
    }
}
