using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class fixdepartmentcoordinatorrelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Coordinators_DEPARTMENT_ID",
                table: "Departments");

            migrationBuilder.AlterColumn<int>(
                name: "DEPARTMENT_ID",
                table: "Departments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DEPARTMENT_COORDINATOR_ID",
                table: "Departments",
                column: "DEPARTMENT_COORDINATOR_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Coordinators_DEPARTMENT_COORDINATOR_ID",
                table: "Departments",
                column: "DEPARTMENT_COORDINATOR_ID",
                principalTable: "Coordinators",
                principalColumn: "COORDINATOR_ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Coordinators_DEPARTMENT_COORDINATOR_ID",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_DEPARTMENT_COORDINATOR_ID",
                table: "Departments");

            migrationBuilder.AlterColumn<int>(
                name: "DEPARTMENT_ID",
                table: "Departments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Coordinators_DEPARTMENT_ID",
                table: "Departments",
                column: "DEPARTMENT_ID",
                principalTable: "Coordinators",
                principalColumn: "COORDINATOR_ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
