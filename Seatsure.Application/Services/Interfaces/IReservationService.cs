using Seatsure.Application.DTOs.Reservations;

namespace Seatsure.Application.Services.Interfaces;

public interface IReservationService
{
    Task<ReservationDto> CreateHoldAsync(Guid callerId, Guid ticketTypeId, CreateReservationRequest request);
    Task<ReservationDto> ConfirmAsync(Guid callerId, Guid reservationId);
    Task<ReservationDto> CancelAsync(Guid callerId, Guid reservationId);
    Task<IEnumerable<ReservationDto>> MyReservationsAsync(Guid callerId);
}