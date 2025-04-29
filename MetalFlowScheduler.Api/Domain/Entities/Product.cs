using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MetalFlowScheduler.Api.Domain.Entities
{
    [Table("Products")]
    public class Product : BaseEntity
    {


        [Required]
        [StringLength(100)] // Example string length
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")] // Example decimal mapping
        public decimal UnitPricePerTon { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")] // Example for percentage margin
        public decimal ProfitMargin { get; set; }

        [Required]
        public int Priority { get; set; } // Product priority

        [Required]
        [Column(TypeName = "decimal(18, 2)")] // Example for penalty cost per delay
        public decimal PenalityCost { get; set; }



        // Relationships
        // This Product has a sequence of operation types required for production (with versioning)
        public List<ProductOperationRoute> OperationRoutes { get; set; } = new List<ProductOperationRoute>();

        // Where this Product is available on Lines (linking table)
        public List<ProductAvailablePerLine> AvailableOnLines { get; set; } = new List<ProductAvailablePerLine>();

        // Inverse relationship: Which production order items refer to this product
        public List<ProductionOrderItem> ProductionOrderItems { get; set; } = new List<ProductionOrderItem>();

        // Inverse relationship: Surplus stock of this Product
        public List<SurplusPerProductAndWorkCenter> SurplusStocks { get; set; } = new List<SurplusPerProductAndWorkCenter>();
    }
}
