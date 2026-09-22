using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVMS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UniqueProfileAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProfileValues_ProfileId",
                table: "ProfileValues");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileValues_ProfileId_AttributeId",
                table: "ProfileValues",
                columns: new[] { "ProfileId", "AttributeId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProfileValues_ProfileId_AttributeId",
                table: "ProfileValues");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileValues_ProfileId",
                table: "ProfileValues",
                column: "ProfileId");
        }
    }
}
