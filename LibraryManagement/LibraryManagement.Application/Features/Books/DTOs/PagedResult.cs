using System.Collections.Generic;

namespace LibraryManagement.Application.Features.Books.DTOs;

public record PagedResult<T>(
    List<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages
);
