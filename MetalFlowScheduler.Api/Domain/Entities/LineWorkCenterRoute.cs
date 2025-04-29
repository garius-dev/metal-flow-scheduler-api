using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MetalFlowScheduler.Api.Domain.Entities
{
    [Table("LinesWorkCentersRoutes")]
    public class LineWorkCenterRoute : BaseEntity
    {
       
        public int Order { get; set; } // The order of this work center in the line route


        // --- Versioning Fields ---
        public int Version { get; set; }
        public DateTime EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        // ----------------------------

        // --- Transport Time Field ---
        [Required]
        public int TransportTimeInMinutes { get; set; } // Time to transport AFTER completing this WorkCenter to the next in the Line
        // ---------------------------------

        // Foreign key and navigation property to Line
        [ForeignKey("Line")]
        public int LineID { get; set; }
        public Line Line { get; set; }

        // Foreign key and navigation property to WorkCenter
        [ForeignKey("WorkCenter")]
        public int WorkCenterID { get; set; }
        public WorkCenter WorkCenter { get; set; }
    }
}
