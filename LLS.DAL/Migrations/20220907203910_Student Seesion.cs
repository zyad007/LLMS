using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LLS.DAL.Migrations
{
    public partial class StudentSeesion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpCourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    MachineId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeSlot = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentSessions_Exp_Courses_ExpCourseId",
                        column: x => x.ExpCourseId,
                        principalTable: "Exp_Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentSessions_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentSessions_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSessions_ExpCourseId",
                table: "StudentSessions",
                column: "ExpCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSessions_MachineId",
                table: "StudentSessions",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSessions_StudentId",
                table: "StudentSessions",
                column: "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentSessions");
        }
    }
}
