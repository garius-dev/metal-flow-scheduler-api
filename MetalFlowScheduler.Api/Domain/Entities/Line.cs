using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MetalFlowScheduler.Api.Domain.Entities
{
    [Table("Lines")]
    public class Line : BaseEntity
    {
        

        [Required]
        [StringLength(100)] // Example string length
        public string Name { get; set; }


        // Relationships
        // This Line contains WorkCenters
        public List<WorkCenter> WorkCenters { get; set; } = new List<WorkCenter>();

        // This Line has a sequence of WorkCenters (with versioning)
        public List<LineWorkCenterRoute> WorkCenterRoutes { get; set; } = new List<LineWorkCenterRoute>();

        // Which products can be produced on this line (linking table)
        public List<ProductAvailablePerLine> AvailableProducts { get; set; } = new List<ProductAvailablePerLine>();
    }
}
