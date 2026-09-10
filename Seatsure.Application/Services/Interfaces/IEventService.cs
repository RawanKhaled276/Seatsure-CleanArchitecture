using Seatsure.Application.DTOs.Event;
using Seatsure.Application.DTOs.Shared;

namespace Seatsure.Application.Services.Interfaces;

public interface IEventService
{
    Task<PagedResult<EventDto>> ListPublishedEventsAsync(int page, int pageSize);
    Task<EventDetailsDto> GetEventByIdAsync(Guid eventId);
    Task<EventDto> CreateEventAsync(Guid callerId, CreateEventRequest request);
    Task<EventDto> PublishEventAsync(Guid callerId, Guid eventId);
}