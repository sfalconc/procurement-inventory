using FluentValidation;
using Procurement.Application.PurchaseOrders.Commands.ReceivePurchaseOrder;
using Procurement.Application.PurchaseOrders.DTOs;

namespace Procurement.Application.PurchaseOrders.Validators;

public class ReceivePurchaseOrderCommandValidator
    : AbstractValidator<ReceivePurchaseOrderCommand>
{
    public ReceivePurchaseOrderCommandValidator()
    {
        RuleFor(x => x.PurchaseOrderId)
            .NotEmpty()
            .WithMessage("Purchase order is required.");


        RuleFor(x => x.Lines)
            .NotEmpty()
            .WithMessage("Receipt must contain at least one line.");


        RuleForEach(x => x.Lines)
            .SetValidator(new ReceivePurchaseOrderLineValidator());
    }
}


public class ReceivePurchaseOrderLineValidator
    : AbstractValidator<ReceivePurchaseOrderLineRequest>
{
    public ReceivePurchaseOrderLineValidator()
    {
        RuleFor(x => x.PurchaseOrderLineId)
            .NotEmpty()
            .WithMessage("Purchase order line is required.");


        RuleFor(x => x.QuantityReceived)
            .GreaterThan(0)
            .WithMessage("Received quantity must be greater than zero.");
    }
}