using api.Contracts.Request;
using api.Contracts.Responses;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace api.Endpoints;

public static partial class EmployeesEndpoints
{
    public static async Task<Ok<IEnumerable<GetEmployeesResponse>>> HandleGetEmployeesAsync([FromQuery] string? company, IRepository<Employee> employeeService)
    {
        var results = await employeeService.GetAllAsync(e => string.IsNullOrEmpty(company) || e.Company != null && company == e.Company.Name);
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
        return TypedResults.Ok(response);
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

    public static async Task<CreatedAtRoute> HandleAddEmployeeAsync(EmployeeRequest request, IRepository<Employee> employeeService)
    {
        //validation is handled via endpoint filter
        var newEmployee = new Employee
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Gender = request.Gender,
            CompanyId = request.CompanyId,
            StartDate = DateTime.UtcNow,
        };

        await employeeService.AddAsync(newEmployee);
        return TypedResults.CreatedAtRoute("GetEmployeeById", new { id = newEmployee.Id });
    }

    public static async Task<Results<Ok<UpdateEmployeeResponse>, NotFound>> HandleUpdateEmployeeAsync(EmployeeRequest request, string id, IRepository<Employee> employeeService)
    {
        var existingEmployee = await employeeService.GetByIdAsync(id);
        if (existingEmployee == null)
        {
            return TypedResults.NotFound();
        }

        existingEmployee.Name = request.Name;
        existingEmployee.Email = request.Email;
        existingEmployee.Phone = request.Phone;
        existingEmployee.Gender = request.Gender;

        var updateDate = DateTime.UtcNow;
        if(existingEmployee.CompanyId != request.CompanyId)
        {
            existingEmployee.CompanyId = request.CompanyId;
            existingEmployee.StartDate = request.CompanyId != null ? updateDate : null;
        }
        existingEmployee.UpdatedDate = updateDate;

        await employeeService.UpdateAsync(existingEmployee);

        var response = new UpdateEmployeeResponse(
            Name: existingEmployee.Name,
            Email: existingEmployee.Email,
            Phone: existingEmployee.Phone,
            Gender: existingEmployee.Gender,
            CompanyId: existingEmployee.CompanyId,
            StartDate: existingEmployee.StartDate
        );
        return TypedResults.Ok(response);
    }

    public static async Task<Results<NoContent, NotFound>> HandleDeleteEmployeeAsync(string id, IRepository<Employee> employeeService)
    {
        var employee = await employeeService.GetByIdAsync(id);
        if (employee == null)
        {
            return TypedResults.NotFound();
        }

        await employeeService.DeleteAsync(employee);
        return TypedResults.NoContent();
    }


}

