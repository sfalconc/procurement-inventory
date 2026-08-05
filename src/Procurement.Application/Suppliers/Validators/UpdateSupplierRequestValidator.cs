using FluentValidation;
using Procurement.Application.Suppliers.DTOs;

namespace Procurement.Application.Suppliers.Validators;

public class UpdateSupplierRequestValidator 
    : AbstractValidator<UpdateSupplierRequest>
{
    public UpdateSupplierRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.TaxId)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.ContactName)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.ContactName));

        RuleFor(x => x.Phone)
            .MaximumLength(30)
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));
    }
}