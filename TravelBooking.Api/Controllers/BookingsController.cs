using Microsoft.AspNetCore.Mvc;
using TravelBooking.Core.Interfaces;
using TravelBooking.Core.Models;

namespace TravelBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingRepository _repository;
    private readonly INotificationService _notification;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(
        IBookingRepository repository,
        INotificationService notification,
        ILogger<BookingsController> logger)
    {
        _repository = repository;
        _notification = notification;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
    {
        var booking = new Booking
        {
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            Destination = request.Destination,
            DepartureDate = request.DepartureDate,
            ReturnDate = request.ReturnDate,
            Passengers = request.Passengers,
            TotalPrice = request.TotalPrice,
            Notes = request.Notes
        };

        var created = await _repository.CreateAsync(booking);

        var bookingEvent = new BookingEvent
        {
            BookingId = created.Id!,
            CustomerEmail = created.CustomerEmail,
            CustomerName = created.CustomerName,
            Destination = created.Destination,
            DepartureDate = created.DepartureDate,
            TotalPrice = created.TotalPrice,
            EventType = BookingEventType.BookingCreated
        };

        await _notification.SendBookingConfirmationAsync(bookingEvent);

        return CreatedAtAction(nameof(GetBooking), new { id = created.Id }, created);
    }

    [HttpGet]
    public async Task<IActionResult> GetBookings([FromQuery] string? destination = null)
    {
        var bookings = destination != null
            ? await _repository.GetByDestinationAsync(destination)
            : await _repository.GetAllAsync();
        return Ok(bookings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(string id)
    {
        var booking = await _repository.GetByIdAsync(id);
        return booking == null ? NotFound() : Ok(booking);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateStatusRequest request)
    {
        var booking = await _repository.UpdateStatusAsync(id, request.Status);
        if (booking == null) return NotFound();

        if (request.Status == BookingStatus.Confirmed)
        {
            var bookingEvent = new BookingEvent
            {
                BookingId = booking.Id!,
                CustomerEmail = booking.CustomerEmail,
                CustomerName = booking.CustomerName,
                Destination = booking.Destination,
                DepartureDate = booking.DepartureDate,
                TotalPrice = booking.TotalPrice,
                EventType = BookingEventType.BookingConfirmed
            };
            await _notification.SendBookingConfirmationAsync(bookingEvent);
        }

        return Ok(booking);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(string id)
    {
        var deleted = await _repository.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}

public record CreateBookingRequest(
    string CustomerName,
    string CustomerEmail,
    string Destination,
    DateTime DepartureDate,
    DateTime ReturnDate,
    int Passengers,
    decimal TotalPrice,
    string? Notes);

public record UpdateStatusRequest(BookingStatus Status);