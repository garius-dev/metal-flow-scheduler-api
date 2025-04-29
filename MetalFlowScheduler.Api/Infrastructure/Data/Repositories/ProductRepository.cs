using MetalFlowScheduler.Api.Domain.Entities;
using MetalFlowScheduler.Api.Interfaces.Repositories;

namespace MetalFlowScheduler.Api.Infrastructure.Data.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Implementar métodos específicos da interface IProductRepository aqui
        // Exemplo:
        // public async Task<Product> GetProductByIdWithRoutesAsync(int productId, DateTime effectiveDate)
        // {
        //     return await _dbSet
        //         .Include(p => p.OperationRoutes.Where(pr => pr.Enabled && pr.EffectiveStartDate <= effectiveDate && (pr.EffectiveEndDate == null || pr.EffectiveEndDate >= effectiveDate)).OrderByDescending(pr => pr.Version).ThenBy(pr => pr.Order)) // Include active routes
        //         .Include(p => p.AvailableOnLines.Where(pal => pal.Enabled)) // Include available lines links
        //         .FirstOrDefaultAsync(p => p.ID == productId && p.Enabled);
        // }
    }
}
