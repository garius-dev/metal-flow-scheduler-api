using MetalFlowScheduler.Api.Domain.Entities;

namespace MetalFlowScheduler.Api.Interfaces.Repositories
{
    public interface ILineRepository : IBaseRepository<Line>
    {
        // Adicionar métodos específicos para Line se necessário
        // Exemplo:
        // Task<Line> GetLineByIdWithWorkCentersAsync(int id);
    }
}
