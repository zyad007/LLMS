using Microsoft.EntityFrameworkCore.Migrations;

namespace LLS.DAL.Migrations
{
    public partial class _33 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LLO_MA",
                table: "Expirments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LLO_SA",
                table: "Expirments",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LLO_MA",
                table: "Expirments");

            migrationBuilder.DropColumn(
                name: "LLO_SA",
                table: "Expirments");
        }
    }
}
