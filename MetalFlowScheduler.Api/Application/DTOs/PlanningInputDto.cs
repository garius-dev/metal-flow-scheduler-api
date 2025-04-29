namespace MetalFlowScheduler.Api.Application.DTOs
{
    /// <summary>
    /// Input for the production planning process.
    /// Defines the scope and timeframe for the plan.
    /// </summary>
    public class PlanningInputDto
    {
        /// <summary>
        /// Optional: List of specific ProductionOrder IDs to include in the plan.
        /// If null or empty, might consider all pending orders within the horizon.
        /// </summary>
        public List<int>? ProductionOrderIds { get; set; }

        /// <summary>
        /// The starting date and time for the planning horizon.
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        public DateTime HorizonStartDate { get; set; }

        /// <summary>
        /// The ending date and time (overall deadline) for the planning horizon.
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        public DateTime HorizonEndDate { get; set; }

        // TODO: Add other potential inputs, e.g., Planning Strategy (Maximize Profit, OnTimeDelivery, etc.)
    }
}
