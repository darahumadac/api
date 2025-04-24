using api.Contracts.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace api.Endpoints;

public static partial class EmployeesEndpoints
{
    public static async Task<Ok<IEnumerable<EmployeeResponse>>> HandleGetEmployeesAsync([FromQuery] string name)
    {
        var result = new List<EmployeeResponse>()
            .Where(r => string.IsNullOrEmpty(name) || r.Name == name);
        return await Task.FromResult(TypedResults.Ok(result));
    }
    public static async Task<Ok<string>> HandleGetEmployeeAsync()
    {
        return await Task.FromResult(TypedResults.Ok(nameof(HandleGetEmployeeAsync)));
    }

    public static async Task<IResult> HandleUpdateEmployeeAsync()
    {
        return await Task.FromResult(Results.Ok(nameof(HandleUpdateEmployeeAsync)));
    }

    public static async Task<IResult> HandleAddEmployeeAsync()
    {
        return await Task.FromResult(Results.Ok(nameof(HandleAddEmployeeAsync)));
    }

    public static async Task<IResult> HandleDeleteEmployeeAsync()
    {
        return await Task.FromResult(Results.Ok(nameof(HandleDeleteEmployeeAsync)));
    }


}

