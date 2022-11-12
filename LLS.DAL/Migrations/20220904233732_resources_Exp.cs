using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LLS.DAL.Migrations
{
    public partial class resources_Exp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Resource_Exps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Exp_CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resource_Exps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resource_Exps_Exp_Courses_Exp_CourseId",
                        column: x => x.Exp_CourseId,
                        principalTable: "Exp_Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resource_Exps_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Resource_Exps_Exp_CourseId",
                table: "Resource_Exps",
                column: "Exp_CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_Exps_ResourceId",
                table: "Resource_Exps",
                column: "ResourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Resource_Exps");
        }
    }
}
