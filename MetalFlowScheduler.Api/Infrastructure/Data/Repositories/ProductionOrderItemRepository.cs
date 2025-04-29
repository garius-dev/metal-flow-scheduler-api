using MetalFlowScheduler.Api.Domain.Entities;
using MetalFlowScheduler.Api.Interfaces.Repositories;

namespace MetalFlowScheduler.Api.Infrastructure.Data.Repositories
{
    public class ProductionOrderItemRepository : BaseRepository<ProductionOrderItem>, IProductionOrderItemRepository
    {
        public ProductionOrderItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Implementar métodos específicos da interface IProductionOrderItemRepository aqui
        // Exemplo:
        // public async Task<List<ProductionOrderItem>> GetItemsByOrderIdAsync(int orderId)
        // {
        //     return await _dbSet.Where(item => item.ProductionOrderID == orderId && item.Enabled).ToListAsync();
        // }
    }
}
