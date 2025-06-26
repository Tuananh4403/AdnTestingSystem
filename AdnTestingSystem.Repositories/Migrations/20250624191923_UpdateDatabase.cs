using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdnTestingSystem.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCivil",
                table: "ServicePrices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SampleMethod",
                table: "ServicePrices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "AppointmentTime",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResultTimeType",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCivil",
                table: "ServicePrices");

            migrationBuilder.DropColumn(
                name: "SampleMethod",
                table: "ServicePrices");

            migrationBuilder.DropColumn(
                name: "AppointmentTime",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ResultTimeType",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Bookings");
        }
    }
}
