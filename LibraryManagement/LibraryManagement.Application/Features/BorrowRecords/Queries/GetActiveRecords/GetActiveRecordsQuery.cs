using System.Collections.Generic;
using LibraryManagement.Application.Features.BorrowRecords.DTOs;
using MediatR;
namespace LibraryManagement.Application.Features.BorrowRecords.Queries.GetActiveRecords;
public record GetActiveRecordsQuery() : IRequest<List<BorrowRecordDto>>;
