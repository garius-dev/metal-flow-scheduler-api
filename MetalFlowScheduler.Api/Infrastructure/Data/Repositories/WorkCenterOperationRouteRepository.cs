using MetalFlowScheduler.Api.Domain.Entities;
using MetalFlowScheduler.Api.Interfaces.Repositories;

namespace MetalFlowScheduler.Api.Infrastructure.Data.Repositories
{
    public class WorkCenterOperationRouteRepository : BaseRepository<WorkCenterOperationRoute>, IWorkCenterOperationRouteRepository
    {
        public WorkCenterOperationRouteRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Implementar métodos específicos da interface IWorkCenterOperationRouteRepository aqui
        // Exemplo:
        // public async Task<List<WorkCenterOperationRoute>> GetActiveRoutesByWorkCenterIdAsync(int workCenterId, DateTime effectiveDate)
        // {
        //     return await _dbSet
        //         .Where(wor => wor.WorkCenterID == workCenterId && wor.Enabled && wor.EffectiveStartDate <= effectiveDate && (wor.EffectiveEndDate == null || wor.EffectiveEndDate >= effectiveDate))
        //         .OrderBy(wor => wor.Version) // Order by version if you want the latest
        //         .ThenBy(wor => wor.Order)
        //         .ToListAsync();
        // }
    }
}
