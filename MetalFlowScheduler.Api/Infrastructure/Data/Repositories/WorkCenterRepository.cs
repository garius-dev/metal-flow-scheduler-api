using MetalFlowScheduler.Api.Domain.Entities;
using MetalFlowScheduler.Api.Interfaces.Repositories;

namespace MetalFlowScheduler.Api.Infrastructure.Data.Repositories
{
    public class WorkCenterRepository : BaseRepository<WorkCenter>, IWorkCenterRepository
    {
        public WorkCenterRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Implementar métodos específicos da interface IWorkCenterRepository aqui
        // Exemplo:
        // public async Task<WorkCenter> GetWorkCenterByIdWithOperationsAsync(int id)
        // {
        //     return await _dbSet.Include(wc => wc.Operations).FirstOrDefaultAsync(wc => wc.ID == id && wc.Enabled);
        // }
    }
}
