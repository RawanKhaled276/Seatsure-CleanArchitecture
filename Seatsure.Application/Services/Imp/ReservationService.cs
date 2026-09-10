using Microsoft.EntityFrameworkCore;
using Seatsure.Application.DTOs.Reservations;
using Seatsure.Application.Exceptions;
using Seatsure.Application.Interfaces;
using Seatsure.Application.Services.Interfaces;
using Seatsure.Domain;

namespace Seatsure.Application.Services;

public class ReservationService : IReservationService
{
    private readonly ITicketTypeRepository _ticketTypeRepository;
    private readonly IReservationRepository _reservationRepository;

    public ReservationService(
        ITicketTypeRepository ticketTypeRepository,
        IReservationRepository reservationRepository)
    {
        _ticketTypeRepository = ticketTypeRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<ReservationDto> CreateHoldAsync(
        Guid callerId,
        Guid ticketTypeId,
        CreateReservationRequest request)
    {
        var ticketType = await _ticketTypeRepository.GetByIdAsync(ticketTypeId);

        if (ticketType is null)
            throw new NotFoundException("Ticket type not found.");

        if (request.Quantity <= 0)
            throw new ValidationException("Quantity must be greater than zero.");

        if (request.Quantity > ticketType.AvailableQuantity)
            throw new ConflictException("Not enough tickets available.");

        ticketType.AvailableQuantity -= request.Quantity;

        var reservation = new Reservation
        {
            TicketTypeId = ticketTypeId,
            UserId = callerId,
            Quantity = request.Quantity,
            Status = ReservationStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow,
            HoldExpiresAtUtc = DateTime.UtcNow.AddMinutes(10)
        };

        await _reservationRepository.AddAsync(reservation);

        try
        {
            await _ticketTypeRepository.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException(
                "Someone booked these tickets first. Please retry.");
        }

        return MapToDto(reservation);
    }

    public async Task<ReservationDto> ConfirmAsync(
        Guid callerId,
        Guid reservationId)
    {
        var reservation =
            await _reservationRepository.GetByIdAsync(reservationId);

        if (reservation is null)
            throw new NotFoundException("Reservation not found.");

        if (reservation.UserId != callerId)
            throw new ForbiddenException(
                "You are not allowed to confirm this reservation.");

        if (reservation.Status != ReservationStatus.Pending)
            throw new ConflictException(
                "Only pending reservations can be confirmed.");

        if (reservation.HoldExpiresAtUtc <= DateTime.UtcNow)
        {
            reservation.Status = ReservationStatus.Expired;

            await _reservationRepository.SaveChangesAsync();

            throw new ConflictException(
                "The reservation hold has expired.");
        }

        reservation.Status = ReservationStatus.Confirmed;
        reservation.ConfirmedAtUtc = DateTime.UtcNow;

        await _reservationRepository.SaveChangesAsync();

        return MapToDto(reservation);
    }

    public async Task<ReservationDto> CancelAsync(
        Guid callerId,
        Guid reservationId)
    {
        var reservation =
            await _reservationRepository.GetByIdAsync(reservationId);

        if (reservation is null)
            throw new NotFoundException("Reservation not found.");

        if (reservation.UserId != callerId)
            throw new ForbiddenException(
                "You are not allowed to cancel this reservation.");

        if (reservation.Status != ReservationStatus.Pending)
            throw new ConflictException(
                "Only pending reservations can be cancelled.");

        reservation.Status = ReservationStatus.Cancelled;

        reservation.TicketType.AvailableQuantity += reservation.Quantity;

        await _reservationRepository.SaveChangesAsync();

        return MapToDto(reservation);
    }

    public async Task<IEnumerable<ReservationDto>> MyReservationsAsync(
        Guid callerId)
    {
        var reservations =
            await _reservationRepository.GetByUserIdAsync(callerId);

        return reservations.Select(MapToDto);
    }

    private static ReservationDto MapToDto(Reservation reservation)
    {
        return new ReservationDto(
            reservation.Id,
            reservation.TicketTypeId,
            reservation.Quantity,
            reservation.Status,
            reservation.HoldExpiresAtUtc,
            reservation.CreatedAtUtc,
            reservation.ConfirmedAtUtc);
    }
}