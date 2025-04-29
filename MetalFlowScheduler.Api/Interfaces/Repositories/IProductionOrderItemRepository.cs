using MetalFlowScheduler.Api.Domain.Entities;

namespace MetalFlowScheduler.Api.Interfaces.Repositories
{
    public interface IProductionOrderItemRepository : IBaseRepository<ProductionOrderItem>
    {
        // Adicionar métodos específicos para ProductionOrderItem se necessário
        // Exemplo:
        // Task<List<ProductionOrderItem>> GetItemsByOrderIdAsync(int orderId);
    }
}
