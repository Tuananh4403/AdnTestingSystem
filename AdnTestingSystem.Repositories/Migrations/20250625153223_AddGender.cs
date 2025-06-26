﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdnTestingSystem.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddGender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "UserProfiles");
        }
    }
}
