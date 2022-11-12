using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LLS.DAL.Migrations
{
    public partial class sec : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blocks_Configs_ConfigGuid",
                table: "Blocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Blocks_Pages_PageGuid",
                table: "Blocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Blocks_Scores_ScoreGuid",
                table: "Blocks");

            migrationBuilder.DropForeignKey(
                name: "FK_ChoiceScore_Scores_ScoreGuid",
                table: "ChoiceScore");

            migrationBuilder.DropForeignKey(
                name: "FK_Expirments_LLOs_LLOGuid",
                table: "Expirments");

            migrationBuilder.DropForeignKey(
                name: "FK_Pages_LLOs_LLOGuid",
                table: "Pages");

            migrationBuilder.DropTable(
                name: "Choices");

            migrationBuilder.DropTable(
                name: "OnSpots");

            migrationBuilder.DropTable(
                name: "Contents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Scores",
                table: "Scores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pages",
                table: "Pages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LLOs",
                table: "LLOs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Blocks",
                table: "Blocks");

            migrationBuilder.RenameTable(
                name: "Scores",
                newName: "Score");

            migrationBuilder.RenameTable(
                name: "Pages",
                newName: "Page");

            migrationBuilder.RenameTable(
                name: "LLOs",
                newName: "LLO");

            migrationBuilder.RenameTable(
                name: "Blocks",
                newName: "Block");

            migrationBuilder.RenameIndex(
                name: "IX_Pages_LLOGuid",
                table: "Page",
                newName: "IX_Page_LLOGuid");

            migrationBuilder.RenameIndex(
                name: "IX_Blocks_ScoreGuid",
                table: "Block",
                newName: "IX_Block_ScoreGuid");

            migrationBuilder.RenameIndex(
                name: "IX_Blocks_PageGuid",
                table: "Block",
                newName: "IX_Block_PageGuid");

            migrationBuilder.RenameIndex(
                name: "IX_Blocks_ConfigGuid",
                table: "Block",
                newName: "IX_Block_ConfigGuid");

            migrationBuilder.AddColumn<Guid>(
                name: "Idd",
                table: "Expirments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Score",
                table: "Score",
                column: "Guid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Page",
                table: "Page",
                column: "Guid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LLO",
                table: "LLO",
                column: "Guid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Block",
                table: "Block",
                column: "Guid");

            migrationBuilder.AddForeignKey(
                name: "FK_Block_Configs_ConfigGuid",
                table: "Block",
                column: "ConfigGuid",
                principalTable: "Configs",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Block_Page_PageGuid",
                table: "Block",
                column: "PageGuid",
                principalTable: "Page",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Block_Score_ScoreGuid",
                table: "Block",
                column: "ScoreGuid",
                principalTable: "Score",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChoiceScore_Score_ScoreGuid",
                table: "ChoiceScore",
                column: "ScoreGuid",
                principalTable: "Score",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Expirments_LLO_LLOGuid",
                table: "Expirments",
                column: "LLOGuid",
                principalTable: "LLO",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Page_LLO_LLOGuid",
                table: "Page",
                column: "LLOGuid",
                principalTable: "LLO",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Block_Configs_ConfigGuid",
                table: "Block");

            migrationBuilder.DropForeignKey(
                name: "FK_Block_Page_PageGuid",
                table: "Block");

            migrationBuilder.DropForeignKey(
                name: "FK_Block_Score_ScoreGuid",
                table: "Block");

            migrationBuilder.DropForeignKey(
                name: "FK_ChoiceScore_Score_ScoreGuid",
                table: "ChoiceScore");

            migrationBuilder.DropForeignKey(
                name: "FK_Expirments_LLO_LLOGuid",
                table: "Expirments");

            migrationBuilder.DropForeignKey(
                name: "FK_Page_LLO_LLOGuid",
                table: "Page");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Score",
                table: "Score");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Page",
                table: "Page");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LLO",
                table: "LLO");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Block",
                table: "Block");

            migrationBuilder.DropColumn(
                name: "Idd",
                table: "Expirments");

            migrationBuilder.RenameTable(
                name: "Score",
                newName: "Scores");

            migrationBuilder.RenameTable(
                name: "Page",
                newName: "Pages");

            migrationBuilder.RenameTable(
                name: "LLO",
                newName: "LLOs");

            migrationBuilder.RenameTable(
                name: "Block",
                newName: "Blocks");

            migrationBuilder.RenameIndex(
                name: "IX_Page_LLOGuid",
                table: "Pages",
                newName: "IX_Pages_LLOGuid");

            migrationBuilder.RenameIndex(
                name: "IX_Block_ScoreGuid",
                table: "Blocks",
                newName: "IX_Blocks_ScoreGuid");

            migrationBuilder.RenameIndex(
                name: "IX_Block_PageGuid",
                table: "Blocks",
                newName: "IX_Blocks_PageGuid");

            migrationBuilder.RenameIndex(
                name: "IX_Block_ConfigGuid",
                table: "Blocks",
                newName: "IX_Blocks_ConfigGuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scores",
                table: "Scores",
                column: "Guid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pages",
                table: "Pages",
                column: "Guid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LLOs",
                table: "LLOs",
                column: "Guid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blocks",
                table: "Blocks",
                column: "Guid");

            migrationBuilder.CreateTable(
                name: "Contents",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    HtmlContent = table.Column<string>(type: "text", nullable: true),
                    Instructions = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contents", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Choices",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    ChoiceId = table.Column<int>(type: "integer", nullable: false),
                    ContentGuid = table.Column<Guid>(type: "uuid", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Choices", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Choices_Contents_ContentGuid",
                        column: x => x.ContentGuid,
                        principalTable: "Contents",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OnSpots",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    ContentGuid = table.Column<Guid>(type: "uuid", nullable: true),
                    Left = table.Column<int>(type: "integer", nullable: false),
                    Top = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnSpots", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_OnSpots_Contents_ContentGuid",
                        column: x => x.ContentGuid,
                        principalTable: "Contents",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Choices_ContentGuid",
                table: "Choices",
                column: "ContentGuid");

            migrationBuilder.CreateIndex(
                name: "IX_OnSpots_ContentGuid",
                table: "OnSpots",
                column: "ContentGuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Blocks_Configs_ConfigGuid",
                table: "Blocks",
                column: "ConfigGuid",
                principalTable: "Configs",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Blocks_Pages_PageGuid",
                table: "Blocks",
                column: "PageGuid",
                principalTable: "Pages",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Blocks_Scores_ScoreGuid",
                table: "Blocks",
                column: "ScoreGuid",
                principalTable: "Scores",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChoiceScore_Scores_ScoreGuid",
                table: "ChoiceScore",
                column: "ScoreGuid",
                principalTable: "Scores",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Expirments_LLOs_LLOGuid",
                table: "Expirments",
                column: "LLOGuid",
                principalTable: "LLOs",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_LLOs_LLOGuid",
                table: "Pages",
                column: "LLOGuid",
                principalTable: "LLOs",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
