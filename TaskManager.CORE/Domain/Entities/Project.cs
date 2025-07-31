
using System.ComponentModel.DataAnnotations;


namespace WebApiTaskManager.Core.Domain.Entities
{
    public class Project
    {
        [Key]
        public Guid ProjectId { get; set; }
        [Required]
        [MaxLength(100)]

        public string ProjectName { get; set; } = string.Empty;
        [Required]
        [MaxLength(500)]
        public string ProjectDescription { get; set; } = string.Empty;

        [Required]
        public DateTime? DateOfStart { get; set; }

        [Required]

        public int TeamSize { get; set; }

    }
}
