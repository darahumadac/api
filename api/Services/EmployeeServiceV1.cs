using System.Linq.Expressions;
using api.Database;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class EmployeeServiceV1 : IRepository<Employee>
{
    private readonly AppDbContext dbContext;

    public EmployeeServiceV1(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    //Get all with optional filter expression
    public async Task<List<Employee>> GetAllAsync(Expression<Func<Employee, bool>>? condition)
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

    public async Task<Employee?> GetByIdAsync(string id)
    {
        return await dbContext.Employees.FindAsync(id);
    }

    public async Task DeleteAsync(Employee e)
    {
        dbContext.Employees.Remove(e);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddAsync(Employee resource)
    {
        await dbContext.Employees.AddAsync(resource);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Employee resource)
    {
        dbContext.Employees.Update(resource);
        await dbContext.SaveChangesAsync();
    }
}