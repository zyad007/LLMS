using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LLS.DAL.Migrations
{
    public partial class _333 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Answers",
                table: "Student_ExpCourses");

            migrationBuilder.DropColumn(
                name: "Grading",
                table: "Student_ExpCourses");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Student_ExpCourses");

            migrationBuilder.CreateTable(
                name: "Trials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Student_ExpCourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsGraded = table.Column<bool>(type: "boolean", nullable: false),
                    TotalScore = table.Column<float>(type: "real", nullable: false),
                    TotalTimeInMin = table.Column<float>(type: "real", nullable: false),
                    LRO_SA = table.Column<string>(type: "text", nullable: true),
                    LRO = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trials_Student_ExpCourses_Student_ExpCourseId",
                        column: x => x.Student_ExpCourseId,
                        principalTable: "Student_ExpCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trials_Student_ExpCourseId",
                table: "Trials",
                column: "Student_ExpCourseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trials");

            migrationBuilder.AddColumn<string>(
                name: "Answers",
                table: "Student_ExpCourses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Grading",
                table: "Student_ExpCourses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Student_ExpCourses",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
