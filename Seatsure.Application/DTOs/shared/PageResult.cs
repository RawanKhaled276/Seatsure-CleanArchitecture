namespace Seatsure.Application.DTOs.Shared;
public record PagedResult<T>(IEnumerable<T> Items, int Page, int PageSize, int TotalCount);