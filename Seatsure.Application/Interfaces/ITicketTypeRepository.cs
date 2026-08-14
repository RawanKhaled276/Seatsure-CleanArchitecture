using Seatsure.Domain;

namespace Seatsure.Application.Interfaces;

public interface ITicketTypeRepository
{
    Task<TicketType?> GetByIdAsync(Guid id);
    Task<IEnumerable<TicketType>> GetByEventIdAsync(Guid eventId);
    Task AddAsync(TicketType ticketType);
    Task SaveChangesAsync();
}