using MetalFlowScheduler.Api.Domain.Entities;

namespace MetalFlowScheduler.Api.Interfaces.Repositories
{
    public interface IOperationRepository : IBaseRepository<Operation>
    {
        // Adicionar métodos específicos para Operation se necessário
        // Exemplo:
        // Task<List<Operation>> GetOperationsByWorkCenterAndOperationTypeAsync(int workCenterId, int operationTypeId);
    }
}
