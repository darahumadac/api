using System.Linq.Expressions;
using api.Database;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class EmployeeService : IRepository<Employee>
{
    private readonly AppDbContext dbContext;

    public EmployeeService(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    //Get all with optional filter expression
    public async Task<List<Employee>> GetAll(Expression<Func<Employee, bool>>? condition)
    {
        IQueryable<Employee> employees = dbContext.Employees.AsQueryable();
        if (condition != null)
        {
            employees = employees.Where(condition);
        }
        return await employees
            .Include(e => e.Company)
            .ToListAsync();
    }
}