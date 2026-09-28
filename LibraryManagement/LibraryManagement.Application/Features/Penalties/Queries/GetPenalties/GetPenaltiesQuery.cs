using System.Collections.Generic;
using LibraryManagement.Application.Features.Penalties.DTOs;
using MediatR;

namespace LibraryManagement.Application.Features.Penalties.Queries.GetPenalties;

public record GetPenaltiesQuery(
    string? PaymentStatus
) : IRequest<List<PenaltyDto>>;
