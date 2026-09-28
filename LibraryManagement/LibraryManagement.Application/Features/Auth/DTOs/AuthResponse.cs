namespace LibraryManagement.Application.Features.Auth.DTOs;

public record AuthResponse(
    string AccessToken,
    string TokenType,
    UserDto User
);
