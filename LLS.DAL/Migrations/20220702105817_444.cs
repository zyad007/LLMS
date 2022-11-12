using Microsoft.EntityFrameworkCore.Migrations;

namespace LLS.DAL.Migrations
{
    public partial class _444 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumbersOfTrials",
                table: "Exp_Courses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumbersOfTrials",
                table: "Exp_Courses");
        }
    }
}
