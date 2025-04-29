using MetalFlowScheduler.Api.Domain.Entities;
using MetalFlowScheduler.Api.Interfaces.Repositories;

namespace MetalFlowScheduler.Api.Infrastructure.Data.Repositories
{
    public class ProductOperationRouteRepository : BaseRepository<ProductOperationRoute>, IProductOperationRouteRepository
    {
        public ProductOperationRouteRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Implementar métodos específicos da interface IProductOperationRouteRepository aqui
        // Exemplo:
        // public async Task<List<ProductOperationRoute>> GetActiveRoutesByProductIdAsync(int productId, DateTime effectiveDate)
        // {
        //     return await _dbSet
        //         .Where(pr => pr.ProductID == productId && pr.Enabled && pr.EffectiveStartDate <= effectiveDate && (pr.EffectiveEndDate == null || pr.EffectiveEndDate >= effectiveDate))
        //         .OrderByDescending(pr => pr.Version) // Get the latest version
        //         .ThenBy(pr => pr.Order) // Order by step order
        //         .ToListAsync();
        // }
    }
}
