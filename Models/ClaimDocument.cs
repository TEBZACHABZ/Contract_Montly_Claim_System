using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contract_Monthly_Claim_System.Models
{
    public class ClaimDocument
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        public int ClaimId { get; set; }

        [ForeignKey("ClaimId")]
        public Claim? Claim { get; set; }

        [Required]
        public string FileName { get; set; } = "";

        [Required]
        public string FilePath { get; set; } = "";

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
