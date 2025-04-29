using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MetalFlowScheduler.Api.Domain.Entities
{
    [Table("WorkCenters")]
    public class WorkCenter : BaseEntity
    {


        [Required]
        [StringLength(100)] // Example string length
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")] // Optimal batch size in Tons
        public decimal OptimalBatch { get; set; }



        // Foreign key and navigation property to Line
        [ForeignKey("Line")]
        public int LineID { get; set; }
        public Line Line { get; set; }

        // Relationships
        // This WorkCenter contains multiple concrete Operations
        public List<Operation> Operations { get; set; } = new List<Operation>();

        // This WorkCenter appears in many Line Work Center route steps
        public List<LineWorkCenterRoute> LineRoutes { get; set; } = new List<LineWorkCenterRoute>();

        // This WorkCenter has multiple Operation Type route step definitions (with versioning)
        public List<WorkCenterOperationRoute> OperationRoutes { get; set; } = new List<WorkCenterOperationRoute>();

        // Inverse relationship: Surplus stock at this WorkCenter
        public List<SurplusPerProductAndWorkCenter> SurplusStocks { get; set; } = new List<SurplusPerProductAndWorkCenter>();
    }
}
