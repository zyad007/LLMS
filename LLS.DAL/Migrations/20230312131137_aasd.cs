using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LLS.DAL.Migrations
{
    public partial class aasd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Update",
                table: "User_Courses",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<float>(
                name: "FinalGrade",
                table: "StudentCourse_ExpCourses",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "RelatedCourse",
                table: "Expirments",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Update",
                table: "User_Courses");

            migrationBuilder.DropColumn(
                name: "FinalGrade",
                table: "StudentCourse_ExpCourses");

            migrationBuilder.DropColumn(
                name: "RelatedCourse",
                table: "Expirments");
        }
    }
}
