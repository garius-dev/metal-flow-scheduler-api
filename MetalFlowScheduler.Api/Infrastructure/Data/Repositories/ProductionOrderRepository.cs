using MetalFlowScheduler.Api.Domain.Entities;
using MetalFlowScheduler.Api.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MetalFlowScheduler.Api.Infrastructure.Data.Repositories
{
    public class ProductionOrderRepository : BaseRepository<ProductionOrder>, IProductionOrderRepository
    {
        public ProductionOrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Implementar métodos específicos da interface IProductionOrderRepository aqui
        // Exemplo:
        public async Task<List<ProductionOrder>> GetPendingOrdersAsync()
        {
            // Exemplo: Obter ordens habilitadas com seus itens
            // Em um cenário real, pode filtrar por status (e.g., Status == "Pending")
            return await _dbSet
                .Include(po => po.Items.Where(item => item.Enabled)) // Incluir itens habilitados
                .Where(po => po.Enabled) // Apenas ordens habilitadas
                                         // .Where(po => po.Status == "Pending") // Exemplo de filtro por status
                .ToListAsync();
        }

        // public async Task<List<ProductionOrder>> GetOrdersForPlanningAsync(DateTime planningHorizonStart, DateTime planningHorizonEnd)
        // {
        //     // Exemplo: Obter ordens pendentes dentro de um horizonte de planejamento
        //     return await _dbSet
        //         .Include(po => po.Items.Where(item => item.Enabled))
        //         .Where(po => po.Enabled && po.EarliestStartDate <= planningHorizonEnd && po.Deadline >= planningHorizonStart)
        //         // .Where(po => po.Status == "Pending")
        //         .ToListAsync();
        // }
    }
}
