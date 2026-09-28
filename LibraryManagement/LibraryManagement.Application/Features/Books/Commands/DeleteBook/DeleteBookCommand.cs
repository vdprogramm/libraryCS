using System;
using MediatR;

namespace LibraryManagement.Application.Features.Books.Commands.DeleteBook;

public record DeleteBookCommand(Guid Id)
    : IRequest<bool>;
