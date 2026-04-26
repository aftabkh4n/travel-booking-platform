using MongoDB.Driver;
using TravelBooking.Core.Interfaces;
using TravelBooking.Core.Models;

namespace TravelBooking.Api.Data;

public class BookingRepository : IBookingRepository
{
    private readonly IMongoCollection<Booking> _bookings;
    private readonly ILogger<BookingRepository> _logger;

    public BookingRepository(IMongoClient mongoClient, IConfiguration configuration, ILogger<BookingRepository> logger)
    {
        _logger = logger;
        var database = mongoClient.GetDatabase(configuration["MongoDB:DatabaseName"] ?? "TravelBookingDb");
        _bookings = database.GetCollection<Booking>("bookings");
    }

    public async Task<Booking> CreateAsync(Booking booking)
    {
        await _bookings.InsertOneAsync(booking);
        _logger.LogInformation("Booking {BookingId} created for {Customer}", booking.Id, booking.CustomerName);
        return booking;
    }

    public async Task<Booking?> GetByIdAsync(string id)
    {
        return await _bookings.Find(b => b.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        return await _bookings.Find(_ => true).SortByDescending(b => b.CreatedAt).ToListAsync();
    }

    public async Task<Booking?> UpdateStatusAsync(string id, BookingStatus status)
    {
        var update = Builders<Booking>.Update.Set(b => b.Status, status);
        var result = await _bookings.FindOneAndUpdateAsync(
            b => b.Id == id,
            update,
            new FindOneAndUpdateOptions<Booking> { ReturnDocument = ReturnDocument.After });
        return result;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _bookings.DeleteOneAsync(b => b.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<IEnumerable<Booking>> GetByDestinationAsync(string destination)
    {
        return await _bookings.Find(b => b.Destination.ToLower().Contains(destination.ToLower()))
            .SortByDescending(b => b.CreatedAt)
            .ToListAsync();
    }
}