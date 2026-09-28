using LibraryManagement.Application.Features.Auth.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public RegisterCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var username = request.Username.Trim();
        var email = request.Email.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(email) ||
            !System.Net.Mail.MailAddress.TryCreate(
                email, out var parsedEmail) ||
            parsedEmail.Address != email ||
            request.Password == null ||
            request.Password.Length < 8 ||
            request.Password.Length > 72)
        {
            throw new ArgumentException(
                "Invalid registration information.");
        }

        var exists = await _context.Users.AnyAsync(
            x => x.Username == username || x.Email == email,
            cancellationToken);

        if (exists)
            throw new InvalidOperationException(
                "Username or email already exists.");

        var readerRole = await _context.Roles
            .SingleOrDefaultAsync(
                x => x.Name == "READER",
                cancellationToken);

        if (readerRole == null)
            throw new InvalidOperationException(
                "READER role has not been initialized.");

        var user = new User
        {
            Username = username,
            Email = email,
            FullName = request.FullName.Trim(),
            PasswordHash = _passwordHasher.Hash(request.Password),
            IsActive = true
        };

        user.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = readerRole.Id,
            Role = readerRole
        });

        _context.Users.Add(user);

        await _context.SaveChangesAsync(cancellationToken);

        var roles = new List<string> { readerRole.Name };

        var dto = new UserDto(
            user.Id,
            user.Username,
            user.Email,
            user.FullName,
            roles
        );

        var token = _jwtService.GenerateToken(user);

        return new AuthResponse(token, "Bearer", dto);
    }
}
