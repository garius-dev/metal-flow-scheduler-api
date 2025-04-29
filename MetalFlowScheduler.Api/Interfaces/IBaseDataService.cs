namespace MetalFlowScheduler.Api.Interfaces
{
    public interface IBaseDataService<T> where T : class
    {
        Task<List<T>> GetAllEnabledAsync();
        // Add other common service methods if needed (e.g., GetByIdAsync)
        // Task<T> GetByIdAsync(int id);
    }
}
