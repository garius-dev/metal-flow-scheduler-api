using MetalFlowScheduler.Api.Domain.Entities;

namespace MetalFlowScheduler.Api.Interfaces.Repositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        // Adicionar métodos específicos para Product se necessário
        // Exemplo:
        // Task<Product> GetProductByIdWithRoutesAsync(int productId, DateTime effectiveDate);
    }
}
