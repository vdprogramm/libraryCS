using System;
using LibraryManagement.Application.Features.Penalties.DTOs;
using MediatR;

namespace LibraryManagement.Application.Features.Penalties.Commands.PayPenalty;

public record PayPenaltyCommand(Guid PenaltyId)
    : IRequest<PenaltyDto>;
