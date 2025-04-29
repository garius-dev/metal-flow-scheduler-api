using MetalFlowScheduler.Api.Domain.Entities;

namespace MetalFlowScheduler.Api.Interfaces.Repositories
{
    public interface ILineWorkCenterRouteRepository : IBaseRepository<LineWorkCenterRoute>
    {
        // Adicionar métodos específicos para LineWorkCenterRoute se necessário
        // Exemplo:
        // Task<List<LineWorkCenterRoute>> GetActiveRoutesByLineIdAsync(int lineId, DateTime effectiveDate);
    }
}
