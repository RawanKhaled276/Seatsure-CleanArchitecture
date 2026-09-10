using Seatsure.Domain; 
namespace Seatsure.Application.DTOs.Reservations;

public record CreateReservationRequest(int Quantity);

public record ReservationDto(
                            Guid Id, 
                            Guid TicketTypeId, 
                            int Quantity,
                            ReservationStatus Status, 
                            DateTime HoldExpiresAtUtc,
                            DateTime CreatedAtUtc,
                            DateTime? ConfirmedAtUtc);
