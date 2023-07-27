using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LLS.DAL.Migrations
{
    public partial class asdadadad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndAt",
                table: "StudentCourse_ExpCourses",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ReservedDay",
                table: "StudentCourse_ExpCourses",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartFrom",
                table: "StudentCourse_ExpCourses",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "StudentCourse_ExpCourses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeSlot",
                table: "StudentCourse_ExpCourses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndAt",
                table: "StudentCourse_ExpCourses");

            migrationBuilder.DropColumn(
                name: "ReservedDay",
                table: "StudentCourse_ExpCourses");

            migrationBuilder.DropColumn(
                name: "StartFrom",
                table: "StudentCourse_ExpCourses");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "StudentCourse_ExpCourses");

            migrationBuilder.DropColumn(
                name: "TimeSlot",
                table: "StudentCourse_ExpCourses");
        }
    }
}
