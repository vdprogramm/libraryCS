using LibraryManagement.Application.Features.Authors.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Authors.Commands.CreateAuthor;

public class CreateAuthorCommandHandler
    : IRequestHandler<CreateAuthorCommand, AuthorDto>
{
    private readonly IApplicationDbContext _context;

    public CreateAuthorCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AuthorDto> Handle(
        CreateAuthorCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException(
                "Author name is required.");

        var author = new Author
        {
            Name = request.Name.Trim(),
            Biography = request.Biography?.Trim()
        };

        _context.Authors.Add(author);

        await _context.SaveChangesAsync(cancellationToken);

        return new AuthorDto(
            author.Id,
            author.Name,
            author.Biography
        );
    }
}
