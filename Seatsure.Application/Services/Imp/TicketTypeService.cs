using Seatsure.Application.DTOs.TicketTypes;
using Seatsure.Application.Exceptions;
using Seatsure.Application.Interfaces;
using Seatsure.Application.Services.Interfaces;
using Seatsure.Domain;

namespace Seatsure.Application.Services.Implementations;

public sealed class TicketTypeService : ITicketTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITicketTypeRepository _ticketTypeRepository;
    private readonly IEventRepository _eventRepository;

    public TicketTypeService(IUnitOfWork unitOfWork, ITicketTypeRepository ticketTypeRepository, IEventRepository eventRepository)
    {
        _unitOfWork = unitOfWork;
        _ticketTypeRepository = ticketTypeRepository;
        _eventRepository = eventRepository;
    }

    public async Task<TicketTypeDto> AddTicketTypeAsync(Guid callerId, Guid eventId, CreateTicketTypeRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
            throw new ValidationException("Ticket type name is required.");
        if (request.Price <= 0)
            throw new ValidationException("Price must be greater than 0.");
        if (request.TotalQuantity < 1)
            throw new ValidationException("Total quantity must be at least 1.");

        var ev = await _eventRepository.GetByIdAsync(eventId)
            ?? throw new NotFoundException($"Event with id {eventId} not found.");

        if (ev.OrganizerId != callerId)
            throw new ForbiddenException("You are not authorized to add ticket types to this event.");

        var ticket = new TicketType
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            Name = request.Name,
            Price = request.Price,
            TotalQuantity = request.TotalQuantity,
            AvailableQuantity = request.TotalQuantity,
        };

        await _ticketTypeRepository.AddAsync(ticket);
        await _unitOfWork.SaveChangesAsync();

        return new TicketTypeDto(ticket.Id, ticket.EventId, ticket.Name, ticket.Price, ticket.TotalQuantity, ticket.AvailableQuantity);
    }

    public async Task<IEnumerable<TicketTypeDto>> ListForEventAsync(Guid eventId)
    {
        var ev = await _eventRepository.GetByIdAsync(eventId)
            ?? throw new NotFoundException($"Event with id {eventId} not found.");

        var tickets = await _ticketTypeRepository.GetByEventIdAsync(eventId);

        return tickets.Select(t => new TicketTypeDto(t.Id, t.EventId, t.Name, t.Price, t.TotalQuantity, t.AvailableQuantity));
    }
}