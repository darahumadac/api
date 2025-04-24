using api.Contracts.Responses;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace api.Endpoints;

public static partial class EmployeesEndpoints
{
    public static async Task<Ok<IEnumerable<EmployeeResponse>>> HandleGetEmployeesAsync([FromQuery] string? company, IRepository<Employee> employeeService)
    {
        var results = await employeeService.GetAll(e => string.IsNullOrEmpty(company) || e.Company != null && company.ToUpper() == e.Company.Name);
        var response = results
            .OrderByDescending(e => e.DaysWorked)
            .ThenBy(e => e.Name)
            .Select(
            e => new EmployeeResponse(
                Id: e.Id,
                Name: e.Name,
                Email: e.Email,
                Phone: e.Phone,
                Gender: e.Gender,
                DaysWorked: e.DaysWorked,
                CompanyName: e.Company?.Name,
                PhotoUrl: e.PhotoUrl
            ));
        return await Task.FromResult(TypedResults.Ok(response));
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

