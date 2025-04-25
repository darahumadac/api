using System.Text.RegularExpressions;
using api.Models;
using api.Services;
using FluentValidation;

namespace api.Contracts.Request;

 //note: photo upload is handled by photo api
public record EmployeeRequest(
    string Name,
    string Email,
    string Phone,
    int Gender,
    string? CompanyId);
public class EmployeeDataValidator : AbstractValidator<EmployeeRequest>
{
    public EmployeeDataValidator(IRepository<Company> companyService)
    {
        RuleFor(r => r.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(6, 10);
        
        RuleFor(r => r.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress();

        RuleFor(r => r.Phone)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(number => Regex.IsMatch(number, @"^[8|9][0-9]{7}$"))
                .WithMessage("'Phone Number' is not a valid SG phone number");

        RuleFor(r => r.Gender)
            .InclusiveBetween(0,1);

        When(r => r.CompanyId != null, () => {
            RuleFor(r => r.CompanyId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("'CompanyId' must not be empty. Remove 'CompanyId' from the request to remove assigned company")
                .Must(companyId => Guid.TryParse(companyId, out Guid parsed) == true)
                    .WithMessage("'CompanyId' format is invalid")
                .Must(companyId => companyService.GetByIdAsync(companyId!).GetAwaiter().GetResult() != null)
                    .WithMessage("'CompanyId' must be an existing company id");
        });        
    }
}