using LibraryManagement.Application.Features.Auth.DTOs;
using LibraryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email?.Trim().ToLowerInvariant();

        if (string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(request.Password))
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        var user = await _context.Users
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(
                x => x.Email == email,
                cancellationToken);

        if (user == null ||
            !user.IsActive ||
            !_passwordHasher.Verify(
                request.Password,
                user.PasswordHash))
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        var roles = user.UserRoles
            .Select(x => x.Role.Name)
            .ToList();

        var dto = new UserDto(
            user.Id,
            user.Username,
            user.Email,
            user.FullName,
            roles
        );

        return new AuthResponse(
            _jwtService.GenerateToken(user),
            "Bearer",
            dto
        );
    }
}
