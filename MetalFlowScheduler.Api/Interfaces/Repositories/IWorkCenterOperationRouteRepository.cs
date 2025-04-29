using MetalFlowScheduler.Api.Domain.Entities;

namespace MetalFlowScheduler.Api.Interfaces.Repositories
{
    public interface IWorkCenterOperationRouteRepository : IBaseRepository<WorkCenterOperationRoute>
    {
        // Adicionar métodos específicos para WorkCenterOperationRoute se necessário
        // Exemplo:
        // Task<List<WorkCenterOperationRoute>> GetActiveRoutesByWorkCenterIdAsync(int workCenterId, DateTime effectiveDate);
    }
}
