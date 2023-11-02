using FluentValidation;

namespace Test_API_Func.Common.Models.Validator
{
    public class EmployeeValidator : AbstractValidator<Employee>
    {
        public EmployeeValidator()
        {
            RuleFor(v => v.Id)
            .NotEmpty()
            .WithMessage("Employee Id can't be blank");

            RuleFor(v => v.FirstName)
            .NotEmpty()
            .WithMessage("First name can't be blank");

            RuleFor(v => v.LastName)
            .NotEmpty()
            .WithMessage("Last name can't be blank");

            RuleFor(v => v.Country)
            .NotEmpty()
            .WithMessage("Country can't be blank");
        }
    }
}
