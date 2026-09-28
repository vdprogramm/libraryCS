using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
