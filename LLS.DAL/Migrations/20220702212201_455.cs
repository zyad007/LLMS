using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LLS.DAL.Migrations
{
    public partial class _455 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LRO",
                table: "Trials");

            migrationBuilder.AddColumn<Guid>(
                name: "LROId",
                table: "Trials",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LRO",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Result = table.Column<string>(type: "text", nullable: true),
                    TotalGrade = table.Column<float>(type: "real", nullable: false),
                    TrialNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LRO", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trials_LROId",
                table: "Trials",
                column: "LROId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trials_LRO_LROId",
                table: "Trials",
                column: "LROId",
                principalTable: "LRO",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trials_LRO_LROId",
                table: "Trials");

            migrationBuilder.DropTable(
                name: "LRO");

            migrationBuilder.DropIndex(
                name: "IX_Trials_LROId",
                table: "Trials");

            migrationBuilder.DropColumn(
                name: "LROId",
                table: "Trials");

            migrationBuilder.AddColumn<string>(
                name: "LRO",
                table: "Trials",
                type: "text",
                nullable: true);
        }
    }
}
