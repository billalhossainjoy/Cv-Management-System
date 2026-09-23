using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVMS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnologyTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TechnologyTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnologyTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PositionTechnologyTags",
                columns: table => new
                {
                    PositionsId = table.Column<Guid>(type: "uuid", nullable: false),
                    TechnologyTagsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionTechnologyTags", x => new { x.PositionsId, x.TechnologyTagsId });
                    table.ForeignKey(
                        name: "FK_PositionTechnologyTags_Positions_PositionsId",
                        column: x => x.PositionsId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PositionTechnologyTags_TechnologyTags_TechnologyTagsId",
                        column: x => x.TechnologyTagsId,
                        principalTable: "TechnologyTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTechnologyTags",
                columns: table => new
                {
                    ProjectsId = table.Column<Guid>(type: "uuid", nullable: false),
                    TechnologyTagsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTechnologyTags", x => new { x.ProjectsId, x.TechnologyTagsId });
                    table.ForeignKey(
                        name: "FK_ProjectTechnologyTags_Projects_ProjectsId",
                        column: x => x.ProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectTechnologyTags_TechnologyTags_TechnologyTagsId",
                        column: x => x.TechnologyTagsId,
                        principalTable: "TechnologyTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PositionTechnologyTags_TechnologyTagsId",
                table: "PositionTechnologyTags",
                column: "TechnologyTagsId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTechnologyTags_TechnologyTagsId",
                table: "ProjectTechnologyTags",
                column: "TechnologyTagsId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnologyTags_Name",
                table: "TechnologyTags",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PositionTechnologyTags");

            migrationBuilder.DropTable(
                name: "ProjectTechnologyTags");

            migrationBuilder.DropTable(
                name: "TechnologyTags");
        }
    }
}
