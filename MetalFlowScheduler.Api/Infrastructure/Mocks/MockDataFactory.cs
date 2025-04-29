using MetalFlowScheduler.Api.Application.DTOs;
using MetalFlowScheduler.Api.Domain.Entities;

namespace MetalFlowScheduler.Api.Infrastructure.Mocks
{
    public class MockDataFactory
    {
        // Static lists to hold mock data
        // Listas estáticas para armazenar dados mockados
        public static List<OperationType> OperationTypes { get; private set; } = new List<OperationType>();
        public static List<Operation> Operations { get; private set; } = new List<Operation>();
        public static List<WorkCenter> WorkCenters { get; private set; } = new List<WorkCenter>();
        public static List<Line> Lines { get; private set; } = new List<Line>();
        public static List<WorkCenterOperationRoute> WorkCenterOperationRoutes { get; private set; } = new List<WorkCenterOperationRoute>();
        public static List<LineWorkCenterRoute> LineWorkCenterRoutes { get; private set; } = new List<LineWorkCenterRoute>();
        public static List<ProductAvailablePerLine> ProductsAvailablePerLines { get; private set; } = new List<ProductAvailablePerLine>();
        public static List<Product> Products { get; private set; } = new List<Product>();
        public static List<ProductOperationRoute> ProductOperationRoutes { get; private set; } = new List<ProductOperationRoute>();
        public static List<SurplusPerProductAndWorkCenter> SurplusStocks { get; private set; } = new List<SurplusPerProductAndWorkCenter>();
        public static List<ProductionOrder> ProductionOrders { get; private set; } = new List<ProductionOrder>();
        public static List<ProductionOrderItem> ProductionOrderItems { get; private set; } = new List<ProductionOrderItem>();

        private static bool _isInitialized = false;
        private static readonly object _lock = new object();

        /// <summary>
        /// Initializes the mock data if it hasn't been initialized yet.
        /// Thread-safe initialization.
        /// Inicializa os dados mockados se ainda não foram inicializados.
        /// Inicialização segura para threads.
        /// </summary>
        public static void Initialize()
        {
            lock (_lock)
            {
                if (_isInitialized) return;

                SeedData();
                _isInitialized = true;
            }
        }

        /// <summary>
        /// Clears all mock data lists. Useful for resetting between tests.
        /// Limpa todas as listas de dados mockados. Útil para redefinir entre testes.
        /// </summary>
        public static void Reset()
        {
            lock (_lock)
            {
                OperationTypes.Clear();
                Operations.Clear();
                WorkCenters.Clear();
                Lines.Clear();
                WorkCenterOperationRoutes.Clear();
                LineWorkCenterRoutes.Clear();
                ProductsAvailablePerLines.Clear();
                Products.Clear();
                ProductOperationRoutes.Clear();
                SurplusStocks.Clear();
                ProductionOrders.Clear();
                ProductionOrderItems.Clear();
                _isInitialized = false;
            }
        }


        /// <summary>
        /// Populates the static lists with sample mock data.
        /// Preenche as listas estáticas com dados mockados de exemplo.
        /// </summary>
        private static void SeedData()
        {
            // --- Tipos de Operação ---
            var opTypeFusao = new OperationType { ID = 1, Name = "Fusão" };
            var opTypeRefino = new OperationType { ID = 2, Name = "Refino" };
            var opTypeLaminacao = new OperationType { ID = 3, Name = "Laminação" };
            var opTypeCorte = new OperationType { ID = 4, Name = "Corte" };
            OperationTypes.AddRange(new[] { opTypeFusao, opTypeRefino, opTypeLaminacao, opTypeCorte });

            // --- Linhas ---
            var linha1 = new Line { ID = 10, Name = "Linha Principal" };
            var linha2 = new Line { ID = 20, Name = "Linha Secundária" };
            Lines.AddRange(new[] { linha1, linha2 });

            // --- Work Centers ---
            var wcAciariaL1 = new WorkCenter { ID = 101, Name = "Aciaria L1", LineID = linha1.ID, OptimalBatch = 50, Line = linha1 };
            var wcLaminacaoL1 = new WorkCenter { ID = 102, Name = "Laminação L1", LineID = linha1.ID, OptimalBatch = 30, Line = linha1 };
            var wcCorteL1 = new WorkCenter { ID = 103, Name = "Corte L1", LineID = linha1.ID, OptimalBatch = 10, Line = linha1 };
            var wcAciariaL2 = new WorkCenter { ID = 201, Name = "Aciaria L2", LineID = linha2.ID, OptimalBatch = 40, Line = linha2 };
            var wcRefinoL2 = new WorkCenter { ID = 202, Name = "Refino L2", LineID = linha2.ID, OptimalBatch = 20, Line = linha2 };
            WorkCenters.AddRange(new[] { wcAciariaL1, wcLaminacaoL1, wcCorteL1, wcAciariaL2, wcRefinoL2 });

            // --- Operações (Máquinas) ---
            // Aciaria L1
            var opForno1L1 = new Operation { ID = 1011, Name = "Forno 01 L1", WorkCenterID = wcAciariaL1.ID, OperationTypeID = opTypeFusao.ID, Capacity = 10, SetupTimeInMinutes = 60, WorkCenter = wcAciariaL1, OperationType = opTypeFusao };
            var opForno2L1 = new Operation { ID = 1012, Name = "Forno 02 L1", WorkCenterID = wcAciariaL1.ID, OperationTypeID = opTypeFusao.ID, Capacity = 12, SetupTimeInMinutes = 75, WorkCenter = wcAciariaL1, OperationType = opTypeFusao };
            // Laminação L1
            var opLam1L1 = new Operation { ID = 1021, Name = "Laminador 01 L1", WorkCenterID = wcLaminacaoL1.ID, OperationTypeID = opTypeLaminacao.ID, Capacity = 20, SetupTimeInMinutes = 30, WorkCenter = wcLaminacaoL1, OperationType = opTypeLaminacao };
            // Corte L1
            var opCort1L1 = new Operation { ID = 1031, Name = "Corte 01 L1", WorkCenterID = wcCorteL1.ID, OperationTypeID = opTypeCorte.ID, Capacity = 50, SetupTimeInMinutes = 15, WorkCenter = wcCorteL1, OperationType = opTypeCorte };
            // Aciaria L2
            var opForno1L2 = new Operation { ID = 2011, Name = "Forno 01 L2", WorkCenterID = wcAciariaL2.ID, OperationTypeID = opTypeFusao.ID, Capacity = 8, SetupTimeInMinutes = 90, WorkCenter = wcAciariaL2, OperationType = opTypeFusao };
            // Refino L2
            var opRef1L2 = new Operation { ID = 2021, Name = "Refino 01 L2", WorkCenterID = wcRefinoL2.ID, OperationTypeID = opTypeRefino.ID, Capacity = 15, SetupTimeInMinutes = 45, WorkCenter = wcRefinoL2, OperationType = opTypeRefino };
            Operations.AddRange(new[] { opForno1L1, opForno2L1, opLam1L1, opCort1L1, opForno1L2, opRef1L2 });

            // --- Produtos ---
            var prodA = new Product { ID = 1001, Name = "Liga A", UnitPricePerTon = 1200, ProfitMargin = 0.15m, Priority = 1, PenalityCost = 50 };
            var prodB = new Product { ID = 1002, Name = "Liga B", UnitPricePerTon = 1500, ProfitMargin = 0.20m, Priority = 2, PenalityCost = 70 };
            var prodC = new Product { ID = 1003, Name = "Liga C", UnitPricePerTon = 1000, ProfitMargin = 0.10m, Priority = 1, PenalityCost = 40 };
            Products.AddRange(new[] { prodA, prodB, prodC });

            // --- Rotas de Produto (Simplificado - apenas tipos de operação) ---
            // Produto A: Fusao -> Laminação -> Corte
            ProductOperationRoutes.Add(new ProductOperationRoute { ID = 501, ProductID = prodA.ID, OperationTypeID = opTypeFusao.ID, Order = 1, Version = 1, EffectiveStartDate = DateTime.MinValue, Product = prodA, OperationType = opTypeFusao });
            ProductOperationRoutes.Add(new ProductOperationRoute { ID = 502, ProductID = prodA.ID, OperationTypeID = opTypeLaminacao.ID, Order = 2, Version = 1, EffectiveStartDate = DateTime.MinValue, Product = prodA, OperationType = opTypeLaminacao });
            ProductOperationRoutes.Add(new ProductOperationRoute { ID = 503, ProductID = prodA.ID, OperationTypeID = opTypeCorte.ID, Order = 3, Version = 1, EffectiveStartDate = DateTime.MinValue, Product = prodA, OperationType = opTypeCorte });
            // Produto B: Fusao -> Refino
            ProductOperationRoutes.Add(new ProductOperationRoute { ID = 504, ProductID = prodB.ID, OperationTypeID = opTypeFusao.ID, Order = 1, Version = 1, EffectiveStartDate = DateTime.MinValue, Product = prodB, OperationType = opTypeFusao });
            ProductOperationRoutes.Add(new ProductOperationRoute { ID = 505, ProductID = prodB.ID, OperationTypeID = opTypeRefino.ID, Order = 2, Version = 1, EffectiveStartDate = DateTime.MinValue, Product = prodB, OperationType = opTypeRefino });
            // Produto C: Fusao -> Laminação
            ProductOperationRoutes.Add(new ProductOperationRoute { ID = 506, ProductID = prodC.ID, OperationTypeID = opTypeFusao.ID, Order = 1, Version = 1, EffectiveStartDate = DateTime.MinValue, Product = prodC, OperationType = opTypeFusao });
            ProductOperationRoutes.Add(new ProductOperationRoute { ID = 507, ProductID = prodC.ID, OperationTypeID = opTypeLaminacao.ID, Order = 2, Version = 1, EffectiveStartDate = DateTime.MinValue, Product = prodC, OperationType = opTypeLaminacao });


            // --- Rotas de Linha (Work Centers) ---
            // Linha 1: Aciaria -> Laminação -> Corte
            LineWorkCenterRoutes.Add(new LineWorkCenterRoute { ID = 601, LineID = linha1.ID, WorkCenterID = wcAciariaL1.ID, Order = 1, Version = 1, EffectiveStartDate = DateTime.MinValue, TransportTimeInMinutes = 15, Line = linha1, WorkCenter = wcAciariaL1 });
            LineWorkCenterRoutes.Add(new LineWorkCenterRoute { ID = 602, LineID = linha1.ID, WorkCenterID = wcLaminacaoL1.ID, Order = 2, Version = 1, EffectiveStartDate = DateTime.MinValue, TransportTimeInMinutes = 10, Line = linha1, WorkCenter = wcLaminacaoL1 });
            LineWorkCenterRoutes.Add(new LineWorkCenterRoute { ID = 603, LineID = linha1.ID, WorkCenterID = wcCorteL1.ID, Order = 3, Version = 1, EffectiveStartDate = DateTime.MinValue, TransportTimeInMinutes = 5, Line = linha1, WorkCenter = wcCorteL1 });
            // Linha 2: Aciaria -> Refino
            LineWorkCenterRoutes.Add(new LineWorkCenterRoute { ID = 604, LineID = linha2.ID, WorkCenterID = wcAciariaL2.ID, Order = 1, Version = 1, EffectiveStartDate = DateTime.MinValue, TransportTimeInMinutes = 20, Line = linha2, WorkCenter = wcAciariaL2 });
            LineWorkCenterRoutes.Add(new LineWorkCenterRoute { ID = 605, LineID = linha2.ID, WorkCenterID = wcRefinoL2.ID, Order = 2, Version = 1, EffectiveStartDate = DateTime.MinValue, TransportTimeInMinutes = 10, Line = linha2, WorkCenter = wcRefinoL2 });

            // --- Rotas de Work Center (Tipos de Operação) ---
            // Aciaria L1: Fusao
            WorkCenterOperationRoutes.Add(new WorkCenterOperationRoute { ID = 701, WorkCenterID = wcAciariaL1.ID, OperationTypeID = opTypeFusao.ID, Order = 1, Version = 1, EffectiveStartDate = DateTime.MinValue, TransportTimeInMinutes = 5, WorkCenter = wcAciariaL1, OperationType = opTypeFusao });
            // Laminação L1: Laminação
            WorkCenterOperationRoutes.Add(new WorkCenterOperationRoute { ID = 702, WorkCenterID = wcLaminacaoL1.ID, OperationTypeID = opTypeLaminacao.ID, Order = 1, Version = 1, EffectiveStartDate = DateTime.MinValue, TransportTimeInMinutes = 5, WorkCenter = wcLaminacaoL1, OperationType = opTypeLaminacao });
            // Corte L1: Corte
            WorkCenterOperationRoutes.Add(new WorkCenterOperationRoute { ID = 703, WorkCenterID = wcCorteL1.ID, OperationTypeID = opTypeCorte.ID, Order = 1, Version = 1, EffectiveStartDate = DateTime.MinValue, TransportTimeInMinutes = 0, WorkCenter = wcCorteL1, OperationType = opTypeCorte });
            // Aciaria L2: Fusao
            WorkCenterOperationRoutes.Add(new WorkCenterOperationRoute { ID = 704, WorkCenterID = wcAciariaL2.ID, OperationTypeID = opTypeFusao.ID, Order = 1, Version = 1, EffectiveStartDate = DateTime.MinValue, TransportTimeInMinutes = 5, WorkCenter = wcAciariaL2, OperationType = opTypeFusao });
            // Refino L2: Refino
            WorkCenterOperationRoutes.Add(new WorkCenterOperationRoute { ID = 705, WorkCenterID = wcRefinoL2.ID, OperationTypeID = opTypeRefino.ID, Order = 1, Version = 1, EffectiveStartDate = DateTime.MinValue, TransportTimeInMinutes = 0, WorkCenter = wcRefinoL2, OperationType = opTypeRefino });


            // --- Disponibilidade de Produto por Linha ---
            ProductsAvailablePerLines.Add(new ProductAvailablePerLine { ID = 801, ProductID = prodA.ID, LineID = linha1.ID, Product = prodA, Line = linha1 });
            ProductsAvailablePerLines.Add(new ProductAvailablePerLine { ID = 802, ProductID = prodB.ID, LineID = linha2.ID, Product = prodB, Line = linha2 });
            ProductsAvailablePerLines.Add(new ProductAvailablePerLine { ID = 803, ProductID = prodC.ID, LineID = linha1.ID, Product = prodC, Line = linha1 });
            ProductsAvailablePerLines.Add(new ProductAvailablePerLine { ID = 804, ProductID = prodC.ID, LineID = linha2.ID, Product = prodC, Line = linha2 }); // Prod C pode em ambas

            // --- Estoque Surplus Inicial ---
            SurplusStocks.Add(new SurplusPerProductAndWorkCenter { ID = 901, ProductID = prodA.ID, WorkCenterID = wcAciariaL1.ID, Surplus = 15, Product = prodA, WorkCenter = wcAciariaL1 });
            SurplusStocks.Add(new SurplusPerProductAndWorkCenter { ID = 902, ProductID = prodC.ID, WorkCenterID = wcAciariaL2.ID, Surplus = 5, Product = prodC, WorkCenter = wcAciariaL2 });

            // --- Ordens de Produção e Itens ---
            var now = DateTime.UtcNow;
            var order1 = new ProductionOrder { ID = 10001, OrderNumber = "PO-001", EarliestStartDate = now.AddHours(1), Deadline = now.AddDays(3) };
            var order2 = new ProductionOrder { ID = 10002, OrderNumber = "PO-002", EarliestStartDate = now.AddHours(2), Deadline = now.AddDays(2) };
            ProductionOrders.AddRange(new[] { order1, order2 });

            var item1_1 = new ProductionOrderItem { ID = 20001, ProductionOrderID = order1.ID, ProductID = prodA.ID, Quantity = 80, ProductionOrder = order1, Product = prodA };
            var item1_2 = new ProductionOrderItem { ID = 20002, ProductionOrderID = order1.ID, ProductID = prodC.ID, Quantity = 50, ProductionOrder = order1, Product = prodC };
            var item2_1 = new ProductionOrderItem { ID = 20003, ProductionOrderID = order2.ID, ProductID = prodB.ID, Quantity = 60, ProductionOrder = order2, Product = prodB };
            ProductionOrderItems.AddRange(new[] { item1_1, item1_2, item2_1 });

            // --- Relacionamentos Inversos (importante para navegação nos mocks) ---
            // Preencher listas de navegação inversa nas entidades mockadas
            foreach (var line in Lines)
            {
                line.WorkCenters = WorkCenters.Where(wc => wc.LineID == line.ID).ToList();
                line.WorkCenterRoutes = LineWorkCenterRoutes.Where(lwr => lwr.LineID == line.ID).ToList();
                line.AvailableProducts = ProductsAvailablePerLines.Where(pal => pal.LineID == line.ID).ToList();
            }
            foreach (var wc in WorkCenters)
            {
                wc.Operations = Operations.Where(op => op.WorkCenterID == wc.ID).ToList();
                wc.LineRoutes = LineWorkCenterRoutes.Where(lwr => lwr.WorkCenterID == wc.ID).ToList();
                wc.OperationRoutes = WorkCenterOperationRoutes.Where(wor => wor.WorkCenterID == wc.ID).ToList();
                wc.SurplusStocks = SurplusStocks.Where(s => s.WorkCenterID == wc.ID).ToList();
                // wc.Line já foi setado na criação
            }
            foreach (var op in Operations)
            {
                op.OperationType = OperationTypes.FirstOrDefault(ot => ot.ID == op.OperationTypeID);
                op.WorkCenter = WorkCenters.FirstOrDefault(wc => wc.ID == op.WorkCenterID);
            }
            foreach (var opType in OperationTypes)
            {
                opType.Operations = Operations.Where(op => op.OperationTypeID == opType.ID).ToList();
                opType.WorkCenterRoutes = WorkCenterOperationRoutes.Where(wor => wor.OperationTypeID == opType.ID).ToList();
                opType.ProductRoutes = ProductOperationRoutes.Where(por => por.OperationTypeID == opType.ID).ToList();
            }
            foreach (var prod in Products)
            {
                prod.OperationRoutes = ProductOperationRoutes.Where(por => por.ProductID == prod.ID).ToList();
                prod.AvailableOnLines = ProductsAvailablePerLines.Where(pal => pal.ProductID == prod.ID).ToList();
                prod.ProductionOrderItems = ProductionOrderItems.Where(poi => poi.ProductID == prod.ID).ToList();
                prod.SurplusStocks = SurplusStocks.Where(s => s.ProductID == prod.ID).ToList();
            }
            foreach (var po in ProductionOrders)
            {
                po.Items = ProductionOrderItems.Where(poi => poi.ProductionOrderID == po.ID).ToList();
            }
            foreach (var poi in ProductionOrderItems)
            {
                poi.ProductionOrder = ProductionOrders.FirstOrDefault(po => po.ID == poi.ProductionOrderID);
                poi.Product = Products.FirstOrDefault(p => p.ID == poi.ProductID);
            }
            // Continuar para outras entidades e relacionamentos (Rotas, Surplus, etc.)
            foreach (var lwr in LineWorkCenterRoutes)
            {
                lwr.Line = Lines.FirstOrDefault(l => l.ID == lwr.LineID);
                lwr.WorkCenter = WorkCenters.FirstOrDefault(wc => wc.ID == lwr.WorkCenterID);
            }
            foreach (var wor in WorkCenterOperationRoutes)
            {
                wor.WorkCenter = WorkCenters.FirstOrDefault(wc => wc.ID == wor.WorkCenterID);
                wor.OperationType = OperationTypes.FirstOrDefault(ot => ot.ID == wor.OperationTypeID);
            }
            foreach (var por in ProductOperationRoutes)
            {
                por.Product = Products.FirstOrDefault(p => p.ID == por.ProductID);
                por.OperationType = OperationTypes.FirstOrDefault(ot => ot.ID == por.OperationTypeID);
            }
            foreach (var pal in ProductsAvailablePerLines)
            {
                pal.Line = Lines.FirstOrDefault(l => l.ID == pal.LineID);
                pal.Product = Products.FirstOrDefault(p => p.ID == pal.ProductID);
            }
            foreach (var sp in SurplusStocks)
            {
                sp.Product = Products.FirstOrDefault(p => p.ID == sp.ProductID);
                sp.WorkCenter = WorkCenters.FirstOrDefault(wc => wc.ID == sp.WorkCenterID);
            }
        }

        // --- Métodos para obter Cenários de Entrada (PlanningInputDto) ---

        /// <summary>
        /// Retorna um DTO de entrada para um cenário de planejamento simples.
        /// Exemplo: Apenas a primeira ordem mockada, com um horizonte razoável.
        /// </summary>
        private static PlanningInputDto GetSimpleScenarioInput()
        {
            Initialize(); // Garante que os dados existem
            var orderId = 10001; // ID da primeira ordem mockada
            var order = ProductionOrders.FirstOrDefault(o => o.ID == orderId);
            if (order == null) return new PlanningInputDto(); // Ou lançar exceção

            return new PlanningInputDto
            {
                ProductionOrderIds = new List<int> { orderId },
                HorizonStartDate = order.EarliestStartDate.AddMinutes(-15), // Começa um pouco antes
                HorizonEndDate = order.Deadline.AddHours(4) // Termina um pouco depois
            };
        }

        /// <summary>
        /// Retorna um DTO de entrada para um cenário de planejamento médio.
        /// Exemplo: As duas primeiras ordens mockadas, com um horizonte que as abrange.
        /// </summary>
        private static PlanningInputDto GetMediumScenarioInput()
        {
            Initialize();
            var orderIds = new List<int> { 10001, 10002 };
            var orders = ProductionOrders.Where(o => orderIds.Contains(o.ID)).ToList();
            if (orders.Count != orderIds.Count) return new PlanningInputDto(); // Alguma ordem não encontrada

            var minStart = orders.Min(o => o.EarliestStartDate);
            var maxDeadline = orders.Max(o => o.Deadline);

            return new PlanningInputDto
            {
                ProductionOrderIds = orderIds,
                HorizonStartDate = minStart.AddMinutes(-30),
                HorizonEndDate = maxDeadline.AddHours(8) // Mais folga
            };
        }

        /// <summary>
        /// Retorna um DTO de entrada para um cenário de planejamento difícil nível 1.
        /// Exemplo: Todas as ordens, com um horizonte mais apertado em relação aos deadlines.
        /// </summary>
        private static PlanningInputDto GetHardLevel1ScenarioInput()
        {
            Initialize();
            var orderIds = ProductionOrders.Select(o => o.ID).ToList(); // Todas as ordens
            if (!orderIds.Any()) return new PlanningInputDto();

            var minStart = ProductionOrders.Min(o => o.EarliestStartDate);
            // Horizonte termina exatamente no último deadline, forçando o solver
            var maxDeadline = ProductionOrders.Max(o => o.Deadline);

            return new PlanningInputDto
            {
                ProductionOrderIds = orderIds,
                HorizonStartDate = minStart.AddMinutes(-5), // Começa quase junto
                HorizonEndDate = maxDeadline // Termina exatamente no último deadline
            };
        }

        /// <summary>
        /// Retorna um DTO de entrada para um cenário de planejamento difícil nível 2.
        /// Exemplo: Todas as ordens, com um horizonte muito curto, provavelmente inviável.
        /// </summary>
        private static PlanningInputDto GetHardLevel2ScenarioInput()
        {
            Initialize();
            var orderIds = ProductionOrders.Select(o => o.ID).ToList();
            if (!orderIds.Any()) return new PlanningInputDto();

            var minStart = ProductionOrders.Min(o => o.EarliestStartDate);
            // Horizonte muito curto, provavelmente antes do deadline de algumas ordens
            var shortDeadline = ProductionOrders.OrderBy(o => o.Deadline).Skip(ProductionOrders.Count / 2).First().Deadline;


            return new PlanningInputDto
            {
                ProductionOrderIds = orderIds,
                HorizonStartDate = minStart,
                HorizonEndDate = shortDeadline.AddHours(-6) // Termina bem antes do necessário para algumas ordens
            };
        }

        /// <summary>
        /// Obtém um cenário de entrada de planejamento com base em um ID.
        /// </summary>
        /// <param name="scenarioId">1: Simples, 2: Médio, 3: Difícil 1, 4: Difícil 2</param>
        /// <returns>O PlanningInputDto correspondente ou null se o ID for inválido.</returns>
        public static PlanningInputDto? GetScenarioInputById(int scenarioId)
        {
            switch (scenarioId)
            {
                case 1: return GetSimpleScenarioInput();
                case 2: return GetMediumScenarioInput();
                case 3: return GetHardLevel1ScenarioInput();
                case 4: return GetHardLevel2ScenarioInput();
                default: return null; // Ou lançar ArgumentOutOfRangeException
            }
        }
    }
}
