using System.ComponentModel.DataAnnotations;

namespace DevOpsMetricsApp.Models
{
    public class DeploymentRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RepositoryName { get; set; }

        [Required]
        public string CommitHash { get; set; }

        public string AnonymizedAuthorId { get; set; }

        public double BuildDurationSeconds { get; set; }

        public bool IsSuccessful { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
