using LibraryManagement.Application.Features.Authors.DTOs;
using MediatR;
using System.Collections.Generic;

namespace LibraryManagement.Application.Features.Authors.Queries.GetAuthors;

public record GetAuthorsQuery()
    : IRequest<List<AuthorDto>>;
