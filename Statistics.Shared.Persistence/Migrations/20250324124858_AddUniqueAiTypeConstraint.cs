using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Statistics.Shared.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueAiTypeConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ArtificialIntelligences_AiType",
                table: "ArtificialIntelligences",
                column: "AiType",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ArtificialIntelligences_AiType",
                table: "ArtificialIntelligences");
        }
    }
}
