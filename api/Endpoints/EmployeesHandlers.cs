using api.Contracts.Responses;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace api.Endpoints;

public static partial class EmployeesEndpoints
{
    public static async Task<Ok<IEnumerable<GetEmployeesResponse>>> HandleGetEmployeesAsync([FromQuery] string? company, IRepository<Employee> employeeService)
    {
        var results = await employeeService.GetAllAsync(e => string.IsNullOrEmpty(company) || e.Company != null && company.ToUpper() == e.Company.Name);
        var response = results
            .OrderByDescending(e => e.DaysWorked)
            .ThenBy(e => e.Name)
            .Select(
            e => new GetEmployeesResponse(
                Id: e.Id,
                Name: e.Name,
                Email: e.Email,
                Phone: e.Phone,
                Gender: e.Gender,
                PhotoUrl: e.PhotoUrl,
                DaysWorked: e.DaysWorked,
                CompanyName: e.Company?.Name
            ));
        return await Task.FromResult(TypedResults.Ok(response));
    }
    public static async Task<Results<Ok<GetEmployeeByIdResponse>, NotFound>> HandleGetEmployeeAsync(string id, IRepository<Employee> employeeService)
    {
        var employee = await employeeService.GetByIdAsync(id);
        if (employee == null)
        {
            return TypedResults.NotFound();
        }
        var response = new GetEmployeeByIdResponse(
            Id: employee.Id,
                Name: employee.Name,
                Email: employee.Email,
                Phone: employee.Phone,
                Gender: employee.Gender,
                PhotoUrl: employee.PhotoUrl,
                CompanyId: employee.CompanyId
        );
        return TypedResults.Ok(response);
    }

    public static async Task<Results<CreatedAtRoute, ValidationProblem, ProblemHttpResult>> HandleAddEmployeeAsync(IRepository<Employee> employeeService)
    {
        return TypedResults.CreatedAtRoute("GetEmployeeById", new { id = "1" });
    }

    public static async Task<IResult> HandleUpdateEmployeeAsync()
    {
        return await Task.FromResult(Results.Ok(nameof(HandleUpdateEmployeeAsync)));
    }

    public static async Task<Results<NotFound, NoContent, ProblemHttpResult>> HandleDeleteEmployeeAsync(string id, IRepository<Employee> employeeService)
    {
        var employee = await employeeService.GetByIdAsync(id);
        if (employee == null)
        {
            return TypedResults.NotFound();
        }

        var deleted = await employeeService.DeleteAsync(employee);
        if (deleted)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.Problem(statusCode: 500, detail: "There was an issue deleting the record");

    }


}

