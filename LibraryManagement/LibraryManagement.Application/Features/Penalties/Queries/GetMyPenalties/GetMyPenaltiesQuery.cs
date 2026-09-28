using System;
using System.Collections.Generic;
using LibraryManagement.Application.Features.Penalties.DTOs;
using MediatR;

namespace LibraryManagement.Application.Features.Penalties.Queries.GetMyPenalties;

public record GetMyPenaltiesQuery(Guid ReaderId)
    : IRequest<List<PenaltyDto>>;
