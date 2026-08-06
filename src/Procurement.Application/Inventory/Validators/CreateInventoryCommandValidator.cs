using FluentValidation;
using Procurement.Application.Inventory.Commands.CreateInventory;

namespace Procurement.Application.Inventory.Validators;

public class CreateInventoryCommandValidator
    : AbstractValidator<CreateInventoryCommand>
{
    public CreateInventoryCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.MinimumStock)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.AverageUnitCost)
            .GreaterThanOrEqualTo(0);
    }
}