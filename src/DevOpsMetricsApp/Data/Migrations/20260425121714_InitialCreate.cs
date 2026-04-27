using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevOpsMetricsApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeploymentRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RepositoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommitHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnonymizedAuthorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BuildDurationSeconds = table.Column<double>(type: "float", nullable: false),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeploymentRecords", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeploymentRecords");
        }
    }
}
