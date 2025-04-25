using FluentValidation;

namespace api.Contracts.Request;

//note: photo upload is done by photo endpoint
public record CompanyRequest(
    string Name,
    string Description,
    string Location);

public class CompanyDataValidator : AbstractValidator<CompanyRequest>
{
    public CompanyDataValidator()
    {
        RuleFor(r => r.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(6, 10);

        RuleFor(r => r.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(r => r.Location)
            .NotEmpty();
    }
}