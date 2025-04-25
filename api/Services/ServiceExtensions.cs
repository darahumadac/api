using api.Contracts.Request;
using api.Models;
using FluentValidation;

namespace api.Services;

public static class ServiceExtensions
{
    public static IServiceCollection AddEmployeeServices(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Employee>, EmployeeService>();
        services.AddScoped<IValidator<EmployeeData>, EmployeeDataValidator>();
        
        return services;
    }

    public static IServiceCollection AddCompanyServices(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Company>, CompanyService>();
        // services.AddScoped<IValidator<CompanyData>, CompanyDataValidator>();
        
        return services;
    }
}