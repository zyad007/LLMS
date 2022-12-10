using Microsoft.EntityFrameworkCore.Migrations;

namespace LLS.DAL.Migrations
{
    public partial class sas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trials_StudentCourse_ExpCourses_Student_ExpCourseId",
                table: "Trials");

            migrationBuilder.RenameColumn(
                name: "Student_ExpCourseId",
                table: "Trials",
                newName: "StudentCourse_ExpCourseId");

            migrationBuilder.RenameColumn(
                name: "LRO_SA",
                table: "Trials",
                newName: "LLA");

            migrationBuilder.RenameIndex(
                name: "IX_Trials_Student_ExpCourseId",
                table: "Trials",
                newName: "IX_Trials_StudentCourse_ExpCourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trials_StudentCourse_ExpCourses_StudentCourse_ExpCourseId",
                table: "Trials",
                column: "StudentCourse_ExpCourseId",
                principalTable: "StudentCourse_ExpCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trials_StudentCourse_ExpCourses_StudentCourse_ExpCourseId",
                table: "Trials");

            migrationBuilder.RenameColumn(
                name: "StudentCourse_ExpCourseId",
                table: "Trials",
                newName: "Student_ExpCourseId");

            migrationBuilder.RenameColumn(
                name: "LLA",
                table: "Trials",
                newName: "LRO_SA");

            migrationBuilder.RenameIndex(
                name: "IX_Trials_StudentCourse_ExpCourseId",
                table: "Trials",
                newName: "IX_Trials_Student_ExpCourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trials_StudentCourse_ExpCourses_Student_ExpCourseId",
                table: "Trials",
                column: "Student_ExpCourseId",
                principalTable: "StudentCourse_ExpCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
