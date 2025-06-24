using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdnTestingSystem.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeStamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserProfiles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "UserProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "UserProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TestResults",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "TestResults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TestResults",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "TestResults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ServicePrices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "ServicePrices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResultTimeType",
                table: "ServicePrices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ServicePrices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "ServicePrices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Samples",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Samples",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Samples",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Samples",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Samples",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Samples",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Ratings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Ratings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Ratings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DnaTestServices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "DnaTestServices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "DnaTestServices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "DnaTestServices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedBy",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Blogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Blogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Blogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Blogs",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ServicePrices");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ServicePrices");

            migrationBuilder.DropColumn(
                name: "ResultTimeType",
                table: "ServicePrices");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ServicePrices");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ServicePrices");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DnaTestServices");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "DnaTestServices");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DnaTestServices");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "DnaTestServices");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Blogs");
        }
    }
}
