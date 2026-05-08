using DevOpsMetricsApp.Data;
using DevOpsMetricsApp.Models;
using DevOpsMetricsApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;

namespace DevOpsMetricsApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly PredictionService _predictionService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, PredictionService predictionService)
        {
            _logger = logger;
            _context = context;
            _predictionService = predictionService;
        }

        public async Task<IActionResult> Index()
        {
            var records = await _context.DeploymentRecords
                .OrderByDescending(d => d.Timestamp)
                .ToListAsync();

            double failureRate = 0;
            if (records.Any())
            {
                double failedCount = records.Count(r => !r.IsSuccessful);
                failureRate = (failedCount / records.Count) * 100;
            }

            // Populate the ViewModel
            var viewModel = new DashboardViewModel
            {
                TotalDeployments = records.Count,
                FailureRate = failureRate,
                AverageBuildTime = records.Any() ? records.Average(r => r.BuildDurationSeconds) : 0,
                PredictedNextBuildTime = await _predictionService.PredictNextBuildDurationAsync(),
                RecentDeployments = records.Take(10).ToList() // Show the 10 most recent
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ExportTelemetryCsv()
        {
            var records = await _context.DeploymentRecords.OrderByDescending(d => d.Timestamp).ToListAsync();

            var builder = new StringBuilder();
            builder.AppendLine("Id,Repository,CommitHash,Author,DurationSeconds,IsSuccessful,Timestamp");

            foreach (var r in records)
            {
                builder.AppendLine($"{r.Id},{r.RepositoryName},{r.CommitHash},{r.AnonymizedAuthorId},{r.BuildDurationSeconds},{r.IsSuccessful},{r.Timestamp}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", $"TelemetryReport_{DateTime.Now:yyyyMMdd}.csv");
        }

        [HttpPost]
        public async Task<IActionResult> GenerateMockData()
        {
            var random = new Random();
            for (int i = 0; i < 15; i++)
            {
                _context.DeploymentRecords.Add(new DeploymentRecord
                {
                    RepositoryName = random.Next(0, 2) == 0 ? "Aquat-Billing-API" : "Albaik-Ordering-Web",
                    CommitHash = Guid.NewGuid().ToString().Substring(0, 7),
                    AnonymizedAuthorId = $"USR-{random.Next(1000, 9999)}",
                    BuildDurationSeconds = random.Next(45, 180) + random.NextDouble(),
                    IsSuccessful = random.Next(0, 10) > 1, // 80% chance of success
                    Timestamp = DateTime.UtcNow.AddHours(-random.Next(1, 72))
                });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "Administrator,TeamLeader")]
        public IActionResult WebhookSettings()
        {
            var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            ViewBag.WebhookUrl = $"{baseUrl}/api/webhook/receive";

            ViewBag.SecretKey = "DevOpsSecureToken2026";

            return View();
        }
    }
}