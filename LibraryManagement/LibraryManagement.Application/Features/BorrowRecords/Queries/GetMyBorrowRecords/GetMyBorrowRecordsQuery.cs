using System;
using System.Collections.Generic;
using LibraryManagement.Application.Features.BorrowRecords.DTOs;
using MediatR;

namespace LibraryManagement.Application.Features.BorrowRecords.Queries.GetMyBorrowRecords;

public record GetMyBorrowRecordsQuery(
    Guid ReaderId
) : IRequest<List<BorrowRecordDto>>;
