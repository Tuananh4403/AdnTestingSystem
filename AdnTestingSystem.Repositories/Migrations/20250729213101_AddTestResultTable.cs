using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdnTestingSystem.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddTestResultTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReleasedAt",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "ResultFileUrl",
                table: "TestResults");

            migrationBuilder.RenameColumn(
                name: "Summary",
                table: "TestResults",
                newName: "Conclusion");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "TestResults",
                newName: "DeletedBy");

            migrationBuilder.AddColumn<decimal>(
                name: "CPI",
                table: "TestResults",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "TestResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "TestResults",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Probability",
                table: "TestResults",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "TestResultDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestResultId = table.Column<int>(type: "int", nullable: false),
                    Locus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Allele1_Person1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Allele2_Person1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Allele1_Person2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Allele2_Person2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PI = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestResultDetail_TestResults_TestResultId",
                        column: x => x.TestResultId,
                        principalTable: "TestResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_CreatedById",
                table: "TestResults",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultDetail_TestResultId",
                table: "TestResultDetail",
                column: "TestResultId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Users_CreatedById",
                table: "TestResults",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Users_CreatedById",
                table: "TestResults");

            migrationBuilder.DropTable(
                name: "TestResultDetail");

            migrationBuilder.DropIndex(
                name: "IX_TestResults_CreatedById",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "CPI",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "Probability",
                table: "TestResults");

            migrationBuilder.RenameColumn(
                name: "DeletedBy",
                table: "TestResults",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "Conclusion",
                table: "TestResults",
                newName: "Summary");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleasedAt",
                table: "TestResults",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ResultFileUrl",
                table: "TestResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
