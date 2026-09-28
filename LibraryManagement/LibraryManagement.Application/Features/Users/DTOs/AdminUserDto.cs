using System;
using System.Collections.Generic;

namespace LibraryManagement.Application.Features.Users.DTOs;

public record AdminUserDto(
    Guid Id,
    string Username,
    string Email,
    string FullName,
    bool IsActive,
    List<string> Roles
);
