using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetalFlowScheduler.Api.Domain.Entities
{
    [Table("OperationTypes")]
    public class OperationType : BaseEntity
    {


        [Required]
        [StringLength(100)] // Example string length, adjust as needed
        public string Name { get; set; }



        // Inverse relationships for EF Core navigation
        // An OperationType can be associated with many concrete Operations
        public List<Operation> Operations { get; set; } = new List<Operation>();

        // An OperationType appears in many Work Center route steps
        public List<WorkCenterOperationRoute> WorkCenterRoutes { get; set; } = new List<WorkCenterOperationRoute>();

        // An OperationType appears in many Product operation route steps
        public List<ProductOperationRoute> ProductRoutes { get; set; } = new List<ProductOperationRoute>();
    }
}
