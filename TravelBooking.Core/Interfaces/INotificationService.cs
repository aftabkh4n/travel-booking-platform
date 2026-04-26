using TravelBooking.Core.Models;

namespace TravelBooking.Core.Interfaces;

public interface INotificationService
{
    Task SendBookingConfirmationAsync(BookingEvent bookingEvent);
}