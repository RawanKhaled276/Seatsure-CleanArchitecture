# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build

# Run API (http://localhost:5149, Swagger at /swagger)
dotnet run --project Seatsure

# EF Core migrations
dotnet ef migrations add <Name> --project Seatsure.DAL --startup-project Seatsure
dotnet ef database update --project Seatsure.DAL --startup-project Seatsure
```

No test project exists yet — xUnit is planned but not scaffolded.

## Architecture

Three-project layered solution:

- **Seatsure.Domain** — pure C# entities (`User`, `Event`, `TicketType`, `Reservation`) and enums (`UserRole`, `EventStatus`, `ReservationStatus`). No dependencies.
- **Seatsure.DAL** — EF Core `AppDbContext` with fluent configuration; references Domain. SQL Server via Windows auth (`Server=.;Database=Seatsure;Trusted_Connection=True`).
- **Seatsure** — ASP.NET Core 8 Web API; references both DAL and Domain. Currently only the scaffold controller exists; all business controllers are unimplemented.

Dependency direction: `Seatsure` → `Seatsure.DAL` → `Seatsure.Domain`

## Concurrency Design

`TicketType` carries a `RowVersion` (byte[]) EF Core concurrency token. The reservation flow must use this for optimistic locking to prevent overbooking — a concurrent write conflict should return HTTP 409. This is the core invariant of the system; do not bypass it.

## Implementation Status

**Done:** domain entities, `AppDbContext` with relationships, initial migration, DI wiring in `Program.cs`, Swagger.

**Not yet built:** controllers (Auth, Events, TicketTypes, Reservations), service layer (`IReservationService`), JWT Bearer auth, SignalR hub (`/hubs/events`), background service (hold expiry every 30 s), DTOs, validation.

## Spec Reference

`README.md` at the repo root is the frozen API contract and technical blueprint. It defines all endpoints, request/response shapes, the concurrency sequence diagram, and the hold-expiry background service behavior. Treat it as authoritative when implementing features.
