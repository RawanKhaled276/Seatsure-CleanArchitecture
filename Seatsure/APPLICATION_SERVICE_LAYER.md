# SeatSure Application Service Layer

## Overview

The Application layer contains the business logic of SeatSure.
It coordinates repositories, validates business rules, and exposes DTOs
instead of exposing Domain entities directly.

## Architecture

SeatSure follows Clean Architecture:

- Domain contains entities and business concepts.
- Application contains DTOs, exceptions, service interfaces, and business logic.
- Infrastructure contains EF Core, repositories, Unit of Work, and security implementations.
- API contains controllers and HTTP-related concerns.

Dependencies point inward.

## DTOs

DTOs are used at the service boundary so Domain entities are not exposed
directly to API consumers.

Server-controlled values such as IDs, timestamps, status, organizer IDs,
and available inventory are not accepted from clients.

## Exceptions

Application-specific exceptions represent expected failures:

- ValidationException → 400 Bad Request
- NotFoundException → 404 Not Found
- ConflictException → 409 Conflict
- ForbiddenException → 403 Forbidden
- UnauthorizedException → 401 Unauthorized

## Unit of Work

Repositories stage changes in the shared scoped DbContext.
IUnitOfWork provides the commit mechanism through SaveChangesAsync.

Write services use the Unit of Work to commit related changes together.

## Reservation Concurrency

Ticket inventory uses optimistic concurrency through RowVersion.

CreateHold:

1. Load the ticket type as a tracked entity.
2. Check available quantity.
3. Decrease AvailableQuantity.
4. Create the reservation.
5. Commit the changes.
6. Translate a concurrency conflict into a ConflictException.

This prevents concurrent requests from successfully selling the same
inventory.

## Reservation Lifecycle

Reservation states include:

- Pending
- Confirmed
- Expired
- Cancelled

Only valid state transitions are allowed. Invalid transitions result in
ConflictException.

## Ownership and Authorization

Ownership checks are performed inside application services.

For example, an organizer can modify or publish only events they own,
and a user can manage only their own reservations.

## Security

The Application layer uses security abstractions such as IJwtService
instead of depending directly on JWT implementation details.

The JWT implementation is provided through dependency injection.

## Dependency Injection

Application services, repositories, Unit of Work, and security services
are registered using scoped lifetime.

## Layering Rules

The Application layer should not depend directly on:

- Entity Framework Core
- ASP.NET Core HTTP types
- Controllers
- DbContext

Services communicate through interfaces and DTOs.

## Definition of Done

- DTOs implemented
- Typed application exceptions implemented
- Unit of Work implemented
- Security service implemented
- Auth, Event, TicketType, and Reservation services implemented
- Dependency Injection configured
- Application service layer builds successfully