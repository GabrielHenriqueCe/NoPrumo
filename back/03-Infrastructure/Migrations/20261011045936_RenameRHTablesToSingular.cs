using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoPrumo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameRHTablesToSingular : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "job_roles",
                newName: "job_role");

            migrationBuilder.RenameTable(
                name: "employment_regimes",
                newName: "employment_regime");

            migrationBuilder.RenameIndex(
                name: "ix_job_roles_name_department_id",
                table: "job_role",
                newName: "ix_job_role_name_department_id");

            migrationBuilder.RenameIndex(
                name: "ix_job_roles_department_id",
                table: "job_role",
                newName: "ix_job_role_department_id");

            migrationBuilder.RenameIndex(
                name: "ix_employment_regimes_label",
                table: "employment_regime",
                newName: "ix_employment_regime_label");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "job_role",
                newName: "job_roles");

            migrationBuilder.RenameTable(
                name: "employment_regime",
                newName: "employment_regimes");

            migrationBuilder.RenameIndex(
                name: "ix_job_role_name_department_id",
                table: "job_roles",
                newName: "ix_job_roles_name_department_id");

            migrationBuilder.RenameIndex(
                name: "ix_job_role_department_id",
                table: "job_roles",
                newName: "ix_job_roles_department_id");

            migrationBuilder.RenameIndex(
                name: "ix_employment_regime_label",
                table: "employment_regimes",
                newName: "ix_employment_regimes_label");
        }
    }
}
