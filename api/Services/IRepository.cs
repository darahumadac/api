using System.Linq.Expressions;

namespace api.Services;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAll(Expression<Func<T, bool>>? condition);
    Task<T?> GetById(string id);
}