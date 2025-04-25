
using api.Contracts.Request;
using api.Endpoints.Filters;

namespace api.Endpoints;

public static partial class CompanyEndpoints
{
    public static void MapCompanyApi(this IEndpointRouteBuilder app)
    {
        var companies = app.MapGroup("/companies");

        companies.MapGet("/", HandleGetCompaniesAsync)
            .WithName("GetCompanies");

        companies.MapGet("/{id}", HandleGetCompanyAsync)
            .WithName("GetCompanyById");
        
        var company = app.MapGroup("/company");

        company.MapPost("/", HandleAddCompanyAsync)
            .AddEndpointFilter<ValidateRequestFilter<CompanyRequest>>()
            .WithName("AddCompany");

        company.MapPut("/{id}", HandleUpdateCompanyAsync)
            .AddEndpointFilter<ValidateRequestFilter<CompanyRequest>>()
            .WithName("UpdateCompany");

        company.MapDelete("/{id}", HandleDeleteCompanyAsync)
            .WithName("DeleteCompany");
            
    }
}