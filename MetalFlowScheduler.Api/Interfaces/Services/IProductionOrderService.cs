using MetalFlowScheduler.Api.Domain.Entities;

namespace MetalFlowScheduler.Api.Interfaces.Services
{
    public interface IProductionOrderService : IBaseDataService<ProductionOrder>
    {
        // Add specific service methods for ProductionOrder needed by the solver
        // For example:
        Task<List<ProductionOrder>> GetPendingOrdersAsync();
        // Task<List<ProductionOrder>> GetOrdersForPlanningAsync(DateTime planningHorizonStart, DateTime planningHorizonEnd);
    }
}
