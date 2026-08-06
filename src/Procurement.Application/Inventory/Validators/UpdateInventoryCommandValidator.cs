using FluentValidation;
using Procurement.Application.Inventory.Commands.UpdateInventory;

namespace Procurement.Application.Inventory.Validators;

public class UpdateInventoryCommandValidator
    : AbstractValidator<UpdateInventoryCommand>
{
    public UpdateInventoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.MinimumStock)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.AverageUnitCost)
            .GreaterThanOrEqualTo(0);
    }
}