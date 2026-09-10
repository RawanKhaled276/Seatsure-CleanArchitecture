using Seatsure.Application.DTOs.TicketTypes;
using Seatsure.Application.Exceptions;
using Seatsure.Application.Interfaces;
using Seatsure.Domain;

namespace Seatsure.Application.Services.Interfaces;

public interface ITicketTypeService
{
    Task<IEnumerable<TicketTypeDto>> ListForEventAsync(Guid eventId);
    Task<TicketTypeDto> AddTicketTypeAsync(Guid callerId, Guid eventId, CreateTicketTypeRequest request);
}