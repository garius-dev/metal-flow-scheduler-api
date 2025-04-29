using MetalFlowScheduler.Api.Domain.Entities;
using MetalFlowScheduler.Api.Interfaces.Repositories;

namespace MetalFlowScheduler.Api.Infrastructure.Data.Repositories
{
    public class OperationRepository : BaseRepository<Operation>, IOperationRepository
    {
        public OperationRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Implementar métodos específicos da interface IOperationRepository aqui
        // Exemplo:
        // public async Task<List<Operation>> GetOperationsByWorkCenterAndOperationTypeAsync(int workCenterId, int operationTypeId)
        // {
        //     return await _dbSet.Where(o => o.WorkCenterID == workCenterId && o.OperationTypeID == operationTypeId && o.Enabled).ToListAsync();
        // }
    }
}
