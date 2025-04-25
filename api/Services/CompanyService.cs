using System.Linq.Expressions;
using api.Database;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class CompanyService : RepositoryService<Company>
{
    public CompanyService(AppDbContext dbContext) : base(dbContext){}

    public override async Task<List<Company>> GetAllAsync(Expression<Func<Company, bool>>? condition)
    {
        return await GetAllQuery(condition)
        .Include(c => c.Employees)
        .ToListAsync();
    }

    public override async Task DeleteAsync(Company c)
    {
        //load employees so they can be set to null by making use of cascading null behavior for optional relationship
        await dbContext.Entry(c).Collection(c => c.Employees).LoadAsync();
        await base.DeleteAsync(c);
    }
}