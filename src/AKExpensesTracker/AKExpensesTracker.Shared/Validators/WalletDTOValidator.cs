using AKExpensesTracker.Shared.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKExpensesTracker.Shared.Validators
{
    public class WalletDtoValidator : AbstractValidator<WalletDTO>
    {
        public WalletDtoValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(50)
                .WithMessage("Name must be less than 100 characters");
            
            RuleFor(p => p.Currency)
                .NotEmpty()
                .WithMessage("Currency is required")
                .Length(3)
                .WithMessage("Currency must be less than 3 characters");

            RuleFor(p => p.Swift)
                .Length(8, 11)
                .When(p => !string.IsNullOrEmpty(p.Swift))
                .WithMessage("SwiftCode must be between 8 and 11 characters");

            RuleFor(p => p.Iban)
                .MaximumLength(34)
                .When(p => !string.IsNullOrEmpty(p.Iban))
                .WithMessage("Iban must be less than 35 characters");
        }
    }
}
