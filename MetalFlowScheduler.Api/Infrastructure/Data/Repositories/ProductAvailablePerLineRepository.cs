using MetalFlowScheduler.Api.Domain.Entities;
using MetalFlowScheduler.Api.Interfaces.Repositories;

namespace MetalFlowScheduler.Api.Infrastructure.Data.Repositories
{
    public class ProductAvailablePerLineRepository : BaseRepository<ProductAvailablePerLine>, IProductAvailablePerLineRepository
    {
        public ProductAvailablePerLineRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Implementar métodos específicos da interface IProductAvailablePerLineRepository aqui
        // Exemplo:
        // public async Task<List<Line>> GetAvailableLinesForProductAsync(int productId)
        // {
        //     return await _dbSet
        //         .Where(pal => pal.ProductID == productId && pal.Enabled)
        //         .Select(pal => pal.Line) // Select the related Line entity
        //         .Where(l => l.Enabled) // Ensure the Line itself is enabled
        //         .ToListAsync();
        // }
    }
}
