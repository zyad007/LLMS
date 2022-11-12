using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LLS.DAL.Migrations
{
    public partial class third : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expirments_LLO_LLOGuid",
                table: "Expirments");

            migrationBuilder.DropTable(
                name: "Block");

            migrationBuilder.DropTable(
                name: "ChoiceScore");

            migrationBuilder.DropTable(
                name: "Configs");

            migrationBuilder.DropTable(
                name: "Page");

            migrationBuilder.DropTable(
                name: "Score");

            migrationBuilder.DropTable(
                name: "LLO");

            migrationBuilder.DropIndex(
                name: "IX_Expirments_LLOGuid",
                table: "Expirments");

            migrationBuilder.DropColumn(
                name: "LLOGuid",
                table: "Expirments");

            migrationBuilder.RenameColumn(
                name: "LLOPath",
                table: "Expirments",
                newName: "LLO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LLO",
                table: "Expirments",
                newName: "LLOPath");

            migrationBuilder.AddColumn<Guid>(
                name: "LLOGuid",
                table: "Expirments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    Grading = table.Column<string>(type: "text", nullable: true),
                    Help = table.Column<string>(type: "text", nullable: true),
                    NumberOfTrials = table.Column<int>(type: "integer", nullable: false),
                    ShowTime = table.Column<bool>(type: "boolean", nullable: false),
                    Time = table.Column<string>(type: "text", nullable: true),
                    TimeState = table.Column<bool>(type: "boolean", nullable: false),
                    ViewCorrectAnswer = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "LLO",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LLO", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Score",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    Answer = table.Column<string>(type: "text", nullable: true),
                    score = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Score", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Page",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    LLOGuid = table.Column<Guid>(type: "uuid", nullable: true),
                    id = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Page", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Page_LLO_LLOGuid",
                        column: x => x.LLOGuid,
                        principalTable: "LLO",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChoiceScore",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    ChoiceId = table.Column<int>(type: "integer", nullable: false),
                    ScoreGuid = table.Column<Guid>(type: "uuid", nullable: true),
                    score = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChoiceScore", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_ChoiceScore_Score_ScoreGuid",
                        column: x => x.ScoreGuid,
                        principalTable: "Score",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Block",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigGuid = table.Column<Guid>(type: "uuid", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    MainType = table.Column<int>(type: "integer", nullable: false),
                    PageGuid = table.Column<Guid>(type: "uuid", nullable: true),
                    ScoreGuid = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Block", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Block_Configs_ConfigGuid",
                        column: x => x.ConfigGuid,
                        principalTable: "Configs",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Block_Page_PageGuid",
                        column: x => x.PageGuid,
                        principalTable: "Page",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Block_Score_ScoreGuid",
                        column: x => x.ScoreGuid,
                        principalTable: "Score",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expirments_LLOGuid",
                table: "Expirments",
                column: "LLOGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Block_ConfigGuid",
                table: "Block",
                column: "ConfigGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Block_PageGuid",
                table: "Block",
                column: "PageGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Block_ScoreGuid",
                table: "Block",
                column: "ScoreGuid");

            migrationBuilder.CreateIndex(
                name: "IX_ChoiceScore_ScoreGuid",
                table: "ChoiceScore",
                column: "ScoreGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Page_LLOGuid",
                table: "Page",
                column: "LLOGuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Expirments_LLO_LLOGuid",
                table: "Expirments",
                column: "LLOGuid",
                principalTable: "LLO",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
