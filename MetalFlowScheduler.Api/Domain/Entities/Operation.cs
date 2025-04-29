using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MetalFlowScheduler.Api.Domain.Entities
{
    [Table("Operations")]
    public class Operation : BaseEntity
    {


        [Required]
        [StringLength(100)] // Example string length
        public string Name { get; set; }

        public int SetupTimeInMinutes { get; set; } // Setup time for this specific operation instance

        [Required]
        [Column(TypeName = "float")] // Capacity in Tons per Hour
        public double Capacity { get; set; }



        // Foreign key and navigation property to OperationType
        [ForeignKey("OperationType")]
        public int OperationTypeID { get; set; }
        public OperationType OperationType { get; set; }

        // Foreign key and navigation property to WorkCenter
        [ForeignKey("WorkCenter")]
        public int WorkCenterID { get; set; }
        public WorkCenter WorkCenter { get; set; }
    }
}
