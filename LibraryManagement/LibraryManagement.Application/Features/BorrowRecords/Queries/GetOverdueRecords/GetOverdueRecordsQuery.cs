using System.Collections.Generic;
using LibraryManagement.Application.Features.BorrowRecords.DTOs;
using MediatR;

namespace LibraryManagement.Application.Features.BorrowRecords.Queries.GetOverdueRecords;

public record GetOverdueRecordsQuery()
    : IRequest<List<BorrowRecordDto>>;
