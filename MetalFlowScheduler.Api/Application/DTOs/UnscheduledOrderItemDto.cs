namespace MetalFlowScheduler.Api.Application.DTOs
{
    /// <summary>
    /// Details of a production order item that could not be scheduled (part of ProductionPlanResultDto).
    /// Matches the definition in ProjectScope.md.
    /// </summary>
    public class UnscheduledOrderItemDto
    {
        public string ProductionOrderNumber { get; set; } = string.Empty;
        public int ProductionOrderItemId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal RequiredQuantityTons { get; set; }
        public DateTime OriginalDeadline { get; set; }
        public string Reason { get; set; } = "Could not be scheduled within constraints/horizon."; // Optional: More specific reason
    }
}
