using Seatsure.Domain;
using Seatsure.Application.DTOs.TicketTypes;
namespace Seatsure.Application.DTOs.Event;

public record EventDto (Guid Id, Guid OrganizerId, string Title, string Description, string Venue, DateTime StartsAtUtc, EventStatus Status);
public record CreateEventRequest(string Title, string Description, string VenueName, DateTime StartsAtUtc);
public record EventDetailsDto(
    Guid Id,
    Guid OrganizerId,
    string Title,
    string Description,
    string VenueName,
    DateTime StartsAtUtc,
    EventStatus Status,
    IEnumerable<TicketTypeDto> Tickets);