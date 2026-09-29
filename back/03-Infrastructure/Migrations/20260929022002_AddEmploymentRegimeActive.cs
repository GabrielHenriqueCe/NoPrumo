using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoPrumo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmploymentRegimeActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "employment_regime",
                newName: "employment_regimes");

            migrationBuilder.RenameIndex(
                name: "ix_employment_regime_label",
                table: "employment_regimes",
                newName: "ix_employment_regimes_label");

            migrationBuilder.AddColumn<bool>(
                name: "active",
                table: "employment_regimes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "active",
                table: "employment_regimes");

            migrationBuilder.RenameTable(
                name: "employment_regimes",
                newName: "employment_regime");

            migrationBuilder.RenameIndex(
                name: "ix_employment_regimes_label",
                table: "employment_regime",
                newName: "ix_employment_regime_label");
        }
    }
}
