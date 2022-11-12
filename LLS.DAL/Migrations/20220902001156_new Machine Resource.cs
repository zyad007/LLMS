using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LLS.DAL.Migrations
{
    public partial class newMachineResource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resources_Machines_MachineId",
                table: "Resources");

            migrationBuilder.DropIndex(
                name: "IX_Resources_MachineId",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "MachineId",
                table: "Resources");

            migrationBuilder.CreateTable(
                name: "Resource_Machines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MachineId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resource_Machines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resource_Machines_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resource_Machines_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Resource_Machines_MachineId",
                table: "Resource_Machines",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_Machines_ResourceId",
                table: "Resource_Machines",
                column: "ResourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Resource_Machines");

            migrationBuilder.AddColumn<Guid>(
                name: "MachineId",
                table: "Resources",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resources_MachineId",
                table: "Resources",
                column: "MachineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Resources_Machines_MachineId",
                table: "Resources",
                column: "MachineId",
                principalTable: "Machines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
