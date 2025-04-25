using FluentValidation;

namespace api.Endpoints.Filters;

public class ValidateRequestFilter<TRequest> : IEndpointFilter
{
    private readonly IValidator<TRequest> validator;

    public ValidateRequestFilter(IValidator<TRequest> validator)
    {
        this.validator = validator;
    }
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.GetArgument<TRequest>(0);
        var validationResult = validator.Validate(request);
        if(!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }
        return await next(context);
    }
}