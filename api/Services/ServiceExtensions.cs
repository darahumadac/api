using api.Contracts.Request;
using api.Models;
using FluentValidation;

namespace api.Services;

public static class ServiceExtensions
{
    public static IServiceCollection AddEmployeeServices(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Employee>, EmployeeServiceV2>();
        services.AddScoped<IValidator<EmployeeRequest>, EmployeeDataValidator>();
        
        return services;
    }

    public static IServiceCollection AddCompanyServices(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Company>, CompanyService>();
        services.AddScoped<IValidator<CompanyRequest>, CompanyDataValidator>();
        
        return services;
    }
}