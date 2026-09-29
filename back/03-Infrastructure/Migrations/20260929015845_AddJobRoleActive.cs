using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoPrumo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobRoleActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "job_role",
                newName: "job_roles");

            migrationBuilder.RenameIndex(
                name: "ix_job_role_name_department_id",
                table: "job_roles",
                newName: "ix_job_roles_name_department_id");

            migrationBuilder.RenameIndex(
                name: "ix_job_role_department_id",
                table: "job_roles",
                newName: "ix_job_roles_department_id");

            migrationBuilder.AddColumn<bool>(
                name: "active",
                table: "job_roles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "active",
                table: "job_roles");

            migrationBuilder.RenameTable(
                name: "job_roles",
                newName: "job_role");

            migrationBuilder.RenameIndex(
                name: "ix_job_roles_name_department_id",
                table: "job_role",
                newName: "ix_job_role_name_department_id");

            migrationBuilder.RenameIndex(
                name: "ix_job_roles_department_id",
                table: "job_role",
                newName: "ix_job_role_department_id");
        }
    }
}
