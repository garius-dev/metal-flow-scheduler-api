using MetalFlowScheduler.Api.Domain.Entities;

namespace MetalFlowScheduler.Api.Interfaces.Services
{
    public interface IProductService : IBaseDataService<Product>
    {
        // Add specific service methods for Product if needed
        // For example:
        // Task<Product> GetProductByIdWithRoutesAsync(int productId, DateTime effectiveDate);
    }
}
