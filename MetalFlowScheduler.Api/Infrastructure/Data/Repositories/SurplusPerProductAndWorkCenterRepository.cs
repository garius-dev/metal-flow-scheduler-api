using MetalFlowScheduler.Api.Domain.Entities;
using MetalFlowScheduler.Api.Interfaces.Repositories;

namespace MetalFlowScheduler.Api.Infrastructure.Data.Repositories
{
    public class SurplusPerProductAndWorkCenterRepository : BaseRepository<SurplusPerProductAndWorkCenter>, ISurplusPerProductAndWorkCenterRepository
    {
        public SurplusPerProductAndWorkCenterRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Implementar métodos específicos da interface ISurplusPerProductAndWorkCenterRepository aqui
        // Exemplo:
        // public async Task<decimal> GetSurplusByProductAndWorkCenterAsync(int productId, int workCenterId)
        // {
        //     var surplusEntry = await _dbSet.FirstOrDefaultAsync(s => s.ProductID == productId && s.WorkCenterID == workCenterId && s.Enabled);
        //     return surplusEntry?.Surplus ?? 0; // Return surplus quantity or 0 if not found
        // }
    }
}
