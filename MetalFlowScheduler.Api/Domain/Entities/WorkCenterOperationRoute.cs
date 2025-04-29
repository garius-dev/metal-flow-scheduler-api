using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MetalFlowScheduler.Api.Domain.Entities
{
    [Table("WorkCentersOperationRoutes")]
    public class WorkCenterOperationRoute : BaseEntity
    {


        [StringLength(50)] // Name or identifier for the route step
        public string Name { get; set; }

        public int Order { get; set; } // The order of this step in the route



        // --- Versioning Fields ---
        public int Version { get; set; } // Route version number
        public DateTime EffectiveStartDate { get; set; } // Date/time when this version of this step becomes active
        public DateTime? EffectiveEndDate { get; set; } // Date/time when this version of this step becomes inactive (null if current version)
        // ----------------------------

        // --- Transport Time Field ---
        [Required]
        public int TransportTimeInMinutes { get; set; } // Time to transport AFTER completing this operation type step to the next within the WorkCenter
        // ---------------------------------

        // Foreign key and navigation property to WorkCenter
        [ForeignKey("WorkCenter")]
        public int WorkCenterID { get; set; }
        public WorkCenter WorkCenter { get; set; }

        // Foreign key and navigation property to OperationType
        [ForeignKey("OperationType")]
        public int OperationTypeID { get; set; }
        public OperationType OperationType { get; set; }
    }
}
