using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MetalFlowScheduler.Api.Domain.Entities
{
    [Table("ProductionOrders")]
    public class ProductionOrder : BaseEntity
    {


        [Required]
        [StringLength(100)] // Max length for the order number
        public string OrderNumber { get; set; }

        [Required]
        public DateTime EarliestStartDate { get; set; } // Earliest possible start date/time for this order

        [Required]
        public DateTime Deadline { get; set; } // Required completion date/time for ALL items in this order

        // Relationship: This Production Order has multiple items
        public List<ProductionOrderItem> Items { get; set; } = new List<ProductionOrderItem>();



        // Optional constructor
        public ProductionOrder(int id, string orderNumber, DateTime earliestStartDate, DateTime deadline)
        {
            ID = id;
            OrderNumber = orderNumber;
            EarliestStartDate = earliestStartDate;
            Deadline = deadline;
            Enabled = true;
        }

        public ProductionOrder()
        {
            // Empty constructor for deserialization or default initialization
            Enabled = true;
        }
    }
}
