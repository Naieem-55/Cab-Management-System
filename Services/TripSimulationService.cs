using CabManagementSystem.Hubs;
using CabManagementSystem.Models.Enums;
using Microsoft.AspNetCore.SignalR;

namespace CabManagementSystem.Services
{
    public class TripSimulationService : ITripSimulationService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<TripTrackingHub> _hubContext;
        private readonly ILogger<TripSimulationService> _logger;

        public TripSimulationService(
            IServiceScopeFactory scopeFactory,
            IHubContext<TripTrackingHub> hubContext,
            ILogger<TripSimulationService> logger)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            _logger = logger;
        }

        public void StartSimulation(int tripId)
        {
            _ = RunSimulationAsync(tripId);
        }

        private async Task RunSimulationAsync(int tripId)
        {
            var steps = new[]
            {
                (Delay: 5000, Status: TripStatus.Confirmed, Text: "Confirmed"),
                (Delay: 8000, Status: TripStatus.InProgress, Text: "In Progress"),
                (Delay: 12000, Status: TripStatus.Completed, Text: "Completed")
            };

            foreach (var step in steps)
            {
                await Task.Delay(step.Delay);

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var tripService = scope.ServiceProvider.GetRequiredService<ITripService>();

                    // Verify trip still exists and hasn't been cancelled
                    var trip = await tripService.GetTripByIdAsync(tripId);
                    if (trip == null || trip.Status == TripStatus.Cancelled)
                    {
                        _logger.LogInformation("Simulation stopped for trip {TripId}: trip is null or cancelled", tripId);
                        return;
                    }

                    await tripService.UpdateTripStatusAsync(tripId, step.Status);

                    // Push real-time update via SignalR
                    await _hubContext.Clients.Group($"trip-{tripId}")
                        .SendAsync("TripStatusUpdated", tripId, (int)step.Status, step.Text);

                    _logger.LogInformation("Simulation: Trip {TripId} status updated to {Status}", tripId, step.Text);

                    // Create in-app notification
                    try
                    {
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                        var tripDetails = await tripService.GetTripWithDetailsAsync(tripId);
                        if (tripDetails?.CustomerEmail != null)
                        {
                            var userManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<Models.ApplicationUser>>();
                            var appUser = await userManager.FindByEmailAsync(tripDetails.CustomerEmail);
                            if (appUser != null)
                            {
                                await notificationService.CreateNotificationAsync(
                                    appUser.Id,
                                    $"Trip #{tripId} {step.Text}",
                                    $"Your trip status has been updated to {step.Text}.",
                                    $"/CustomerPortal/Trip/Details/{tripId}");
                            }
                        }
                    }
                    catch (Exception notifEx)
                    {
                        _logger.LogWarning(notifEx, "Failed to create notification during simulation for trip {TripId}", tripId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Simulation error for trip {TripId} at status {Status}", tripId, step.Text);
                    return;
                }
            }

            _logger.LogInformation("Simulation completed for trip {TripId}", tripId);
        }
    }
}
