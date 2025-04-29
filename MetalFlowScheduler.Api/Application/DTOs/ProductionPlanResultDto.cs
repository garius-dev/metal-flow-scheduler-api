using MetalFlowScheduler.Api.Application.Services;

namespace MetalFlowScheduler.Api.Application.DTOs
{
    /// <summary>
    /// Represents the detailed result of the production planning process.
    /// Matches the definition in ProjectScope.md.
    /// </summary>
    public class ProductionPlanResultDto
    {
        public DateTime PlanOverallDeadline { get; set; }
        public DateTime? PlanActualCompletionDate { get; set; } // Nullable if no tasks scheduled
        public List<ScheduledRunDetailDto> ScheduledRuns { get; set; } = new List<ScheduledRunDetailDto>();
        public List<SurplusSummaryDto> GeneratedSurplus { get; set; } = new List<SurplusSummaryDto>();
        public decimal EstimatedTotalProfit { get; set; }
        public List<UnscheduledOrderItemDto> UnscheduledOrderItems { get; set; } = new List<UnscheduledOrderItemDto>();
        public string SolverStatus { get; set; } = "Not Solved";
        public TimeSpan SolverSolveTime { get; set; } = TimeSpan.Zero;
    }
}
