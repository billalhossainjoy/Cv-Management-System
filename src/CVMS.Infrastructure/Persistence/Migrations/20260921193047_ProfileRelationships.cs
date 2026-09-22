using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVMS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProfileRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileValues_Attributes_AttributeId",
                table: "ProfileValues");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileValues_Attributes_AttributeId",
                table: "ProfileValues",
                column: "AttributeId",
                principalTable: "Attributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileValues_Attributes_AttributeId",
                table: "ProfileValues");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileValues_Attributes_AttributeId",
                table: "ProfileValues",
                column: "AttributeId",
                principalTable: "Attributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
