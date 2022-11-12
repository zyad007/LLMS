using Microsoft.EntityFrameworkCore.Migrations;

namespace LLS.DAL.Migrations
{
    public partial class _451 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGraded",
                table: "Trials");

            migrationBuilder.AddColumn<int>(
                name: "TrialNumber",
                table: "Trials",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTials",
                table: "Student_ExpCourses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrialNumber",
                table: "Trials");

            migrationBuilder.DropColumn(
                name: "NumberOfTials",
                table: "Student_ExpCourses");

            migrationBuilder.AddColumn<bool>(
                name: "IsGraded",
                table: "Trials",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
