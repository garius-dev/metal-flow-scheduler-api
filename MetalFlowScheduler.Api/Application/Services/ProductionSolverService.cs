using Google.OrTools.Sat;
using MetalFlowScheduler.Api.Application.DTOs;
using MetalFlowScheduler.Api.Domain.Entities;
using MetalFlowScheduler.Api.Interfaces.Repositories;
using MetalFlowScheduler.Api.Interfaces.Services;
using System.Diagnostics;

namespace MetalFlowScheduler.Api.Application.Services
{
    #region Helper Structures for Planning

    // --- OrderItemPlanningInfo (Inalterado da V3) ---
    internal class OrderItemPlanningInfo
    {
        public ProductionOrderItem OrderItem { get; }
        public Product Product { get; }
        public List<ProductOperationRoute> ProductRoute { get; } = new List<ProductOperationRoute>();
        public WorkCenter? FirstRelevantWorkCenter { get; set; }
        public decimal InitialSurplusAtFirstWC { get; set; } = 0m;
        public decimal NetDemand { get; set; } = 0m;
        public int NumberOfRuns { get; set; } = 0; // Quantos runs são *necessários*
        public decimal ProducedQuantityInRuns { get; set; } = 0m;
        public decimal GeneratedSurplus { get; set; } = 0m;
        public decimal ConsumedSurplus => InitialSurplusAtFirstWC;
        public OrderItemPlanningInfo(ProductionOrderItem item, Product product) { OrderItem = item; Product = product; }
    }

    // --- ItemSolverVariables (Completo) ---
    /// <summary>
    /// Mantém as variáveis de decisão do OR-Tools relacionadas a um ProductionOrderItem específico.
    /// </summary>
    internal class ItemSolverVariables
    {
        public int OrderItemId { get; }
        public BoolVar Presence { get; } // Este item está agendado?
        public List<BoolVar> LineAssignments { get; } = new List<BoolVar>(); // Uma por linha potencial
        public Dictionary<int, BoolVar> LineAssignmentLookup { get; } = new Dictionary<int, BoolVar>(); // LineId -> BoolVar
        // Estrutura: <(LineId, WcId, ProdRouteStepOrder, RunIndex), IntervalVar>
        public Dictionary<(int, int, int, int), IntervalVar> RunIntervals { get; } = new Dictionary<(int, int, int, int), IntervalVar>();
        // Estrutura: <(LineId, WcId, ProdRouteStepOrder, RunIndex, OperationId), BoolVar>
        // Indica se um run específico usa uma máquina (Operation) específica.
        public Dictionary<(int, int, int, int, int), BoolVar> MachineUsageVars { get; } = new Dictionary<(int, int, int, int, int), BoolVar>();
        public ItemSolverVariables(int orderItemId, BoolVar presence) { OrderItemId = orderItemId; Presence = presence; }
    }

    #endregion


    /// <summary>
    /// Serviço responsável por orquestrar o processo de planejamento de produção usando Google OR-Tools.
    /// </summary>
    public class ProductionSolverService : IProductionSolverService
    {
        // Constantes (Inalterado)
        private const int MINUTES_PER_HOUR = 60;
        private const int TIME_SCALE_FACTOR = 1;

        // Dependências Injetadas (Inalterado)
        private readonly ILineRepository _lineRepository;
        private readonly IWorkCenterRepository _workCenterRepository;
        private readonly IOperationRepository _operationRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductionOrderRepository _productionOrderRepository;
        private readonly ISurplusPerProductAndWorkCenterRepository _surplusRepository;
        private readonly ILineWorkCenterRouteRepository _lineWorkCenterRouteRepository;
        private readonly IWorkCenterOperationRouteRepository _workCenterOperationRouteRepository;
        private readonly IProductOperationRouteRepository _productOperationRouteRepository;
        private readonly IProductAvailablePerLineRepository _productAvailablePerLineRepository;
        private readonly IProductionOrderItemRepository _productionOrderItemRepository;
        private readonly IOperationTypeRepository _operationTypeRepository;
        private readonly ILogger<ProductionSolverService> _logger;

        // Construtor (Inalterado - recebe dependências)
        public ProductionSolverService(
            ILineRepository lineRepository,
            IWorkCenterRepository workCenterRepository,
            IOperationRepository operationRepository,
            IProductRepository productRepository,
            IProductionOrderRepository productionOrderRepository,
            ISurplusPerProductAndWorkCenterRepository surplusRepository,
            ILineWorkCenterRouteRepository lineWorkCenterRouteRepository,
            IWorkCenterOperationRouteRepository workCenterOperationRouteRepository,
            IProductOperationRouteRepository productOperationRouteRepository,
            IProductAvailablePerLineRepository productAvailablePerLineRepository,
            IProductionOrderItemRepository productionOrderItemRepository,
            IOperationTypeRepository operationTypeRepository,
            ILogger<ProductionSolverService> logger)
        {
            _lineRepository = lineRepository ?? throw new ArgumentNullException(nameof(lineRepository));
            _workCenterRepository = workCenterRepository ?? throw new ArgumentNullException(nameof(workCenterRepository));
            _operationRepository = operationRepository ?? throw new ArgumentNullException(nameof(operationRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _productionOrderRepository = productionOrderRepository ?? throw new ArgumentNullException(nameof(productionOrderRepository));
            _surplusRepository = surplusRepository ?? throw new ArgumentNullException(nameof(surplusRepository));
            _lineWorkCenterRouteRepository = lineWorkCenterRouteRepository ?? throw new ArgumentNullException(nameof(lineWorkCenterRouteRepository));
            _workCenterOperationRouteRepository = workCenterOperationRouteRepository ?? throw new ArgumentNullException(nameof(workCenterOperationRouteRepository));
            _productOperationRouteRepository = productOperationRouteRepository ?? throw new ArgumentNullException(nameof(productOperationRouteRepository));
            _productAvailablePerLineRepository = productAvailablePerLineRepository ?? throw new ArgumentNullException(nameof(productAvailablePerLineRepository));
            _productionOrderItemRepository = productionOrderItemRepository ?? throw new ArgumentNullException(nameof(productionOrderItemRepository));
            _operationTypeRepository = operationTypeRepository ?? throw new ArgumentNullException(nameof(operationTypeRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        /// <summary>
        /// Executa o processo de planejamento de produção com base na entrada fornecida.
        /// </summary>
        /// <param name="input">Os parâmetros de entrada do planejamento.</param>
        /// <returns>Um DTO contendo os resultados do processo de planejamento.</returns>
        public async Task<ProductionPlanResultDto> PlanProductionAsync(PlanningInputDto input)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Iniciando processo de planejamento de produção para o horizonte {StartDate} a {EndDate}.", input.HorizonStartDate, input.HorizonEndDate);
            DateTime effectiveDateForRoutes = input.HorizonStartDate;

            var planResult = new ProductionPlanResultDto { PlanOverallDeadline = input.HorizonEndDate };
            var itemPlanningInfos = new Dictionary<int, OrderItemPlanningInfo>();

            try
            {
                // --- 1. Busca de Dados & Filtragem de Rotas ---
                _logger.LogDebug("Buscando dados necessários e filtrando rotas ativas (data efetiva: {EffectiveDate})...", effectiveDateForRoutes);
                // Código completo restaurado, sem placeholders
                var fetchDataTasks = new
                {
                    Lines = _lineRepository.GetAllEnabledAsync(),
                    WorkCenters = _workCenterRepository.GetAllEnabledAsync(),
                    Operations = _operationRepository.GetAllEnabledAsync(),
                    Products = _productRepository.GetAllEnabledAsync(),
                    Surplus = _surplusRepository.GetAllEnabledAsync(),
                    LineWcRoutes = _lineWorkCenterRouteRepository.GetAllEnabledAsync(),
                    WcOpRoutes = _workCenterOperationRouteRepository.GetAllEnabledAsync(),
                    ProdOpRoutes = _productOperationRouteRepository.GetAllEnabledAsync(),
                    ProdAvail = _productAvailablePerLineRepository.GetAllEnabledAsync(),
                    Orders = _productionOrderRepository.GetPendingOrdersAsync()
                };
                await Task.WhenAll(
                    fetchDataTasks.Lines, fetchDataTasks.WorkCenters, fetchDataTasks.Operations,
                    fetchDataTasks.Products, fetchDataTasks.Surplus, fetchDataTasks.LineWcRoutes,
                    fetchDataTasks.WcOpRoutes, fetchDataTasks.ProdOpRoutes, fetchDataTasks.ProdAvail,
                    fetchDataTasks.Orders
                );

                var allLines = await fetchDataTasks.Lines;
                var allWorkCenters = (await fetchDataTasks.WorkCenters).ToDictionary(wc => wc.ID);
                var allOperations = await fetchDataTasks.Operations;
                var allProducts = (await fetchDataTasks.Products).ToDictionary(p => p.ID);
                var allSurplus = await fetchDataTasks.Surplus;
                var allLineWcRoutesRaw = await fetchDataTasks.LineWcRoutes;
                var allWcOpRoutesRaw = await fetchDataTasks.WcOpRoutes;
                var allProdOpRoutesRaw = await fetchDataTasks.ProdOpRoutes;
                var allProdAvail = await fetchDataTasks.ProdAvail;
                var productionOrders = await fetchDataTasks.Orders;

                // Filtrar Rotas Ativas (Código completo restaurado)
                var activeProdOpRoutes = allProdOpRoutesRaw
                   .Where(r => r.Enabled &&
                               r.EffectiveStartDate <= effectiveDateForRoutes &&
                               (r.EffectiveEndDate == null || r.EffectiveEndDate >= effectiveDateForRoutes))
                   .GroupBy(r => r.ProductID)
                   .Select(g => g.OrderByDescending(r => r.Version).First())
                   .SelectMany(latestVersionGroup => allProdOpRoutesRaw
                                   .Where(r => r.ProductID == latestVersionGroup.ProductID && r.Version == latestVersionGroup.Version && r.Enabled)
                                   .OrderBy(r => r.Order))
                   .ToList();
                var productRoutesLookup = activeProdOpRoutes.ToLookup(r => r.ProductID);

                var activeLineWcRoutes = allLineWcRoutesRaw
                    .Where(r => r.Enabled &&
                                r.EffectiveStartDate <= effectiveDateForRoutes &&
                                (r.EffectiveEndDate == null || r.EffectiveEndDate >= effectiveDateForRoutes))
                    .GroupBy(r => r.LineID)
                    .Select(g => g.OrderByDescending(r => r.Version).First())
                    .SelectMany(latestVersionGroup => allLineWcRoutesRaw
                                    .Where(r => r.LineID == latestVersionGroup.LineID && r.Version == latestVersionGroup.Version && r.Enabled)
                                    .OrderBy(r => r.Order))
                    .ToList();
                var lineRoutesLookup = activeLineWcRoutes.ToLookup(r => r.LineID);

                var activeWcOpRoutes = allWcOpRoutesRaw
                   .Where(r => r.Enabled &&
                               r.EffectiveStartDate <= effectiveDateForRoutes &&
                               (r.EffectiveEndDate == null || r.EffectiveEndDate >= effectiveDateForRoutes))
                   .GroupBy(r => r.WorkCenterID)
                   .Select(g => g.OrderByDescending(r => r.Version).First())
                   .SelectMany(latestVersionGroup => allWcOpRoutesRaw
                                   .Where(r => r.WorkCenterID == latestVersionGroup.WorkCenterID && r.Version == latestVersionGroup.Version && r.Enabled)
                                   .OrderBy(r => r.Order))
                   .ToList();
                var wcOpRoutesLookup = activeWcOpRoutes.ToLookup(r => r.WorkCenterID);

                var surplusLookup = allSurplus.ToLookup(s => (s.ProductID, s.WorkCenterID));

                // Filtrar ordens com base na entrada DTO
                if (input.ProductionOrderIds != null && input.ProductionOrderIds.Any())
                {
                    productionOrders = productionOrders.Where(po => input.ProductionOrderIds.Contains(po.ID)).ToList();
                }

                var itemsToPlan = productionOrders.SelectMany(po => po.Items).Where(i => i.Enabled).ToList();
                _logger.LogDebug("Busca de dados e filtragem de rotas completa.");
                if (!itemsToPlan.Any())
                {
                    _logger.LogWarning("Nenhum item de ordem de produção habilitado encontrado para agendar.");
                    planResult.SolverStatus = "NO_ITEMS_TO_SCHEDULE";
                    return planResult;
                }

                // --- 2. Inicialização do Modelo ---
                CpModel model = new CpModel();

                // --- 3. Definição do Horizonte ---
                DateTime horizonStartDt = input.HorizonStartDate;
                DateTime horizonEndDt = input.HorizonEndDate;
                long horizonEndMinutes = (horizonEndDt > horizonStartDt) ? (long)Math.Ceiling((horizonEndDt - horizonStartDt).TotalMinutes * TIME_SCALE_FACTOR) : 0;
                _logger.LogDebug("Horizonte de planejamento definido: Início={StartDate}, Fim={EndDate}, DuraçãoMinutos={DurationMinutes}", horizonStartDt, horizonEndDt, horizonEndMinutes);

                // --- 4. Pré-processamento ---
                _logger.LogDebug("Pré-processando itens da ordem usando rotas ativas...");
                itemPlanningInfos = new Dictionary<int, OrderItemPlanningInfo>();
                // Código completo restaurado da V16 para popular itemPlanningInfos
                foreach (var item in itemsToPlan)
                {
                    if (!allProducts.TryGetValue(item.ProductID, out var product)) { _logger.LogWarning("Produto com ID {ProductId} não encontrado para Item {OrderItemId}. Pulando item.", item.ProductID, item.ID); continue; }
                    var planningInfo = new OrderItemPlanningInfo(item, product);
                    itemPlanningInfos.Add(item.ID, planningInfo);
                    planningInfo.ProductRoute.AddRange(productRoutesLookup[item.ProductID]);
                    if (!planningInfo.ProductRoute.Any()) { _logger.LogWarning("Nenhuma rota de produção ATIVA encontrada para Produto {ProductId} (ID: {ProductIdValue}) para data {EffectiveDate}. Pulando item {OrderItemId}.", product.Name, item.ProductID, effectiveDateForRoutes, item.ID); continue; }
                    var firstProductOpType = planningInfo.ProductRoute.FirstOrDefault()?.OperationTypeID;
                    if (firstProductOpType == null) { _logger.LogWarning("Não foi possível determinar o primeiro tipo de operação para o Produto {ProductName}. Pulando item {OrderItemId}.", product.Name, item.ID); continue; }
                    var potentialLineIds = allProdAvail.Where(pa => pa.ProductID == item.ProductID && pa.Enabled).Select(pa => pa.LineID).Distinct();
                    WorkCenter? firstWcForSurplusCalc = null;
                    foreach (var lineId in potentialLineIds)
                    {
                        var lineRoute = lineRoutesLookup[lineId];
                        foreach (var lineRouteStep in lineRoute)
                        {
                            var wcOpRoutes = wcOpRoutesLookup[lineRouteStep.WorkCenterID];
                            if (wcOpRoutes.Any(wor => wor.OperationTypeID == firstProductOpType))
                            {
                                if (allWorkCenters.TryGetValue(lineRouteStep.WorkCenterID, out var wc))
                                {
                                    firstWcForSurplusCalc = wc; break;
                                }
                            }
                        }
                        if (firstWcForSurplusCalc != null) break;
                    }
                    if (firstWcForSurplusCalc == null)
                    {
                        _logger.LogWarning("Não foi possível encontrar um Work Center inicial realizando Operation Type ID {OpTypeId} para Produto {ProductId} usando rotas ATIVAS para data {EffectiveDate}. Não é possível calcular surplus/runs inicial para Item {OrderItemId}.", firstProductOpType, product.Name, effectiveDateForRoutes, item.ID);
                        planningInfo.NetDemand = item.Quantity;
                        planningInfo.NumberOfRuns = (item.Quantity > 0) ? 1 : 0;
                        planningInfo.ProducedQuantityInRuns = item.Quantity;
                        planningInfo.GeneratedSurplus = 0;
                    }
                    else
                    {
                        planningInfo.FirstRelevantWorkCenter = firstWcForSurplusCalc;
                        planningInfo.InitialSurplusAtFirstWC = surplusLookup[(item.ProductID, firstWcForSurplusCalc.ID)].Sum(s => s.Surplus);
                        planningInfo.NetDemand = Math.Max(0, item.Quantity - planningInfo.InitialSurplusAtFirstWC);
                        if (planningInfo.NetDemand > 0)
                        {
                            if (firstWcForSurplusCalc.OptimalBatch > 0)
                            {
                                planningInfo.NumberOfRuns = (int)Math.Ceiling(planningInfo.NetDemand / firstWcForSurplusCalc.OptimalBatch);
                                planningInfo.ProducedQuantityInRuns = planningInfo.NumberOfRuns * firstWcForSurplusCalc.OptimalBatch;
                            }
                            else { _logger.LogWarning("OptimalBatch é zero ou menor para Work Center {WcName}. Assumindo um run para Item {OrderItemId}.", firstWcForSurplusCalc.Name, item.ID); planningInfo.NumberOfRuns = 1; planningInfo.ProducedQuantityInRuns = planningInfo.NetDemand; }
                        }
                        else { planningInfo.NumberOfRuns = 0; planningInfo.ProducedQuantityInRuns = 0; }
                        planningInfo.GeneratedSurplus = Math.Max(0, planningInfo.ProducedQuantityInRuns - planningInfo.NetDemand);
                        _logger.LogDebug("Item {OrderItemId} (Produto: {ProductName}): Demanda={Demand}, SurplusInicial={Surplus}, DemandaLiquida={NetDemand}, Runs={Runs}, QtdProduzida={ProducedQty}, SurplusGerado={GenSurplus} (baseado em WC {WcName})", item.ID, product.Name, item.Quantity, planningInfo.InitialSurplusAtFirstWC, planningInfo.NetDemand, planningInfo.NumberOfRuns, planningInfo.ProducedQuantityInRuns, planningInfo.GeneratedSurplus, firstWcForSurplusCalc.Name);
                    }
                } // Fim foreach item
                _logger.LogDebug("Pré-processamento completo para {ItemCount} itens.", itemPlanningInfos.Count);


                // --- 5. Criação de Variáveis (Variáveis Completas) ---
                _logger.LogInformation("Criando variáveis do solver (Variáveis Completas)...");
                var stopwatchVariables = Stopwatch.StartNew();

                var solverVars = new Dictionary<int, ItemSolverVariables>();
                // Dicionário para agrupar intervalos por máquina para NoOverlap
                var machineIntervals = new Dictionary<int, List<IntervalVar>>();

                foreach (var planningInfoKvp in itemPlanningInfos)
                {
                    int orderItemId = planningInfoKvp.Key;
                    var planningInfo = planningInfoKvp.Value;

                    if (planningInfo.ProductRoute == null || !planningInfo.ProductRoute.Any()) continue;

                    // Criar variável de presença
                    BoolVar itemPresenceVar = model.NewBoolVar($"p{orderItemId}");
                    var itemVars = new ItemSolverVariables(orderItemId, itemPresenceVar);
                    solverVars.Add(orderItemId, itemVars);

                    // Criar variáveis de atribuição de linha
                    var potentialLines = allProdAvail.Where(pa => pa.ProductID == planningInfo.Product.ID && pa.Enabled).Select(pa => pa.LineID).Distinct().ToList();
                    if (!potentialLines.Any()) { model.Add(itemPresenceVar == 0); continue; }

                    foreach (int lineId in potentialLines)
                    {
                        if (!lineRoutesLookup.Contains(lineId)) { continue; }
                        BoolVar lineAssignmentVar = model.NewBoolVar($"a{orderItemId}l{lineId}");
                        itemVars.LineAssignments.Add(lineAssignmentVar);
                        itemVars.LineAssignmentLookup.Add(lineId, lineAssignmentVar);
                    }

                    if (itemVars.LineAssignments.Any())
                    {
                        model.Add(LinearExpr.Sum(itemVars.LineAssignments) == itemPresenceVar);
                    }
                    else
                    {
                        model.Add(itemPresenceVar == 0); continue;
                    }

                    // ** BLOCO DE CRIAÇÃO DE INTERVALOS E USO DE MÁQUINA RESTAURADO **
                    if (planningInfo.NumberOfRuns > 0)
                    {
                        decimal quantityPerRun = planningInfo.ProducedQuantityInRuns / planningInfo.NumberOfRuns;

                        foreach (var lineAssignmentVar in itemVars.LineAssignments)
                        {
                            var kvp = itemVars.LineAssignmentLookup.First(pair => object.ReferenceEquals(pair.Value, lineAssignmentVar));
                            int lineId = kvp.Key;
                            var activeLineRoute = lineRoutesLookup[lineId];

                            foreach (var prodRouteStep in planningInfo.ProductRoute)
                            {
                                int operationTypeId = prodRouteStep.OperationTypeID;
                                int prodRouteStepOrder = prodRouteStep.Order;
                                WorkCenter? targetWc = null;
                                // ... (lógica para encontrar targetWc) ...
                                foreach (var lineWcStep in activeLineRoute)
                                {
                                    var wcOpRoutes = wcOpRoutesLookup[lineWcStep.WorkCenterID];
                                    if (wcOpRoutes.Any(wor => wor.OperationTypeID == operationTypeId))
                                    {
                                        if (allWorkCenters.TryGetValue(lineWcStep.WorkCenterID, out var wc)) { targetWc = wc; break; }
                                    }
                                }
                                if (targetWc == null) { /* log error */ continue; }

                                var availableMachines = allOperations
                                    .Where(op => op.WorkCenterID == targetWc.ID && op.OperationTypeID == operationTypeId && op.Enabled)
                                    .ToList();

                                if (!availableMachines.Any())
                                {
                                    model.Add(lineAssignmentVar == 0); break;
                                }

                                for (int runIdx = 0; runIdx < planningInfo.NumberOfRuns; runIdx++)
                                {
                                    // Calcular duração (simplificado)
                                    double totalCapacity = availableMachines.Sum(m => m.Capacity);
                                    if (totalCapacity <= 0) { model.Add(lineAssignmentVar == 0); goto NextLineAssignment; }
                                    long runDurationMinutes = (long)Math.Ceiling((double)quantityPerRun / totalCapacity * MINUTES_PER_HOUR);
                                    long runDuration = runDurationMinutes * TIME_SCALE_FACTOR;
                                    if (runDuration <= 0 && quantityPerRun > 0) runDuration = TIME_SCALE_FACTOR;

                                    // Criar IntervalVar (Opcional)
                                    string intervalName = $"r{orderItemId}l{lineId}w{targetWc.ID}s{prodRouteStepOrder}n{runIdx}";
                                    IntVar startVar = model.NewIntVar(0, horizonEndMinutes, $"{intervalName}s");
                                    IntervalVar runInterval = model.NewOptionalFixedSizeIntervalVar(startVar, runDuration, lineAssignmentVar, intervalName);
                                    itemVars.RunIntervals.Add((lineId, targetWc.ID, prodRouteStepOrder, runIdx), runInterval);

                                    // ** BLOCO DE USO DE MÁQUINA RESTAURADO **
                                    var machineUsageVarsForRun = new List<BoolVar>();
                                    foreach (var machine in availableMachines)
                                    {
                                        BoolVar machineUsageVar = model.NewBoolVar($"{intervalName}m{machine.ID}");
                                        itemVars.MachineUsageVars.Add((lineId, targetWc.ID, prodRouteStepOrder, runIdx, machine.ID), machineUsageVar);
                                        machineUsageVarsForRun.Add(machineUsageVar);

                                        // Adicionar à lista global de intervalos da máquina
                                        if (!machineIntervals.ContainsKey(machine.ID))
                                        {
                                            machineIntervals[machine.ID] = new List<IntervalVar>();
                                        }
                                        // ** CORRIGIDO: Usar runDuration (long) em vez de runInterval.SizeExpr() **
                                        IntervalVar machineSpecificInterval = model.NewOptionalFixedSizeIntervalVar(
                                            runInterval.StartExpr(), // Start (LinearExpr)
                                            runDuration,             // Size (long) - CORRIGIDO
                                            machineUsageVar,         // Presence Literal (BoolVar)
                                            $"{intervalName}m{machine.ID}_int"); // Name (string)
                                        machineIntervals[machine.ID].Add(machineSpecificInterval);
                                    }
                                    // Restrição: Cada run usa exatamente uma máquina
                                    model.Add(LinearExpr.Sum(machineUsageVarsForRun) == 1).OnlyEnforceIf(lineAssignmentVar);
                                    // ** FIM DO BLOCO DE USO DE MÁQUINA RESTAURADO **

                                } // Fim for runIdx
                            } // Fim foreach prodRouteStep
                        } // Fim foreach lineAssignmentVar
                    NextLineAssignment:;
                    } // Fim if NumberOfRuns > 0
                } // Fim foreach planningInfoKvp

                stopwatchVariables.Stop();
                _logger.LogInformation("Criação de variáveis do solver (Variáveis Completas) completa em {ElapsedMilliseconds} ms.", stopwatchVariables.ElapsedMilliseconds);

                // --- 6. Definição de Restrições ---
                _logger.LogInformation("Adicionando restrições ao modelo...");
                var stopwatchConstraints = Stopwatch.StartNew();

                // 6.1 Precedências e Tempos de Transporte
                foreach (var kvp in solverVars)
                {
                    int orderItemId = kvp.Key;
                    var itemVars = kvp.Value;
                    if (!itemPlanningInfos.TryGetValue(orderItemId, out var planningInfo)) continue; // Segurança
                    if (planningInfo.NumberOfRuns <= 0) continue;

                    foreach (var lineAssignmentVar in itemVars.LineAssignments)
                    {
                        int lineId = itemVars.LineAssignmentLookup.First(pair => object.ReferenceEquals(pair.Value, lineAssignmentVar)).Key;
                        var activeLineRoute = lineRoutesLookup[lineId].ToList();
                        var activeProdRoute = planningInfo.ProductRoute;

                        WorkCenter? previousStepWc = null;
                        LineWorkCenterRoute? previousLineWcRouteStep = null;

                        for (int i = 0; i < activeProdRoute.Count; i++)
                        {
                            var currentProdStep = activeProdRoute[i];
                            int currentOpTypeId = currentProdStep.OperationTypeID;
                            int currentProdStepOrder = currentProdStep.Order;

                            WorkCenter? currentWc = null;
                            LineWorkCenterRoute? currentLineWcRouteStep = null;
                            // ... (lógica para encontrar currentWc e currentLineWcRouteStep) ...
                            foreach (var lineWcStep in activeLineRoute)
                            {
                                var wcOpRoutes = wcOpRoutesLookup[lineWcStep.WorkCenterID];
                                if (wcOpRoutes.Any(wor => wor.OperationTypeID == currentOpTypeId))
                                {
                                    if (allWorkCenters.TryGetValue(lineWcStep.WorkCenterID, out var wc))
                                    {
                                        currentWc = wc;
                                        currentLineWcRouteStep = lineWcStep;
                                        break;
                                    }
                                }
                            }
                            if (currentWc == null || currentLineWcRouteStep == null) { /* Log Warning */ continue; }

                            if (i > 0)
                            {
                                var previousProdStep = activeProdRoute[i - 1];
                                int previousProdStepOrder = previousProdStep.Order;
                                if (previousStepWc != null)
                                {
                                    long transportTime = 0;
                                    // ... (lógica para calcular transportTime) ...
                                    if (previousStepWc.ID != currentWc.ID)
                                    {
                                        if (previousLineWcRouteStep != null) { transportTime = previousLineWcRouteStep.TransportTimeInMinutes * TIME_SCALE_FACTOR; }
                                    }
                                    else
                                    {
                                        var wcRoute = wcOpRoutesLookup[currentWc.ID].ToList();
                                        var previousWcOpRouteStep = wcRoute.FirstOrDefault(wos => wos.OperationTypeID == previousProdStep.OperationTypeID);
                                        if (previousWcOpRouteStep != null) { transportTime = previousWcOpRouteStep.TransportTimeInMinutes * TIME_SCALE_FACTOR; }
                                    }

                                    for (int runIdx = 0; runIdx < planningInfo.NumberOfRuns; runIdx++)
                                    {
                                        if (itemVars.RunIntervals.TryGetValue((lineId, currentWc.ID, currentProdStepOrder, runIdx), out var currentRunInterval) &&
                                            itemVars.RunIntervals.TryGetValue((lineId, previousStepWc.ID, previousProdStepOrder, runIdx), out var previousRunInterval))
                                        {
                                            model.Add(currentRunInterval.StartExpr() >= previousRunInterval.EndExpr() + transportTime)
                                                 .OnlyEnforceIf(lineAssignmentVar);
                                        }
                                        else { /* Log Warning */ }
                                    }
                                }
                            }
                            previousStepWc = currentWc;
                            previousLineWcRouteStep = currentLineWcRouteStep;
                        }
                    }
                }

                // 6.2 Não Sobreposição em Máquinas (NoOverlap)
                foreach (var machineId in machineIntervals.Keys)
                {
                    if (machineIntervals[machineId].Count > 1)
                    {
                        model.AddNoOverlap(machineIntervals[machineId]);
                        _logger.LogDebug("Adicionada restrição NoOverlap para Máquina ID {MachineId} com {Count} intervalos opcionais.", machineId, machineIntervals[machineId].Count);
                    }
                }

                // 6.3 Deadlines
                foreach (var kvp in solverVars)
                {
                    int orderItemId = kvp.Key;
                    var itemVars = kvp.Value;
                    if (!itemPlanningInfos.TryGetValue(orderItemId, out var planningInfo)) continue; // Segurança

                    var lastProdStep = planningInfo.ProductRoute.LastOrDefault();
                    if (lastProdStep == null || planningInfo.NumberOfRuns <= 0) continue;

                    int lastProdStepOrder = lastProdStep.Order;
                    DateTime deadline = planningInfo.OrderItem.ProductionOrder?.Deadline ?? DateTime.MaxValue;
                    long deadlineMinutes = (deadline > horizonStartDt)
                                          ? (long)Math.Ceiling((deadline - horizonStartDt).TotalMinutes * TIME_SCALE_FACTOR)
                                          : horizonEndMinutes;
                    deadlineMinutes = Math.Min(deadlineMinutes, horizonEndMinutes);
                    int lastRunIdx = planningInfo.NumberOfRuns - 1;

                    IntVar lastRunEndVar = model.NewIntVar(0, horizonEndMinutes, $"end_item_{orderItemId}");
                    model.Add(lastRunEndVar == 0).OnlyEnforceIf(itemVars.Presence.Not());

                    foreach (var lineAssignmentVar in itemVars.LineAssignments)
                    {
                        int lineId = itemVars.LineAssignmentLookup.First(pair => object.ReferenceEquals(pair.Value, lineAssignmentVar)).Key;
                        WorkCenter? lastWc = null;
                        // ... (lógica para encontrar lastWc) ...
                        foreach (var lineWcStep in lineRoutesLookup[lineId])
                        {
                            var wcOpRoutes = wcOpRoutesLookup[lineWcStep.WorkCenterID];
                            if (wcOpRoutes.Any(wor => wor.OperationTypeID == lastProdStep.OperationTypeID))
                            {
                                if (allWorkCenters.TryGetValue(lineWcStep.WorkCenterID, out var wc)) { lastWc = wc; break; }
                            }
                        }
                        if (lastWc == null) continue;

                        if (itemVars.RunIntervals.TryGetValue((lineId, lastWc.ID, lastProdStepOrder, lastRunIdx), out var lastRunInterval))
                        {
                            model.Add(lastRunEndVar == lastRunInterval.EndExpr()).OnlyEnforceIf(lineAssignmentVar);
                        }
                        else { /* Log Warning */ }
                    }
                    model.Add(lastRunEndVar <= deadlineMinutes).OnlyEnforceIf(itemVars.Presence);
                }

                // TODO: Implementar Restrições R5, R6, R7

                stopwatchConstraints.Stop();
                _logger.LogInformation("Etapa de definição de restrições (Iniciais) completa em {ElapsedMilliseconds} ms.", stopwatchConstraints.ElapsedMilliseconds);


                // --- 7. Função Objetivo ---
                _logger.LogInformation("Definindo a função objetivo...");
                // Objetivo Principal: Maximizar itens agendados, ponderado por prioridade
                List<IntVar> presences = new List<IntVar>();
                List<long> weights = new List<long>();
                long maxPriorityWeight = (long)(allProducts.Any() ? allProducts.Values.Max(p => p.Priority) + 1 : 2);

                foreach (var kvp in solverVars)
                {
                    var itemVars = kvp.Value;
                    if (!itemPlanningInfos.TryGetValue(kvp.Key, out var planningInfo)) continue; // Segurança
                    presences.Add(itemVars.Presence);
                    long weight = maxPriorityWeight - planningInfo.Product.Priority;
                    weights.Add(weight > 0 ? weight : 1);
                }

                if (presences.Any())
                {
                    model.Maximize(LinearExpr.WeightedSum(presences, weights));
                    _logger.LogDebug("Objetivo definido: Maximizar Soma Ponderada(Presença dos itens pela Prioridade)");
                }
                else { _logger.LogWarning("Nenhuma variável de solver criada, nenhum objetivo definido."); }


                // --- 8. Resolver o Modelo ---
                _logger.LogInformation("Resolvendo o modelo CP-SAT (Com Restrições Iniciais)...");

                // Validar o modelo
                var validationError = model.Validate();
                if (!string.IsNullOrEmpty(validationError))
                {
                    _logger.LogError("Falha na validação do modelo CP-SAT: {ValidationError}", validationError);
                    planResult.SolverStatus = "MODEL_INVALID";
                    foreach (var kvp in itemPlanningInfos) { AddUnscheduledItem(planResult, kvp.Value, $"Modelo inválido: {validationError}"); }
                    return planResult;
                }
                else { _logger.LogDebug("Validação do modelo CP-SAT bem-sucedida."); }

                CpSolver solver = new CpSolver();

                // StringParameters como funcionou para você
                double timeLimitInSeconds = 60.0; // Aumentar tempo com restrições
                solver.StringParameters += $"max_time_in_seconds:{timeLimitInSeconds};";
                // solver.StringParameters += "num_search_workers:auto;"; // Comentado
                // solver.StringParameters += "log_search_progress:true;"; // Comentado

                _logger.LogInformation("Chamando solver.Solve() DIRETAMENTE com limite de tempo de {TimeLimit} segundos...", timeLimitInSeconds);
                DateTime solveStartTime = DateTime.UtcNow;
                CpSolverStatus status = CpSolverStatus.Unknown;
                try
                {
                    // Chamada direta, sem Task.Run
                    status = solver.Solve(model);
                    _logger.LogCritical("!!! CHAMADA solver.Solve() DIRETA CONCLUÍDA (Com Restrições Iniciais) !!! Status retornado: {SolverStatus}", status);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "!!! EXCEÇÃO DURANTE solver.Solve() DIRETA !!!");
                    planResult.SolverStatus = "SOLVER_EXCEPTION";
                    foreach (var kvp in itemPlanningInfos) { AddUnscheduledItem(planResult, kvp.Value, $"Exceção no Solver: {ex.Message}"); }
                    return planResult;
                }

                DateTime solveEndTime = DateTime.UtcNow;
                planResult.SolverSolveTime = solveEndTime - solveStartTime;
                planResult.SolverStatus = status.ToString();
                _logger.LogInformation("Solver finalizou com status: {Status} em {SolveTime} segundos (Com Restrições Iniciais).", status, planResult.SolverSolveTime.TotalSeconds);


                // --- 9. Processar Resultados ---
                _logger.LogInformation("Processando resultados (Com Restrições Iniciais)...");
                if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
                {
                    _logger.LogInformation("Modelo com restrições iniciais resolvido com sucesso.");
                    // Código completo restaurado da V16 para processar resultados
                    planResult.ScheduledRuns = new List<ScheduledRunDetailDto>();
                    planResult.GeneratedSurplus = new List<SurplusSummaryDto>();
                    planResult.UnscheduledOrderItems = new List<UnscheduledOrderItemDto>();
                    var scheduledItems = new HashSet<int>();
                    decimal totalProfit = 0;
                    DateTime lastCompletionDate = horizonStartDt;
                    int runDetailIdCounter = 1;

                    foreach (var kvp in solverVars)
                    {
                        var itemId = kvp.Key;
                        var itemVars = kvp.Value;
                        if (!itemPlanningInfos.TryGetValue(itemId, out var planningInfo)) { continue; }

                        if (solver.Value(itemVars.Presence) == 1)
                        {
                            scheduledItems.Add(itemId);
                            _logger.LogInformation("Item {ItemId} ({ProductName}) foi agendado (presença=1).", itemId, planningInfo.Product.Name);
                            int assignedLineId = -1;
                            Line? assignedLine = null;
                            foreach (var assignmentKvp in itemVars.LineAssignmentLookup)
                            {
                                if (solver.Value(assignmentKvp.Value) == 1)
                                {
                                    assignedLineId = assignmentKvp.Key;
                                    assignedLine = allLines.FirstOrDefault(l => l.ID == assignedLineId);
                                    break;
                                }
                            }
                            _logger.LogInformation("  -> Atribuído à Linha ID: {LineId} ({LineName})", assignedLineId, assignedLine?.Name ?? "N/A");
                            totalProfit += planningInfo.OrderItem.Quantity * planningInfo.Product.UnitPricePerTon * planningInfo.Product.ProfitMargin;

                            foreach (var runIntervalKvp in itemVars.RunIntervals)
                            {
                                var key = runIntervalKvp.Key;
                                var runInterval = runIntervalKvp.Value;
                                if (key.Item1 == assignedLineId)
                                {
                                    long startVal = solver.Value(runInterval.StartExpr());
                                    long endVal = solver.Value(runInterval.EndExpr());
                                    long durationVal = solver.Value(runInterval.SizeExpr());
                                    if (startVal == 0 && endVal == 0 && durationVal > 0 && planningInfo.NumberOfRuns > 0) continue;
                                    if (startVal == endVal) continue;
                                    DateTime startTime = horizonStartDt.AddMinutes(startVal / TIME_SCALE_FACTOR);
                                    DateTime endTime = horizonStartDt.AddMinutes(endVal / TIME_SCALE_FACTOR);
                                    if (endTime > lastCompletionDate) lastCompletionDate = endTime;
                                    var wc = allWorkCenters.GetValueOrDefault(key.Item2);
                                    var opType = activeProdOpRoutes.FirstOrDefault(pr => pr.Order == key.Item3 && pr.ProductID == planningInfo.Product.ID)?.OperationType;
                                    decimal qty = (planningInfo.NumberOfRuns > 0) ? planningInfo.ProducedQuantityInRuns / planningInfo.NumberOfRuns : 0;
                                    planResult.ScheduledRuns.Add(new ScheduledRunDetailDto
                                    {
                                        RunId = runDetailIdCounter++,
                                        ProductionOrderNumber = planningInfo.OrderItem.ProductionOrder?.OrderNumber ?? "N/A",
                                        ProductionOrderItemId = itemId,
                                        ProductName = planningInfo.Product.Name,
                                        RunNumber = key.Item4 + 1,
                                        LineName = assignedLine?.Name ?? "N/A",
                                        WorkCenterName = wc?.Name ?? "N/A",
                                        OperationName = opType?.Name ?? "N/A",
                                        QuantityTons = qty,
                                        StartTime = startTime,
                                        EndTime = endTime
                                    });
                                }
                            }
                            if (planningInfo.GeneratedSurplus > 0 && planningInfo.FirstRelevantWorkCenter != null)
                            {
                                planResult.GeneratedSurplus.Add(new SurplusSummaryDto
                                {
                                    ProductName = planningInfo.Product.Name,
                                    WorkCenterName = planningInfo.FirstRelevantWorkCenter.Name,
                                    QuantityTons = planningInfo.GeneratedSurplus
                                });
                            }
                        }
                    } // Fim foreach solverVars

                    foreach (var kvp in itemPlanningInfos)
                    {
                        if (!scheduledItems.Contains(kvp.Key))
                        {
                            var planningInfo = kvp.Value;
                            AddUnscheduledItem(planResult, planningInfo, $"Solver status {status}, item não selecionado/priorizado.");
                            totalProfit -= (planningInfo.OrderItem.Quantity * planningInfo.Product.PenalityCost);
                        }
                    }

                    planResult.EstimatedTotalProfit = totalProfit;
                    planResult.PlanActualCompletionDate = (lastCompletionDate > horizonStartDt) ? lastCompletionDate : (DateTime?)null;
                }
                else
                {
                    _logger.LogWarning("Nenhuma solução factível encontrada. Status: {Status}", status);
                    foreach (var kvp in itemPlanningInfos) { AddUnscheduledItem(planResult, kvp.Value, $"Solver status was {status}."); }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro durante o processo de planejamento de produção.");
                planResult.SolverStatus = "ERROR";
                if (itemPlanningInfos != null && itemPlanningInfos.Any())
                {
                    foreach (var kvp in itemPlanningInfos) { AddUnscheduledItem(planResult, kvp.Value, $"Erro geral no planejamento: {ex.Message}"); }
                }
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Processo de planejamento de produção concluído com status final: {Status}. Tempo total: {ElapsedMilliseconds} ms.", planResult.SolverStatus, stopwatch.ElapsedMilliseconds);
            }

            return planResult;
        }

        // Método auxiliar para adicionar item não agendado
        private void AddUnscheduledItem(ProductionPlanResultDto planResult, OrderItemPlanningInfo planningInfo, string reason)
        {
            if (planningInfo?.OrderItem == null || planningInfo.Product == null) { _logger.LogWarning("Tentativa de adicionar item não agendado com planningInfo inválido."); return; }
            planResult.UnscheduledOrderItems.Add(new UnscheduledOrderItemDto
            {
                ProductionOrderNumber = planningInfo.OrderItem.ProductionOrder?.OrderNumber ?? "N/A",
                ProductionOrderItemId = planningInfo.OrderItem.ID,
                ProductName = planningInfo.Product.Name,
                RequiredQuantityTons = planningInfo.OrderItem.Quantity,
                OriginalDeadline = planningInfo.OrderItem.ProductionOrder?.Deadline ?? DateTime.MinValue,
                Reason = reason
            });
        }

    } // Fim da classe ProductionSolverService

    /// <summary>
    /// Interface para o ProductionSolverService (para injeção de dependência).
    /// </summary>
    public interface IProductionSolverService
    {
        Task<ProductionPlanResultDto> PlanProductionAsync(PlanningInputDto input);
    }
}
