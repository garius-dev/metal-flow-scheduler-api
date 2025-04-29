using MetalFlowScheduler.Api.Domain.Entities;

namespace MetalFlowScheduler.Api.Interfaces.Repositories
{
    public interface ISurplusPerProductAndWorkCenterRepository : IBaseRepository<SurplusPerProductAndWorkCenter>
    {
        // Adicionar métodos específicos para SurplusPerProductAndWorkCenter se necessário
        // Exemplo:
        // Task<decimal> GetSurplusByProductAndWorkCenterAsync(int productId, int workCenterId);
    }
}
