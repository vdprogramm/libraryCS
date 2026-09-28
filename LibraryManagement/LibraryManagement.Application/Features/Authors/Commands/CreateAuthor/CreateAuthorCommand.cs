using LibraryManagement.Application.Features.Authors.DTOs;
using MediatR;

namespace LibraryManagement.Application.Features.Authors.Commands.CreateAuthor;

public record CreateAuthorCommand(
    string Name,
    string? Biography
) : IRequest<AuthorDto>;
