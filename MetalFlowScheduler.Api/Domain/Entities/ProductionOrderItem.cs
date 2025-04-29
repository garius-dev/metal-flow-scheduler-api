using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MetalFlowScheduler.Api.Domain.Entities
{
    [Table("ProductionOrderItems")]
    public class ProductionOrderItem : BaseEntity
    {


        // Foreign key and navigation property to the parent ProductionOrder
        [ForeignKey("ProductionOrder")]
        public int ProductionOrderID { get; set; }
        public ProductionOrder ProductionOrder { get; set; }

        // Reference to the Product
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        // Navigation property
        public Product Product { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")] // Quantity of this product to be produced in Tons
        public decimal Quantity { get; set; }


    }
}
