using MetalFlowScheduler.Api.Domain.Entities;
using MetalFlowScheduler.Api.Interfaces.Repositories;

namespace MetalFlowScheduler.Api.Infrastructure.Data.Repositories
{
    public class OperationTypeRepository : BaseRepository<OperationType>, IOperationTypeRepository
    {
        public OperationTypeRepository(ApplicationDbContext context) : base(context)
        {
            
        }
    }
}
