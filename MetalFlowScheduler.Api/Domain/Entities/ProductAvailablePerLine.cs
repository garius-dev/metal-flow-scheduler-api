using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MetalFlowScheduler.Api.Domain.Entities
{
    [Table("ProductsAvailablesPerLines")]
    public class ProductAvailablePerLine : BaseEntity
    {




        // Foreign key and navigation property to Line
        [ForeignKey("Line")]
        public int LineID { get; set; }
        public Line Line { get; set; }

        // Foreign key and navigation property to Product
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }
    }
}
