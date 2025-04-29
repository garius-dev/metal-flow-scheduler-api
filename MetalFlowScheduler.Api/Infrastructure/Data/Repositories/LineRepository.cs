using MetalFlowScheduler.Api.Domain.Entities;
using MetalFlowScheduler.Api.Interfaces.Repositories;

namespace MetalFlowScheduler.Api.Infrastructure.Data.Repositories
{
    public class LineRepository : BaseRepository<Line>, ILineRepository
    {
        public LineRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Implementar métodos específicos da interface ILineRepository aqui
        // Exemplo:
        // public async Task<Line> GetLineByIdWithWorkCentersAsync(int id)
        // {
        //     return await _dbSet.Include(l => l.WorkCenters).FirstOrDefaultAsync(l => l.ID == id && l.Enabled);
        // }
    }
}
