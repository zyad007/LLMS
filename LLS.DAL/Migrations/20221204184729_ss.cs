using Microsoft.EntityFrameworkCore.Migrations;

namespace LLS.DAL.Migrations
{
    public partial class ss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resource_Exp_Exp_Courses_Exp_CourseId",
                table: "Resource_Exp");

            migrationBuilder.DropForeignKey(
                name: "FK_Resource_Exp_Resource_ResourceId",
                table: "Resource_Exp");

            migrationBuilder.DropForeignKey(
                name: "FK_Resource_Machine_Resource_ResourceId",
                table: "Resource_Machine");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Resource_Exp",
                table: "Resource_Exp");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Resource",
                table: "Resource");

            migrationBuilder.RenameTable(
                name: "Resource_Exp",
                newName: "Resource_Exps");

            migrationBuilder.RenameTable(
                name: "Resource",
                newName: "Resources");

            migrationBuilder.RenameColumn(
                name: "Exp_CourseId",
                table: "Resource_Exps",
                newName: "ExperimentId");

            migrationBuilder.RenameIndex(
                name: "IX_Resource_Exp_ResourceId",
                table: "Resource_Exps",
                newName: "IX_Resource_Exps_ResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_Resource_Exp_Exp_CourseId",
                table: "Resource_Exps",
                newName: "IX_Resource_Exps_ExperimentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Resource_Exps",
                table: "Resource_Exps",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Resources",
                table: "Resources",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Resource_Exps_Expirments_ExperimentId",
                table: "Resource_Exps",
                column: "ExperimentId",
                principalTable: "Expirments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Resource_Exps_Resources_ResourceId",
                table: "Resource_Exps",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Resource_Machine_Resources_ResourceId",
                table: "Resource_Machine",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resource_Exps_Expirments_ExperimentId",
                table: "Resource_Exps");

            migrationBuilder.DropForeignKey(
                name: "FK_Resource_Exps_Resources_ResourceId",
                table: "Resource_Exps");

            migrationBuilder.DropForeignKey(
                name: "FK_Resource_Machine_Resources_ResourceId",
                table: "Resource_Machine");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Resources",
                table: "Resources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Resource_Exps",
                table: "Resource_Exps");

            migrationBuilder.RenameTable(
                name: "Resources",
                newName: "Resource");

            migrationBuilder.RenameTable(
                name: "Resource_Exps",
                newName: "Resource_Exp");

            migrationBuilder.RenameColumn(
                name: "ExperimentId",
                table: "Resource_Exp",
                newName: "Exp_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Resource_Exps_ResourceId",
                table: "Resource_Exp",
                newName: "IX_Resource_Exp_ResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_Resource_Exps_ExperimentId",
                table: "Resource_Exp",
                newName: "IX_Resource_Exp_Exp_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Resource",
                table: "Resource",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Resource_Exp",
                table: "Resource_Exp",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Resource_Exp_Exp_Courses_Exp_CourseId",
                table: "Resource_Exp",
                column: "Exp_CourseId",
                principalTable: "Exp_Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Resource_Exp_Resource_ResourceId",
                table: "Resource_Exp",
                column: "ResourceId",
                principalTable: "Resource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Resource_Machine_Resource_ResourceId",
                table: "Resource_Machine",
                column: "ResourceId",
                principalTable: "Resource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
