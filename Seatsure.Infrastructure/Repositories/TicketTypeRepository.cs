using Microsoft.EntityFrameworkCore;
using Seatsure.Application.Interfaces;
using Seatsure.Domain;
using Seatsure.Infrastructure.Data;

namespace Seatsure.Infrastructure.Repositories;

public class TicketTypeRepository : ITicketTypeRepository
{
    private readonly AppDbContext _context;

    public TicketTypeRepository(AppDbContext context) => _context = context;

    public async Task<TicketType?> GetByIdAsync(Guid id) =>
        await _context.TicketTypes.FindAsync(id);

    public async Task<IEnumerable<TicketType>> GetByEventIdAsync(Guid eventId) =>
        await _context.TicketTypes
            .Where(t => t.EventId == eventId)
            .ToListAsync();

    public async Task AddAsync(TicketType ticketType) =>
        await _context.TicketTypes.AddAsync(ticketType);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}