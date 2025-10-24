using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contract_Monthly_Claim_System.Models
{
    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }

        // lecturer who submitted
        public int LecturerId { get; set; }
        [ForeignKey("LecturerId")]
        public Lecturer? Lecturer { get; set; }

        [Required]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(0, 1000)]
        public decimal HoursWorked { get; set; }

        [Required]
        [Range(0, 100000)]
        public decimal HourlyRate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;

        // Navigation
        public List<SupportingDocument>? Documents { get; set; }
    }
}
