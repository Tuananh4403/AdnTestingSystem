using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdnTestingSystem.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldSampleCollector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SampleCollectorId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SampleCollectorId",
                table: "Bookings",
                column: "SampleCollectorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_SampleCollectorId",
                table: "Bookings",
                column: "SampleCollectorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_SampleCollectorId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_SampleCollectorId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "SampleCollectorId",
                table: "Bookings");
        }
    }
}
