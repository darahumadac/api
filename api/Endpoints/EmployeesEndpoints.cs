
using api.Contracts.Request;
using api.Endpoints.Filters;
using FluentValidation;

namespace api.Endpoints;

public static partial class EmployeesEndpoints
{
    public static void MapEmployeesApi(this IEndpointRouteBuilder app)
    {
        //create resource group
        var employees = app.MapGroup("/employees");

        //map endpoints
        employees.MapGet("/", HandleGetEmployeesAsync)
            .WithName("GetEmployees");

        employees.MapGet("/{id}", HandleGetEmployeeAsync)
            .WithName("GetEmployeeById");

        //single resource
        var employee = app.MapGroup("/employee");

        //map endpoints
        employee.MapPost("/", HandleAddEmployeeAsync)
            .AddEndpointFilter<ValidateRequestFilter<EmployeeRequest>>()
            .WithName("AddEmployee");

        employee.MapPut("/{id}", HandleUpdateEmployeeAsync)
            .AddEndpointFilter<ValidateRequestFilter<EmployeeRequest>>()
            .WithName("UpdateEmployee");

        employee.MapDelete("/{id}", HandleDeleteEmployeeAsync)
            .WithName("DeleteEmployee");
    }

}