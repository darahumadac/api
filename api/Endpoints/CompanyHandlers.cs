using api.Contracts.Request;
using api.Contracts.Responses;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace api.Endpoints;

public static partial class CompanyEndpoints
{
    private static async Task<Ok<IEnumerable<GetCompaniesResponse>>> HandleGetCompaniesAsync([FromQuery] string? location, IRepository<Company> companyService)
    {   
        var companies = await companyService.GetAllAsync(c => string.IsNullOrEmpty(location) || c.Location == location);
        var response = companies
            .OrderByDescending(c => c.Employees.Count)
            .ThenBy(c => c.Name)
            .Select(c => new GetCompaniesResponse(
                Id: c.Id,
                Name: c.Name,
                Description: c.Description,
                EmployeeCount: c.Employees.Count,
                Location: c.Location,
                PhotoUrl: c.PhotoUrl                
            ));
            
        return TypedResults.Ok(response);
    }
    private static async Task<Results<Ok<GetCompanyResponse>,NotFound>> HandleGetCompanyAsync(string id, IRepository<Company> companyService)
    {
        var company = await companyService.GetByIdAsync(id);
        if(company == null)
        {
            return TypedResults.NotFound();
        }
        var response = new GetCompanyResponse(
            Id: company.Id,
            Name: company.Name,
            Description: company.Description,
            Location: company.Location,
            PhotoUrl: company.PhotoUrl
        );
        return TypedResults.Ok(response);
    }

    private static async Task<CreatedAtRoute> HandleAddCompanyAsync(CompanyRequest request, IRepository<Company> companyService)
    {
        //validation is done via endpoint filter
        var newCompany = new Company{
            Name = request.Name,
            Description = request.Description,
            Location = request.Location,
        };
        await companyService.AddAsync(newCompany);
        return TypedResults.CreatedAtRoute("GetCompanyById", new {id = newCompany.Id});
    }


    private static async Task<Results<Ok<CompanyRequest>, NotFound>> HandleUpdateCompanyAsync(CompanyRequest request, string id, IRepository<Company> companyService)
    {
        var existingCompany = await companyService.GetByIdAsync(id);
        if(existingCompany == null)
        {
            return TypedResults.NotFound();
        }

        existingCompany.Name = request.Name;
        existingCompany.Description  = request.Description;
        existingCompany.Location = request.Location;
        existingCompany.UpdatedDate = DateTime.UtcNow;

        await companyService.UpdateAsync(existingCompany);

        var response = new CompanyRequest(
            Name: existingCompany.Name,
            Description: existingCompany.Description,
            Location: existingCompany.Location
        );

        return TypedResults.Ok(response);
    
    }

    private static async Task<Results<NoContent, NotFound>> HandleDeleteCompanyAsync(string id, IRepository<Company> companyService)
    {
        var company = await companyService.GetByIdAsync(id);
        if(company == null)
        {
            return TypedResults.NotFound();
        }

        await companyService.DeleteAsync(company);
        return TypedResults.NoContent();
    }

}