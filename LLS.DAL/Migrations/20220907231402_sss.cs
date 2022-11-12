using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LLS.DAL.Migrations
{
    public partial class sss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentSessions_Machines_MachineId",
                table: "StudentSessions");

            migrationBuilder.AlterColumn<Guid>(
                name: "MachineId",
                table: "StudentSessions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSessions_Machines_MachineId",
                table: "StudentSessions",
                column: "MachineId",
                principalTable: "Machines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentSessions_Machines_MachineId",
                table: "StudentSessions");

            migrationBuilder.AlterColumn<Guid>(
                name: "MachineId",
                table: "StudentSessions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSessions_Machines_MachineId",
                table: "StudentSessions",
                column: "MachineId",
                principalTable: "Machines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
