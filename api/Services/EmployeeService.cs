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

    public async Task<bool> DeleteAsync(Employee e)
    {
        using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            dbContext.Employees.Remove(e);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return true;
        }
        catch(Exception ex)
        {
            //TODO: Add serilog logging here
            return false;
        }
        
    }
}