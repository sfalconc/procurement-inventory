using MediatR;
using Procurement.Application.Receipts.DTOs;

namespace Procurement.Application.Receipts.Queries.GetReceiptById;

public record GetReceiptByIdQuery(Guid Id)
    : IRequest<ReceiptResponse?>;