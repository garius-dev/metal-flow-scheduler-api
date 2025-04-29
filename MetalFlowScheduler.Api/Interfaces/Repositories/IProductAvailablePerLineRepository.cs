using MetalFlowScheduler.Api.Domain.Entities;

namespace MetalFlowScheduler.Api.Interfaces.Repositories
{
    public interface IProductAvailablePerLineRepository : IBaseRepository<ProductAvailablePerLine>
    {
        // Adicionar métodos específicos para ProductAvailablePerLine se necessário
        // Exemplo:
        // Task<List<Line>> GetAvailableLinesForProductAsync(int productId);
    }
}
