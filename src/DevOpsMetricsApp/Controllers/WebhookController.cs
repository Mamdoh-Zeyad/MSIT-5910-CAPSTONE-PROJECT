using Microsoft.AspNetCore.Mvc;
using DevOpsMetricsApp.Data;
using DevOpsMetricsApp.Models;

namespace DevOpsMetricsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WebhookController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("receive")]
        public async Task<IActionResult> ReceiveDeploymentData([FromBody] DeploymentRecord payload)
        {
            var providedSecret = Request.Headers["X-API-Key"].FirstOrDefault();
            var expectedSecret = "DevOpsSecureToken2026";

            if (providedSecret != expectedSecret)
            {
                return Unauthorized("Invalid webhook signature. Access denied.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid telemetry payload.");
            }

            try
            {
                if (payload.Timestamp == default)
                {
                    payload.Timestamp = DateTime.UtcNow;
                }

                _context.DeploymentRecords.Add(payload);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Telemetry data successfully ingested.", RecordId = payload.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error during data ingestion.");
            }
        }
    }
}