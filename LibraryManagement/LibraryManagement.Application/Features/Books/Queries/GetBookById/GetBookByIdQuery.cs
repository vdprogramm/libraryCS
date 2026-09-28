using System;
using LibraryManagement.Application.Features.Books.DTOs;
using MediatR;

namespace LibraryManagement.Application.Features.Books.Queries.GetBookById;

public record GetBookByIdQuery(Guid Id)
    : IRequest<BookDto?>;
