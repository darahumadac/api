using System.Linq.Expressions;
using api.Database;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services;
/// <summary>
/// RepositoryService<T> contains base implementations for the IRepository interface
/// </summary>
/// <typeparam name="T"></typeparam>
public class RepositoryService<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext dbContext;

    public RepositoryService(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    //Get all with optional filter expression
    public virtual async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? condition)
    {
        IQueryable<T> resources = GetAllQuery(condition);
        return await resources.ToListAsync();
    }

    public IQueryable<T> GetAllQuery(Expression<Func<T, bool>>? condition)
    {
        IQueryable<T> resources = dbContext.Set<T>().AsQueryable();
        if (condition != null)
        {
            resources = resources.Where(condition);
        }
        return resources;
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        return await dbContext.Set<T>().FindAsync(id);
    }

    public virtual async Task DeleteAsync(T resource)
    {
        dbContext.Set<T>().Remove(resource);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddAsync(T resource)
    {
        await dbContext.Set<T>().AddAsync(resource);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T resource)
    {
        dbContext.Set<T>().Update(resource);
        await dbContext.SaveChangesAsync();
    }
}