using MetalFlowScheduler.Api.Domain.Entities;

namespace MetalFlowScheduler.Api.Interfaces.Repositories
{
    public interface IProductOperationRouteRepository : IBaseRepository<ProductOperationRoute>
    {
        // Adicionar métodos específicos para ProductOperationRoute se necessário
        // Exemplo:
        // Task<List<ProductOperationRoute>> GetActiveRoutesByProductIdAsync(int productId, DateTime effectiveDate);
    }
}
