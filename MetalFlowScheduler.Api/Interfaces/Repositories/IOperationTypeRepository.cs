using MetalFlowScheduler.Api.Domain.Entities;

namespace MetalFlowScheduler.Api.Interfaces.Repositories
{
    public interface IOperationTypeRepository : IBaseRepository<OperationType>
    {
        // Adicionar métodos específicos para OperationType se necessário
        // Task<OperationType> GetByNameAsync(string name);
    }
}
