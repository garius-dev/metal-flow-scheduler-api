using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MetalFlowScheduler.Api.Domain.Entities
{
    [Table("ProductsOperationRoutes")]
    public class ProductOperationRoute : BaseEntity
    {


        public int Order { get; set; } // The order of this operation type in the product route



        // --- Versioning Fields ---
        public int Version { get; set; }
        public DateTime EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        // ----------------------------

        // Foreign key and navigation property to Product
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        // Foreign key and navigation property to OperationType
        [ForeignKey("OperationType")]
        public int OperationTypeID { get; set; }
        public OperationType OperationType { get; set; }
    }
}
