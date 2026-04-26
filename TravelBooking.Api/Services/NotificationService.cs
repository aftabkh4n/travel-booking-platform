using TravelBooking.Core.Interfaces;
using TravelBooking.Core.Models;
using System.Text.Json;

namespace TravelBooking.Api.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public NotificationService(
        ILogger<NotificationService> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient("LogicApps");
    }

    public async Task SendBookingConfirmationAsync(BookingEvent bookingEvent)
    {
        var logicAppsUrl = _configuration["LogicApps:BookingConfirmationUrl"];

        if (string.IsNullOrEmpty(logicAppsUrl))
        {
            _logger.LogWarning("Logic Apps URL not configured. Logging notification locally.");
            _logger.LogInformation(
                "NOTIFICATION: Booking {BookingId} confirmed for {Customer} to {Destination} on {Date}. Total: {Price}",
                bookingEvent.BookingId,
                bookingEvent.CustomerName,
                bookingEvent.Destination,
                bookingEvent.DepartureDate.ToString("yyyy-MM-dd"),
                bookingEvent.TotalPrice);
            return;
        }

        var payload = JsonSerializer.Serialize(bookingEvent);
        var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(logicAppsUrl, content);
        if (response.IsSuccessStatusCode)
            _logger.LogInformation("Logic Apps notification sent for booking {BookingId}", bookingEvent.BookingId);
        else
            _logger.LogWarning("Logic Apps notification failed for booking {BookingId}: {Status}", bookingEvent.BookingId, response.StatusCode);
    }
}