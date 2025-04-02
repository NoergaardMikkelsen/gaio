using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Statistics.Shared.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStartAndEndSearchPropertiesToKeyword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndSearch",
                table: "Keywords",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartSearch",
                table: "Keywords",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndSearch",
                table: "Keywords");

            migrationBuilder.DropColumn(
                name: "StartSearch",
                table: "Keywords");
        }
    }
}
