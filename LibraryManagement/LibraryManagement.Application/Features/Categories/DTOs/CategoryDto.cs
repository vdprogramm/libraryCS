using System;

namespace LibraryManagement.Application.Features.Categories.DTOs;

public record CategoryDto(
    Guid Id,
    string Name,
    string? Description
);
