using api.Models;

namespace api.Services;

public static class ServiceExtensions
{
    public static IServiceCollection AddEmployeeServices(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Employee>, EmployeeService>();
        
        return services;
    }
}