using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdnTestingSystem.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddTestResultField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResultDetail_TestResults_TestResultId",
                table: "TestResultDetail");

            migrationBuilder.AddColumn<bool>(
                name: "IsTestResultCreated",
                table: "Bookings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResultDetail_TestResults_TestResultId",
                table: "TestResultDetail",
                column: "TestResultId",
                principalTable: "TestResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResultDetail_TestResults_TestResultId",
                table: "TestResultDetail");

            migrationBuilder.DropColumn(
                name: "IsTestResultCreated",
                table: "Bookings");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResultDetail_TestResults_TestResultId",
                table: "TestResultDetail",
                column: "TestResultId",
                principalTable: "TestResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
