using System.Collections.Generic;

namespace DevOpsMetricsApp.Models
{
    public class DashboardViewModel
    {
        public int TotalDeployments { get; set; }
        public double FailureRate { get; set; }
        public double AverageBuildTime { get; set; }
        public float PredictedNextBuildTime { get; set; }
        public List<DeploymentRecord> RecentDeployments { get; set; } = new List<DeploymentRecord>();
    }
}