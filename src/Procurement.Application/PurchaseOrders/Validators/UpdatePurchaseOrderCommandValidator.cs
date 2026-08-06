using FluentValidation;
using Procurement.Application.PurchaseOrders.Commands.UpdatePurchaseOrder;

namespace Procurement.Application.PurchaseOrders.Validators;

public class UpdatePurchaseOrderCommandValidator
    : AbstractValidator<UpdatePurchaseOrderCommand>
{
    public UpdatePurchaseOrderCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();


        RuleFor(x => x.SupplierId)
            .NotEmpty();


        RuleFor(x => x.Lines)
            .NotEmpty();


        RuleForEach(x => x.Lines)
            .SetValidator(new CreatePurchaseOrderLineValidator());
    }
}