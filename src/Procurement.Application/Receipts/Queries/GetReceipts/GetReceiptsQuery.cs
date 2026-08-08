using MediatR;
using Procurement.Application.Receipts.DTOs;

namespace Procurement.Application.Receipts.Queries.GetReceipts;

public record GetReceiptsQuery()
    : IRequest<List<ReceiptResponse>>;