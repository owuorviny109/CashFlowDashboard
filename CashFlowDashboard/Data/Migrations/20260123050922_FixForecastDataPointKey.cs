using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CashFlowDashboard.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixForecastDataPointKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ForecastDataPoint",
                table: "ForecastDataPoint");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ForecastDataPoint",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ForecastDataPoint",
                table: "ForecastDataPoint",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastDataPoint_ForecastScenarioId",
                table: "ForecastDataPoint",
                column: "ForecastScenarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ForecastDataPoint",
                table: "ForecastDataPoint");

            migrationBuilder.DropIndex(
                name: "IX_ForecastDataPoint_ForecastScenarioId",
                table: "ForecastDataPoint");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ForecastDataPoint",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ForecastDataPoint",
                table: "ForecastDataPoint",
                columns: new[] { "ForecastScenarioId", "Id" });
        }
    }
}
