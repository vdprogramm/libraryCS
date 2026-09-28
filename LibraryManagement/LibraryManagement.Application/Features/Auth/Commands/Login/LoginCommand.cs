using LibraryManagement.Application.Features.Auth.DTOs;
using MediatR;

namespace LibraryManagement.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<AuthResponse>;
