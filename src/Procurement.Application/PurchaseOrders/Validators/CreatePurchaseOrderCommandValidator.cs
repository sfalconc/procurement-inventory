using FluentValidation;
using Procurement.Application.PurchaseOrders.Commands.CreatePurchaseOrder;
using Procurement.Application.PurchaseOrders.DTOs;

namespace Procurement.Application.PurchaseOrders.Validators;

public class CreatePurchaseOrderCommandValidator 
    : AbstractValidator<CreatePurchaseOrderCommand>
{
    public CreatePurchaseOrderCommandValidator()
    {
        RuleFor(x => x.SupplierId)
            .NotEmpty()
            .WithMessage("Supplier is required.");


        RuleFor(x => x.Lines)
            .NotEmpty()
            .WithMessage("Purchase order must contain at least one line.");


        RuleForEach(x => x.Lines)
            .SetValidator(new CreatePurchaseOrderLineValidator());
    }
}


public class CreatePurchaseOrderLineValidator
    : AbstractValidator<CreatePurchaseOrderLineRequest>
{
    public CreatePurchaseOrderLineValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product is required.");


        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");


        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Unit price must be greater than zero.");
    }
}