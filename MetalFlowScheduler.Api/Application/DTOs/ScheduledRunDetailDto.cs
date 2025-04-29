namespace MetalFlowScheduler.Api.Application.DTOs
{
    /// <summary>
    /// Details of a single scheduled production run (part of ProductionPlanResultDto).
    /// Matches the definition in ProjectScope.md.
    /// </summary>
    public class ScheduledRunDetailDto
    {
        public int RunId { get; set; } // Unique identifier for this run entry in the plan
        public string ProductionOrderNumber { get; set; } = string.Empty;
        public int ProductionOrderItemId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int RunNumber { get; set; } // e.g., 1st run, 2nd run for a specific item/WC
        public string LineName { get; set; } = string.Empty;
        public string WorkCenterName { get; set; } = string.Empty;
        public string OperationName { get; set; } = string.Empty; // Or OperationType Name
        public decimal QuantityTons { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
