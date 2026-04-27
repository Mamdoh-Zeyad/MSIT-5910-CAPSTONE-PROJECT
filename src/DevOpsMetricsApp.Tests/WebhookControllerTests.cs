using DevOpsMetricsApp.Controllers;
using DevOpsMetricsApp.Data;
using DevOpsMetricsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace DevOpsMetricsApp.Tests
{
    public class WebhookControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task ReceiveDeploymentData_ValidPayload_ReturnsOkResult()
        {
            var context = GetInMemoryDbContext();
            var controller = new WebhookController(context);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-API-Key"] = "DevOpsSecureToken2026";
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            var testPayload = new DeploymentRecord
            {
                RepositoryName = "DevOps-Billing-API",
                CommitHash = "abc1234",
                AnonymizedAuthorId = "USR-999",
                BuildDurationSeconds = 120.5,
                IsSuccessful = true
            };

            var result = await controller.ReceiveDeploymentData(testPayload);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            Assert.Equal(1, await context.DeploymentRecords.CountAsync());
        }
    }
}