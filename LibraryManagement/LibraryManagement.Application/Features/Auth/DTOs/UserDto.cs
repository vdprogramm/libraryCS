using System;
using System.Collections.Generic;

namespace LibraryManagement.Application.Features.Auth.DTOs;

public record UserDto(
    Guid Id,
    string Username,
    string Email,
    string FullName,
    List<string> Roles
);
