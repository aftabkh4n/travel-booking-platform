namespace TravelBooking.Core.Models;

public class BookingEvent
{
    public string BookingId { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DepartureDate { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingEventType EventType { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}

public enum BookingEventType
{
    BookingCreated,
    BookingConfirmed,
    BookingCancelled
}