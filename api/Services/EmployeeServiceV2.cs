using System.Linq.Expressions;
using api.Database;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

/// <summary>
/// EmployeeServiceV2 implementation inherits from RepositoryService base class
/// </summary>
public class EmployeeServiceV2 : RepositoryService<Employee>
{
    public EmployeeServiceV2(AppDbContext dbContext) : base(dbContext){}

    //Get all with optional filter expression
    public override async Task<List<Employee>> GetAllAsync(Expression<Func<Employee, bool>>? condition)
    {
        IQueryable<Employee> employees = GetAllQuery(condition);
        return await employees
            .Include(e => e.Company)
            .ToListAsync();
    }
}