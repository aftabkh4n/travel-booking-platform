using TravelBooking.Core.Models;

namespace TravelBooking.Core.Interfaces;

public interface IBookingRepository
{
    Task<Booking> CreateAsync(Booking booking);
    Task<Booking?> GetByIdAsync(string id);
    Task<IEnumerable<Booking>> GetAllAsync();
    Task<Booking?> UpdateStatusAsync(string id, BookingStatus status);
    Task<bool> DeleteAsync(string id);
    Task<IEnumerable<Booking>> GetByDestinationAsync(string destination);
}