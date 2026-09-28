using LibraryManagement.Application.Features.Auth.DTOs;
using MediatR;

namespace LibraryManagement.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Username,
    string Email,
    string Password,
    string FullName
) : IRequest<AuthResponse>;
