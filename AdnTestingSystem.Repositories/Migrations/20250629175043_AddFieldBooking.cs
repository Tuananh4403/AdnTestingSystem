using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdnTestingSystem.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCivil",
                table: "Bookings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCivil",
                table: "Bookings");
        }
    }
}
