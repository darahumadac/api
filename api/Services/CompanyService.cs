using System.Linq.Expressions;
using api.Database;
using api.Models;

namespace api.Services;

public class CompanyService : IRepository<Company>
{
    private readonly AppDbContext dbContext;

    public CompanyService(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task AddAsync(Company resource)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Company resource)
    {
        throw new NotImplementedException();
    }

    public Task<List<Company>> GetAllAsync(Expression<Func<Company, bool>>? condition)
    {
        throw new NotImplementedException();
    }

    public async Task<Company?> GetByIdAsync(string id)
    {
        return await dbContext.Company.FindAsync(id);
    }
}