using System.Linq.Expressions;

namespace MetalFlowScheduler.Api.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<int> SaveChangesAsync(); // Para salvar alterações no contexto

        // Método comum para obter entidades habilitadas
        Task<List<T>> GetAllEnabledAsync();
    }
}
