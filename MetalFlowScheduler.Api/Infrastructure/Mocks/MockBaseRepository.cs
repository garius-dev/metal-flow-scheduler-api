using MetalFlowScheduler.Api.Domain.Entities;
using MetalFlowScheduler.Api.Infrastructure.Data.Repositories;
using MetalFlowScheduler.Api.Interfaces;
using MetalFlowScheduler.Api.Interfaces.Repositories;
using System.Linq.Expressions;

namespace MetalFlowScheduler.Api.Infrastructure.Mocks
{
    /// <summary>
    /// Base class for mock repositories providing common in-memory operations.
    /// Classe base para repositórios mock fornecendo operações comuns em memória.
    /// </summary>
    /// <typeparam name="T">The entity type, must inherit from BaseEntity.</typeparam>
    public abstract class MockBaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly List<T> _mockDataStorage;
        private int _nextId = 1; // Simple ID generation for Adds

        protected MockBaseRepository(List<T> mockDataStorage)
        {
            MockDataFactory.Initialize(); // Ensure data is loaded
            _mockDataStorage = mockDataStorage ?? throw new ArgumentNullException(nameof(mockDataStorage));
            if (_mockDataStorage.Any())
            {
                _nextId = _mockDataStorage.Max(e => e.ID) + 1;
            }
        }

        public virtual Task<T?> GetByIdAsync(int id)
        {
            var entity = _mockDataStorage.FirstOrDefault(e => e.ID == id);
            return Task.FromResult(entity);
        }

        public virtual Task<List<T>> GetAllAsync()
        {
            // Return a copy to prevent external modification of the mock source
            // Retorna uma cópia para prevenir modificação externa da fonte mock
            return Task.FromResult(_mockDataStorage.ToList());
        }

        public virtual Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            // Compile and execute the predicate against the in-memory list
            // Compila e executa o predicado contra a lista em memória
            var query = _mockDataStorage.Where(predicate.Compile()).ToList();
            return Task.FromResult(query);
        }

        public virtual Task AddAsync(T entity)
        {
            if (entity.ID == 0) // Assign ID if not set
            {
                entity.ID = _nextId++;
            }
            else if (_mockDataStorage.Any(e => e.ID == entity.ID))
            {
                // Handle potential ID collision if needed, or assume IDs are managed externally for adds with ID
            }
            else
            {
                // If adding with a specific ID higher than current max, update nextId
                _nextId = Math.Max(_nextId, entity.ID + 1);
            }

            entity.CreatedAt = DateTime.UtcNow;
            entity.LastUpdate = DateTime.UtcNow;
            _mockDataStorage.Add(entity);
            return Task.CompletedTask;
        }

        public virtual Task AddRangeAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.ID == 0) entity.ID = _nextId++;
                else _nextId = Math.Max(_nextId, entity.ID + 1);
                entity.CreatedAt = DateTime.UtcNow;
                entity.LastUpdate = DateTime.UtcNow;
                _mockDataStorage.Add(entity);
            }
            return Task.CompletedTask;
        }

        public virtual void Update(T entity)
        {
            var existingEntity = _mockDataStorage.FirstOrDefault(e => e.ID == entity.ID);
            if (existingEntity != null)
            {
                // Simple update: replace the existing entity instance
                // You might want more sophisticated merging logic depending on needs
                // Atualização simples: substitui a instância da entidade existente
                // Pode ser necessária uma lógica de mesclagem mais sofisticada dependendo das necessidades
                var index = _mockDataStorage.IndexOf(existingEntity);
                entity.LastUpdate = DateTime.UtcNow; // Update timestamp
                entity.CreatedAt = existingEntity.CreatedAt; // Preserve original creation date
                _mockDataStorage[index] = entity;
            }
            // Optionally throw an exception if not found, or do nothing
        }

        public virtual void Remove(T entity)
        {
            var existingEntity = _mockDataStorage.FirstOrDefault(e => e.ID == entity.ID);
            if (existingEntity != null)
            {
                _mockDataStorage.Remove(existingEntity);
            }
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            var idsToRemove = entities.Select(e => e.ID).ToList();
            _mockDataStorage.RemoveAll(e => idsToRemove.Contains(e.ID));
        }

        public virtual Task<int> SaveChangesAsync()
        {
            // No actual saving needed for mocks, just simulate success
            // Nenhuma gravação real necessária para mocks, apenas simula sucesso
            return Task.FromResult(1); // Simulate one change saved
        }

        public virtual Task<List<T>> GetAllEnabledAsync()
        {
            var enabledEntities = _mockDataStorage.Where(e => e.Enabled).ToList();
            return Task.FromResult(enabledEntities);
        }
    }

    // --- Concrete Mock Repository Implementations ---
    // --- Implementações Concretas de Repositórios Mock ---

    public class MockLineRepository : MockBaseRepository<Line>, ILineRepository
    {
        // Pass the specific static list from MockDataFactory to the base constructor
        // Passa a lista estática específica de MockDataFactory para o construtor base
        public MockLineRepository() : base(MockDataFactory.Lines) { }

        // Implement specific ILineRepository methods here if any
        // Implementar métodos específicos de ILineRepository aqui, se houver
    }

    public class MockWorkCenterRepository : MockBaseRepository<WorkCenter>, IWorkCenterRepository
    {
        public MockWorkCenterRepository() : base(MockDataFactory.WorkCenters) { }

        // Implement specific IWorkCenterRepository methods here if any
    }

    public class MockOperationRepository : MockBaseRepository<Operation>, IOperationRepository
    {
        public MockOperationRepository() : base(MockDataFactory.Operations) { }

        // Implement specific IOperationRepository methods here if any
    }

    public class MockProductRepository : MockBaseRepository<Product>, IProductRepository
    {
        public MockProductRepository() : base(MockDataFactory.Products) { }

        // Implement specific IProductRepository methods here if any
    }

    public class MockProductionOrderRepository : MockBaseRepository<ProductionOrder>, IProductionOrderRepository
    {
        public MockProductionOrderRepository() : base(MockDataFactory.ProductionOrders) { }

        /// <summary>
        /// Mock implementation to get "pending" orders (all enabled mock orders in this case).
        /// Includes related items.
        /// Implementação mock para obter ordens "pendentes" (todas as ordens mock habilitadas neste caso).
        /// Inclui itens relacionados.
        /// </summary>
        public Task<List<ProductionOrder>> GetPendingOrdersAsync()
        {
            // In a real scenario, you might filter by a Status property.
            // Here, we just return all enabled orders with their items (which were linked in SeedData).
            // Num cenário real, poderia filtrar por uma propriedade Status.
            // Aqui, apenas retornamos todas as ordens habilitadas com seus itens (que foram ligados em SeedData).
            var pendingOrders = _mockDataStorage
                .Where(po => po.Enabled)
                .ToList(); // Get enabled orders

            // Ensure items are loaded (they should be linked in MockDataFactory.SeedData)
            // Garantir que os itens estão carregados (devem estar ligados em MockDataFactory.SeedData)
            foreach (var order in pendingOrders)
            {
                // If Items list wasn't populated in SeedData, you'd fetch them here:
                if (order.Items == null || !order.Items.Any())
                {
                    order.Items = MockDataFactory.ProductionOrderItems
                                    .Where(item => item.ProductionOrderID == order.ID && item.Enabled)
                                    .ToList();
                }
            }


            return Task.FromResult(pendingOrders);
        }
    }

    public class MockProductionOrderItemRepository : MockBaseRepository<ProductionOrderItem>, IProductionOrderItemRepository
    {
        public MockProductionOrderItemRepository() : base(MockDataFactory.ProductionOrderItems) { }

        // Implement specific IProductionOrderItemRepository methods here if any
    }

    public class MockSurplusPerProductAndWorkCenterRepository : MockBaseRepository<SurplusPerProductAndWorkCenter>, ISurplusPerProductAndWorkCenterRepository
    {
        public MockSurplusPerProductAndWorkCenterRepository() : base(MockDataFactory.SurplusStocks) { }

        // Implement specific ISurplusPerProductAndWorkCenterRepository methods here if any
        // Example:
        // public Task<decimal> GetSurplusByProductAndWorkCenterAsync(int productId, int workCenterId)
        // {
        //     var surplus = _mockDataStorage
        //         .FirstOrDefault(s => s.ProductID == productId && s.WorkCenterID == workCenterId && s.Enabled)?
        //         .Surplus ?? 0m;
        //     return Task.FromResult(surplus);
        // }
    }

    public class MockOperationTypeRepository : MockBaseRepository<OperationType>, IOperationTypeRepository
    {
        public MockOperationTypeRepository() : base(MockDataFactory.OperationTypes) { }

        // Implement specific IProductionOrderItemRepository methods here if any
    }

    public class MockWorkCenterOperationRouteRepository : MockBaseRepository<WorkCenterOperationRoute>, IWorkCenterOperationRouteRepository
    {
        public MockWorkCenterOperationRouteRepository() : base(MockDataFactory.WorkCenterOperationRoutes) { }

        // Implement specific IProductionOrderItemRepository methods here if any
    }

    public class MockLineWorkCenterRouteRepository : MockBaseRepository<LineWorkCenterRoute>, ILineWorkCenterRouteRepository
    {
        public MockLineWorkCenterRouteRepository() : base(MockDataFactory.LineWorkCenterRoutes) { }

        // Implement specific IProductionOrderItemRepository methods here if any
    }

    public class MockProductAvailablePerLineRepository : MockBaseRepository<ProductAvailablePerLine>, IProductAvailablePerLineRepository
    {
        public MockProductAvailablePerLineRepository() : base(MockDataFactory.ProductsAvailablePerLines) { }

        // Implement specific IProductionOrderItemRepository methods here if any
    }

    public class MockProductOperationRouteRepository : MockBaseRepository<ProductOperationRoute>, IProductOperationRouteRepository
    {
        public MockProductOperationRouteRepository() : base(MockDataFactory.ProductOperationRoutes) { }

        // Implement specific IProductionOrderItemRepository methods here if any
    }

}
