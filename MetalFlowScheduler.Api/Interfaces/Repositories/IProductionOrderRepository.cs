using MetalFlowScheduler.Api.Domain.Entities;

namespace MetalFlowScheduler.Api.Interfaces.Repositories
{
    public interface IProductionOrderRepository : IBaseRepository<ProductionOrder>
    {
        // Adicionar métodos específicos para ProductionOrder necessários pelo solver ou outras partes da aplicação
        // Exemplo:
        Task<List<ProductionOrder>> GetPendingOrdersAsync();
        // Task<List<ProductionOrder>> GetOrdersForPlanningAsync(DateTime planningHorizonStart, DateTime planningHorizonEnd);
    }
}
