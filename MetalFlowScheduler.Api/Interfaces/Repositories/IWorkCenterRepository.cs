using MetalFlowScheduler.Api.Domain.Entities;

namespace MetalFlowScheduler.Api.Interfaces.Repositories
{
    public interface IWorkCenterRepository : IBaseRepository<WorkCenter>
    {
        // Adicionar métodos específicos para WorkCenter se necessário
        // Exemplo:
        // Task<WorkCenter> GetWorkCenterByIdWithOperationsAsync(int id); // Exemplo incluindo entidades relacionadas
    }
}
