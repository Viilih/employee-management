using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coordinators",
                columns: table => new
                {
                    COORDINATOR_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    COORDINATOR_FIRST_NAME = table.Column<string>(type: "text", nullable: false),
                    COORDINATOR_LAST_NAME = table.Column<string>(type: "text", nullable: false),
                    COORDINATOR_EMAIL = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coordinators", x => x.COORDINATOR_ID);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DEPARTMENT_ID = table.Column<int>(type: "integer", nullable: false),
                    DEPARTMENT_SECTOR = table.Column<string>(type: "text", nullable: false),
                    DEPARTMENT_COORDINATOR_ID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DEPARTMENT_ID);
                    table.ForeignKey(
                        name: "FK_Departments_Coordinators_DEPARTMENT_ID",
                        column: x => x.DEPARTMENT_ID,
                        principalTable: "Coordinators",
                        principalColumn: "COORDINATOR_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EMPLOYEE_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EMPLOYEE_FIRST_NAME = table.Column<string>(type: "text", nullable: false),
                    EMPLOYEE_LAST_NAME = table.Column<string>(type: "text", nullable: false),
                    EMPLOYEE_EMAIL = table.Column<string>(type: "text", nullable: false),
                    EMPLOYEE_DEPARTMENT = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EMPLOYEE_ID);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_EMPLOYEE_DEPARTMENT",
                        column: x => x.EMPLOYEE_DEPARTMENT,
                        principalTable: "Departments",
                        principalColumn: "DEPARTMENT_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EMPLOYEE_DEPARTMENT",
                table: "Employees",
                column: "EMPLOYEE_DEPARTMENT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Coordinators");
        }
    }
}
