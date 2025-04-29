using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MetalFlowScheduler.Api.Domain.Entities
{
    [Table("SurplusPerProductAndWorkCenter")]
    public class SurplusPerProductAndWorkCenter : BaseEntity
    {


        // Foreign key and navigation property to Product
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        // Foreign key and navigation property to WorkCenter
        [ForeignKey("WorkCenter")]
        public int WorkCenterID { get; set; }
        public WorkCenter WorkCenter { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")] // Surplus quantity in Tons
        public decimal Surplus { get; set; }


    }
}
