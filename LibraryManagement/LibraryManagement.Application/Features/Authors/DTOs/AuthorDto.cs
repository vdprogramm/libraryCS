using System;

namespace LibraryManagement.Application.Features.Authors.DTOs;

public record AuthorDto(
    Guid Id,
    string Name,
    string? Biography
);
