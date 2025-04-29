namespace MetalFlowScheduler.Api.Application.DTOs
{
    /// <summary>
    /// Summary of surplus generated for a specific product at a specific work center (part of ProductionPlanResultDto).
    /// Matches the definition in ProjectScope.md.
    /// </summary>
    public class SurplusSummaryDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string WorkCenterName { get; set; } = string.Empty;
        public decimal QuantityTons { get; set; }
    }
}
