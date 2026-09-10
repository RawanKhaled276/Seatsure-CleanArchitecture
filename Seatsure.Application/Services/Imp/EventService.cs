using Seatsure.Application.DTOs.Event;
using Seatsure.Application.DTOs.Shared;
using Seatsure.Application.DTOs.TicketTypes;
using Seatsure.Application.Exceptions;
using Seatsure.Application.Interfaces;
using Seatsure.Application.Services.Interfaces;
using Seatsure.Domain;

namespace Seatsure.Application.Services.Implementations;

public sealed class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EventService(IEventRepository eventRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<EventDto>> ListPublishedEventsAsync(int page, int pageSize)
    {
        if (page < 1) throw new ValidationException("Page must be at least 1");
        if (pageSize < 1) throw new ValidationException("PageSize must be at least 1");

        var (events, totalCount) = await _eventRepository.GetPublishedAsync(page, pageSize);

        var items = events.Select(e => new EventDto(
            e.Id, e.OrganizerId, e.Title, e.Description, e.VenueName, e.StartsAtUtc, e.Status));

        return new PagedResult<EventDto>(items, page, pageSize, totalCount);
    }

    public async Task<EventDetailsDto> GetEventByIdAsync(Guid eventId)
    {
        var ev = await _eventRepository.GetByIdAsync(eventId)
            ?? throw new NotFoundException("Event not found");

        var tickets = ev.TicketTypes.Select(t => new TicketTypeDto(
            t.Id, t.EventId, t.Name, t.Price, t.TotalQuantity, t.AvailableQuantity));

        return new EventDetailsDto(
            ev.Id, ev.OrganizerId, ev.Title, ev.Description, ev.VenueName, ev.StartsAtUtc, ev.Status, tickets);
    }

    public async Task<EventDto> CreateEventAsync(Guid callerId, CreateEventRequest request)
    {
        if (string.IsNullOrEmpty(request.Title))
            throw new ValidationException("Title is required");
        if (string.IsNullOrEmpty(request.VenueName))
            throw new ValidationException("VenueName is required");
        if (request.StartsAtUtc <= DateTime.UtcNow)
            throw new ValidationException("Start date must be in the future");

        var organizer = await _userRepository.GetByIdAsync(callerId)
            ?? throw new NotFoundException("User not found");

        if (organizer.Role != UserRole.Organizer)
            throw new ForbiddenException("Only organizers can create events");

        var ev = new Event
        {
            Id = Guid.NewGuid(),
            OrganizerId = callerId,
            Title = request.Title,
            Description = request.Description,
            VenueName = request.VenueName,
            StartsAtUtc = request.StartsAtUtc,
            Status = EventStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _eventRepository.AddAsync(ev);
        await _unitOfWork.SaveChangesAsync();

        return new EventDto(ev.Id, ev.OrganizerId, ev.Title, ev.Description, ev.VenueName, ev.StartsAtUtc, ev.Status);
    }

    public async Task<EventDto> PublishEventAsync(Guid callerId, Guid eventId)
    {
        var ev = await _eventRepository.GetByIdAsync(eventId)
            ?? throw new NotFoundException("Event not found");

        if (ev.OrganizerId != callerId)
            throw new ForbiddenException("You do not own this event");

        ev.Status = EventStatus.Published;
        await _unitOfWork.SaveChangesAsync();

        return new EventDto(ev.Id, ev.OrganizerId, ev.Title, ev.Description, ev.VenueName, ev.StartsAtUtc, ev.Status);
    }
}