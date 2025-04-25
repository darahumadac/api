using System.Linq.Expressions;

namespace api.Services;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? condition);
    Task<T?> GetByIdAsync(string id);
    Task<bool> DeleteAsync(T resource);
}