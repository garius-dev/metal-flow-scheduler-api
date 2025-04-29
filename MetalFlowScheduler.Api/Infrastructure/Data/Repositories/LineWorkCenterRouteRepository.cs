using MetalFlowScheduler.Api.Domain.Entities;
using MetalFlowScheduler.Api.Interfaces.Repositories;

namespace MetalFlowScheduler.Api.Infrastructure.Data.Repositories
{
    public class LineWorkCenterRouteRepository : BaseRepository<LineWorkCenterRoute>, ILineWorkCenterRouteRepository
    {
        public LineWorkCenterRouteRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Implementar métodos específicos da interface ILineWorkCenterRouteRepository aqui
        // Exemplo:
        // public async Task<List<LineWorkCenterRoute>> GetActiveRoutesByLineIdAsync(int lineId, DateTime effectiveDate)
        // {
        //     return await _dbSet
        //         .Where(lwr => lwr.LineID == lineId && lwr.Enabled && lwr.EffectiveStartDate <= effectiveDate && (lwr.EffectiveEndDate == null || lwr.EffectiveEndDate >= effectiveDate))
        //         .OrderBy(lwr => lwr.Version) // Order by version
        //         .ThenBy(lwr => lwr.Order)
        //         .ToListAsync();
        // }
    }
}
