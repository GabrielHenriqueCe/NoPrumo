using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoPrumo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "client",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    person_type = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, defaultValueSql: "'company'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    document_encrypted = table.Column<byte[]>(type: "varbinary(512)", maxLength: 512, nullable: true),
                    document_hash = table.Column<string>(type: "char(64)", fixedLength: true, maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    document_masked = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contact_name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mobile = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    street = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    number = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    complement = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    district = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    city = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    state = table.Column<string>(type: "char(2)", fixedLength: true, maxLength: 2, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    postal_code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notes = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    active_key = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true, computedColumnSql: "ifnull(`deleted_at`,'1970-01-01 00:00:00')", stored: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "department",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "employment_regime",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    label = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    unit = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    monthly_hours = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "permission",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "stock_category",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tracks_project_balance = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    requires_return = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "supplier",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    document_encrypted = table.Column<byte[]>(type: "varbinary(512)", maxLength: 512, nullable: true),
                    document_hash = table.Column<string>(type: "char(64)", fixedLength: true, maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    document_masked = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contact_name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    city = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    state = table.Column<string>(type: "char(2)", fixedLength: true, maxLength: 2, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notes = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    active_key = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true, computedColumnSql: "ifnull(`deleted_at`,'1970-01-01 00:00:00')", stored: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "training_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    validity_months = table.Column<int>(type: "int", nullable: true),
                    min_workload_hours = table.Column<int>(type: "int", nullable: true),
                    requires_in_person = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    notes = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "job_role",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    department_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_job_role_department",
                        column: x => x.department_id,
                        principalTable: "department",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "team",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    department_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    active_key = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true, computedColumnSql: "ifnull(`deleted_at`,'1970-01-01 00:00:00')", stored: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_team_department",
                        column: x => x.department_id,
                        principalTable: "department",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "payroll_charge_rate",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    employment_regime_id = table.Column<long>(type: "bigint", nullable: false),
                    percentage = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    effective_start = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_end = table.Column<DateOnly>(type: "date", nullable: true),
                    source = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notes = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_payroll_charge_rate_regime",
                        column: x => x.employment_regime_id,
                        principalTable: "employment_regime",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "role_permission",
                columns: table => new
                {
                    role_id = table.Column<long>(type: "bigint", nullable: false),
                    permission_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.role_id, x.permission_id })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "fk_role_permission_permission",
                        column: x => x.permission_id,
                        principalTable: "permission",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_role_permission_role",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "stock_group",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    stock_category_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_stock_group_category",
                        column: x => x.stock_category_id,
                        principalTable: "stock_category",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "employee",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    registration_number = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    job_role_id = table.Column<long>(type: "bigint", nullable: true),
                    employment_regime_id = table.Column<long>(type: "bigint", nullable: true),
                    pay_rate = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    additional_percentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    hire_date = table.Column<DateOnly>(type: "date", nullable: true),
                    termination_date = table.Column<DateOnly>(type: "date", nullable: true),
                    phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    document_encrypted = table.Column<byte[]>(type: "varbinary(512)", maxLength: 512, nullable: true),
                    document_hash = table.Column<string>(type: "char(64)", fixedLength: true, maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    document_masked = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'"),
                    anonymized_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    active_key = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true, computedColumnSql: "ifnull(`deleted_at`,'1970-01-01 00:00:00')", stored: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_employee_job_role",
                        column: x => x.job_role_id,
                        principalTable: "job_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_employee_regime",
                        column: x => x.employment_regime_id,
                        principalTable: "employment_regime",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "stock_item",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    stock_group_id = table.Column<long>(type: "bigint", nullable: false),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    unit = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    min_quantity = table.Column<decimal>(type: "decimal(15,3)", precision: 15, scale: 3, nullable: false),
                    reference_price = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    ca = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ca_expiry_date = table.Column<DateOnly>(type: "date", nullable: true),
                    active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    active_key = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true, computedColumnSql: "ifnull(`deleted_at`,'1970-01-01 00:00:00')", stored: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_stock_item_group",
                        column: x => x.stock_group_id,
                        principalTable: "stock_group",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "employee_team",
                columns: table => new
                {
                    employee_id = table.Column<long>(type: "bigint", nullable: false),
                    team_id = table.Column<long>(type: "bigint", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.employee_id, x.team_id, x.start_date })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });
                    table.ForeignKey(
                        name: "fk_employee_team_employee",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_employee_team_team",
                        column: x => x.team_id,
                        principalTable: "team",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "employee_training",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    employee_id = table.Column<long>(type: "bigint", nullable: false),
                    training_type_id = table.Column<long>(type: "bigint", nullable: false),
                    issue_date = table.Column<DateOnly>(type: "date", nullable: false),
                    expiry_date = table.Column<DateOnly>(type: "date", nullable: true),
                    workload_hours = table.Column<int>(type: "int", nullable: true),
                    modality = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    instructor = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    certificate_number = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    attachment_url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_employee_training_employee",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_employee_training_type",
                        column: x => x.training_type_id,
                        principalTable: "training_type",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    username = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password_hash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    role_id = table.Column<long>(type: "bigint", nullable: false),
                    employee_id = table.Column<long>(type: "bigint", nullable: true),
                    active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'"),
                    last_login_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    failed_attempts = table.Column<int>(type: "int", nullable: false),
                    locked_until = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_employee",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_user_role",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "audit_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: true),
                    table_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    record_id = table.Column<long>(type: "bigint", nullable: true),
                    action = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    changed_fields = table.Column<string>(type: "json", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    old_data = table.Column<string>(type: "json", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    new_data = table.Column<string>(type: "json", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ip = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_agent = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_audit_log_user",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "project",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    client_id = table.Column<long>(type: "bigint", nullable: true),
                    name = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cno = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    street = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    number = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    complement = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    district = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    city = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    state = table.Column<string>(type: "char(2)", fixedLength: true, maxLength: 2, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    postal_code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contract_amount = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'planning'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    supervisor_id = table.Column<long>(type: "bigint", nullable: true),
                    technical_manager = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    crea_rt = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    forecast_date = table.Column<DateOnly>(type: "date", nullable: true),
                    completion_date = table.Column<DateOnly>(type: "date", nullable: true),
                    closed_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    closed_by = table.Column<long>(type: "bigint", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    active_key = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true, computedColumnSql: "ifnull(`deleted_at`,'1970-01-01 00:00:00')", stored: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_project_client",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_project_closed_by",
                        column: x => x.closed_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_project_supervisor",
                        column: x => x.supervisor_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "account_payable",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    supplier_id = table.Column<long>(type: "bigint", nullable: true),
                    project_id = table.Column<long>(type: "bigint", nullable: true),
                    description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    invoice_number = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    amount = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    paid_amount = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    due_date = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'pending'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    source_type = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    source_id = table.Column<long>(type: "bigint", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_account_payable_project",
                        column: x => x.project_id,
                        principalTable: "project",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_account_payable_supplier",
                        column: x => x.supplier_id,
                        principalTable: "supplier",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "account_receivable",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    client_id = table.Column<long>(type: "bigint", nullable: true),
                    project_id = table.Column<long>(type: "bigint", nullable: true),
                    description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    invoice_number = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    amount = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    paid_amount = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    due_date = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'pending'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notes = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_account_receivable_client",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_account_receivable_project",
                        column: x => x.project_id,
                        principalTable: "project",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "appointment",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    time = table.Column<TimeOnly>(type: "time", nullable: true),
                    title = table.Column<string>(type: "varchar(220)", maxLength: 220, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    project_id = table.Column<long>(type: "bigint", nullable: true),
                    assigned_to_id = table.Column<long>(type: "bigint", nullable: true),
                    type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'project'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    completed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    completed_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_appointment_assigned_to",
                        column: x => x.assigned_to_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_appointment_project",
                        column: x => x.project_id,
                        principalTable: "project",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "ppe_issue",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    employee_id = table.Column<long>(type: "bigint", nullable: false),
                    project_id = table.Column<long>(type: "bigint", nullable: true),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    issued_by_id = table.Column<long>(type: "bigint", nullable: true),
                    signed_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    signature_url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    signature_hash = table.Column<string>(type: "char(64)", fixedLength: true, maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_ppe_issue_employee",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ppe_issue_issued_by",
                        column: x => x.issued_by_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_ppe_issue_project",
                        column: x => x.project_id,
                        principalTable: "project",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "project_amendment",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    project_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    number = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    extra_days = table.Column<int>(type: "int", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    approved_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_project_amendment_approver",
                        column: x => x.approved_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_project_amendment_project",
                        column: x => x.project_id,
                        principalTable: "project",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "project_link",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    project_id = table.Column<long>(type: "bigint", nullable: false),
                    token_hash = table.Column<string>(type: "char(64)", fixedLength: true, maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    label = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expires_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    revoked_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    last_access_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    access_count = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_project_link_creator",
                        column: x => x.created_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_project_link_project",
                        column: x => x.project_id,
                        principalTable: "project",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "purchase_request",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    project_id = table.Column<long>(type: "bigint", nullable: false),
                    requested_by = table.Column<long>(type: "bigint", nullable: true),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    needed_by_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'pending'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notes = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    decided_by = table.Column<long>(type: "bigint", nullable: true),
                    decided_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    rejection_reason = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_purchase_request_decider",
                        column: x => x.decided_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_purchase_request_project",
                        column: x => x.project_id,
                        principalTable: "project",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_purchase_request_requester",
                        column: x => x.requested_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "stage",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    project_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    team_id = table.Column<long>(type: "bigint", nullable: true),
                    supervisor_id = table.Column<long>(type: "bigint", nullable: true),
                    planned_date = table.Column<DateOnly>(type: "date", nullable: true),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    completion_date = table.Column<DateOnly>(type: "date", nullable: true),
                    percentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'planned'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notes = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    marked_by = table.Column<long>(type: "bigint", nullable: true),
                    marked_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_stage_marked_by",
                        column: x => x.marked_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_stage_project",
                        column: x => x.project_id,
                        principalTable: "project",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_stage_supervisor",
                        column: x => x.supervisor_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_stage_team",
                        column: x => x.team_id,
                        principalTable: "team",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "stock_movement",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    stock_item_id = table.Column<long>(type: "bigint", nullable: false),
                    project_id = table.Column<long>(type: "bigint", nullable: true),
                    employee_id = table.Column<long>(type: "bigint", nullable: true),
                    type = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    quantity = table.Column<decimal>(type: "decimal(15,3)", precision: 15, scale: 3, nullable: false),
                    unit = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    unit_cost = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    supplier_id = table.Column<long>(type: "bigint", nullable: true),
                    invoice_number = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    source_type = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    source_id = table.Column<long>(type: "bigint", nullable: true),
                    transfer_id = table.Column<long>(type: "bigint", nullable: true),
                    notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    date = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    recorded_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_stock_movement_employee",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_stock_movement_item",
                        column: x => x.stock_item_id,
                        principalTable: "stock_item",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_stock_movement_project",
                        column: x => x.project_id,
                        principalTable: "project",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_stock_movement_recorder",
                        column: x => x.recorded_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_stock_movement_supplier",
                        column: x => x.supplier_id,
                        principalTable: "supplier",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "team_project",
                columns: table => new
                {
                    team_id = table.Column<long>(type: "bigint", nullable: false),
                    project_id = table.Column<long>(type: "bigint", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.team_id, x.project_id, x.start_date })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });
                    table.ForeignKey(
                        name: "fk_team_project_project",
                        column: x => x.project_id,
                        principalTable: "project",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_team_project_team",
                        column: x => x.team_id,
                        principalTable: "team",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "time_entry",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    employee_id = table.Column<long>(type: "bigint", nullable: false),
                    project_id = table.Column<long>(type: "bigint", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    clock_in = table.Column<TimeOnly>(type: "time", nullable: true),
                    break_start = table.Column<TimeOnly>(type: "time", nullable: true),
                    break_end = table.Column<TimeOnly>(type: "time", nullable: true),
                    clock_out = table.Column<TimeOnly>(type: "time", nullable: true),
                    hours = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true, computedColumnSql: "round((greatest((ifnull(time_to_sec(timediff(`clock_out`,`clock_in`)),0) - ifnull(time_to_sec(timediff(`break_end`,`break_start`)),0)),0) / 3600),2)", stored: true),
                    hourly_rate_snapshot = table.Column<decimal>(type: "decimal(15,4)", precision: 15, scale: 4, nullable: false),
                    additional_snapshot = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    charges_snapshot = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    cost = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    source = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'manual'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    external_id = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    recorded_by = table.Column<long>(type: "bigint", nullable: true),
                    notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_time_entry_employee",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_time_entry_project",
                        column: x => x.project_id,
                        principalTable: "project",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_time_entry_recorder",
                        column: x => x.recorded_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "user_project",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    project_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.user_id, x.project_id })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "fk_user_project_project",
                        column: x => x.project_id,
                        principalTable: "project",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_project_user",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    account_receivable_id = table.Column<long>(type: "bigint", nullable: true),
                    account_payable_id = table.Column<long>(type: "bigint", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    payment_date = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    payment_method = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    recorded_by = table.Column<long>(type: "bigint", nullable: true),
                    notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_payment_payable",
                        column: x => x.account_payable_id,
                        principalTable: "account_payable",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_payment_receivable",
                        column: x => x.account_receivable_id,
                        principalTable: "account_receivable",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_payment_recorder",
                        column: x => x.recorded_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "ppe_issue_item",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ppe_issue_id = table.Column<long>(type: "bigint", nullable: false),
                    stock_item_id = table.Column<long>(type: "bigint", nullable: true),
                    item_name_snapshot = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ca_snapshot = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    quantity = table.Column<decimal>(type: "decimal(15,3)", precision: 15, scale: 3, nullable: false, defaultValueSql: "'1.000'"),
                    unit = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'issued'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    issue_date = table.Column<DateOnly>(type: "date", nullable: true),
                    return_date = table.Column<DateOnly>(type: "date", nullable: true),
                    notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_ppe_issue_item_issue",
                        column: x => x.ppe_issue_id,
                        principalTable: "ppe_issue",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ppe_issue_item_item",
                        column: x => x.stock_item_id,
                        principalTable: "stock_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "purchase_request_item",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    purchase_request_id = table.Column<long>(type: "bigint", nullable: false),
                    stock_item_id = table.Column<long>(type: "bigint", nullable: false),
                    requested_quantity = table.Column<decimal>(type: "decimal(15,3)", precision: 15, scale: 3, nullable: false),
                    fulfilled_quantity = table.Column<decimal>(type: "decimal(15,3)", precision: 15, scale: 3, nullable: false),
                    unit = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_purchase_request_item_item",
                        column: x => x.stock_item_id,
                        principalTable: "stock_item",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_purchase_request_item_request",
                        column: x => x.purchase_request_id,
                        principalTable: "purchase_request",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "subcontract",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    project_id = table.Column<long>(type: "bigint", nullable: false),
                    supplier_id = table.Column<long>(type: "bigint", nullable: true),
                    stage_id = table.Column<long>(type: "bigint", nullable: true),
                    description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price_type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'global'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    total_amount = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: true),
                    unit = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    planned_quantity = table.Column<decimal>(type: "decimal(15,3)", precision: 15, scale: 3, nullable: true),
                    inss_retention_pct = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'active'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notes = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_subcontract_project",
                        column: x => x.project_id,
                        principalTable: "project",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_subcontract_stage",
                        column: x => x.stage_id,
                        principalTable: "stage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_subcontract_supplier",
                        column: x => x.supplier_id,
                        principalTable: "supplier",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "subcontract_measurement",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    subcontract_id = table.Column<long>(type: "bigint", nullable: false),
                    number = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    quantity = table.Column<decimal>(type: "decimal(15,3)", precision: 15, scale: 3, nullable: true),
                    percentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    amount = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    approved_by = table.Column<long>(type: "bigint", nullable: true),
                    approved_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_subcontract_measurement_approver",
                        column: x => x.approved_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_subcontract_measurement_subcontract",
                        column: x => x.subcontract_id,
                        principalTable: "subcontract",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "ix_account_payable_due_date_status",
                table: "account_payable",
                columns: new[] { "due_date", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_account_payable_project_id",
                table: "account_payable",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_account_payable_source_type_source_id",
                table: "account_payable",
                columns: new[] { "source_type", "source_id" });

            migrationBuilder.CreateIndex(
                name: "ix_account_payable_supplier_id",
                table: "account_payable",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "ix_account_receivable_client_id",
                table: "account_receivable",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_account_receivable_due_date_status",
                table: "account_receivable",
                columns: new[] { "due_date", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_account_receivable_project_id",
                table: "account_receivable",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_assigned_to_id",
                table: "appointment",
                column: "assigned_to_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_date_time",
                table: "appointment",
                columns: new[] { "date", "time" });

            migrationBuilder.CreateIndex(
                name: "ix_appointment_project_id",
                table: "appointment",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_created_at",
                table: "audit_log",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_table_name_record_id",
                table: "audit_log",
                columns: new[] { "table_name", "record_id" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_user_id_created_at",
                table: "audit_log",
                columns: new[] { "user_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_client_city",
                table: "client",
                column: "city");

            migrationBuilder.CreateIndex(
                name: "ix_client_deleted_at",
                table: "client",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "ix_client_document_hash_active_key",
                table: "client",
                columns: new[] { "document_hash", "active_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_client_name",
                table: "client",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_department_name",
                table: "department",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_employee_active",
                table: "employee",
                column: "active");

            migrationBuilder.CreateIndex(
                name: "ix_employee_deleted_at",
                table: "employee",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "ix_employee_document_hash_active_key",
                table: "employee",
                columns: new[] { "document_hash", "active_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_employee_employment_regime_id",
                table: "employee",
                column: "employment_regime_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_job_role_id",
                table: "employee",
                column: "job_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_registration_number_active_key",
                table: "employee",
                columns: new[] { "registration_number", "active_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_employee_team_team_id",
                table: "employee_team",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_training_employee_id",
                table: "employee_training",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_training_expiry_date",
                table: "employee_training",
                column: "expiry_date");

            migrationBuilder.CreateIndex(
                name: "ix_employee_training_training_type_id",
                table: "employee_training",
                column: "training_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_employment_regime_label",
                table: "employment_regime",
                column: "label",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_job_role_department_id",
                table: "job_role",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_job_role_name_department_id",
                table: "job_role",
                columns: new[] { "name", "department_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_payment_account_payable_id",
                table: "payment",
                column: "account_payable_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_account_receivable_id",
                table: "payment",
                column: "account_receivable_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_payment_date",
                table: "payment",
                column: "payment_date");

            migrationBuilder.CreateIndex(
                name: "ix_payment_recorded_by",
                table: "payment",
                column: "recorded_by");

            migrationBuilder.CreateIndex(
                name: "ix_payroll_charge_rate_employment_regime_id_effective_start",
                table: "payroll_charge_rate",
                columns: new[] { "employment_regime_id", "effective_start" });

            migrationBuilder.CreateIndex(
                name: "ix_permission_code",
                table: "permission",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ppe_issue_employee_id_date",
                table: "ppe_issue",
                columns: new[] { "employee_id", "date" });

            migrationBuilder.CreateIndex(
                name: "ix_ppe_issue_issued_by_id",
                table: "ppe_issue",
                column: "issued_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_ppe_issue_project_id",
                table: "ppe_issue",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_ppe_issue_item_ppe_issue_id",
                table: "ppe_issue_item",
                column: "ppe_issue_id");

            migrationBuilder.CreateIndex(
                name: "ix_ppe_issue_item_status",
                table: "ppe_issue_item",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_ppe_issue_item_stock_item_id",
                table: "ppe_issue_item",
                column: "stock_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_client_id",
                table: "project",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_client_id_status",
                table: "project",
                columns: new[] { "client_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_project_closed_by",
                table: "project",
                column: "closed_by");

            migrationBuilder.CreateIndex(
                name: "ix_project_code_active_key",
                table: "project",
                columns: new[] { "code", "active_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_project_deleted_at",
                table: "project",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "ix_project_forecast_date",
                table: "project",
                column: "forecast_date");

            migrationBuilder.CreateIndex(
                name: "ix_project_status",
                table: "project",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_project_supervisor_id",
                table: "project",
                column: "supervisor_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_amendment_approved_by",
                table: "project_amendment",
                column: "approved_by");

            migrationBuilder.CreateIndex(
                name: "ix_project_amendment_project_id",
                table: "project_amendment",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_link_created_by",
                table: "project_link",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_project_link_project_id",
                table: "project_link",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_link_token_hash",
                table: "project_link",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_purchase_request_date",
                table: "purchase_request",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_request_decided_by",
                table: "purchase_request",
                column: "decided_by");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_request_project_id_status",
                table: "purchase_request",
                columns: new[] { "project_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_purchase_request_requested_by",
                table: "purchase_request",
                column: "requested_by");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_request_item_purchase_request_id",
                table: "purchase_request_item",
                column: "purchase_request_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_request_item_stock_item_id",
                table: "purchase_request_item",
                column: "stock_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_name",
                table: "role",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_role_permission_permission_id",
                table: "role_permission",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "ix_stage_marked_by",
                table: "stage",
                column: "marked_by");

            migrationBuilder.CreateIndex(
                name: "ix_stage_planned_date",
                table: "stage",
                column: "planned_date");

            migrationBuilder.CreateIndex(
                name: "ix_stage_project_id",
                table: "stage",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_stage_project_id_status",
                table: "stage",
                columns: new[] { "project_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_stage_supervisor_id",
                table: "stage",
                column: "supervisor_id");

            migrationBuilder.CreateIndex(
                name: "ix_stage_team_id",
                table: "stage",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "ix_stock_category_name",
                table: "stock_category",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_stock_group_name_stock_category_id",
                table: "stock_group",
                columns: new[] { "name", "stock_category_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_stock_group_stock_category_id",
                table: "stock_group",
                column: "stock_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_stock_item_active",
                table: "stock_item",
                column: "active");

            migrationBuilder.CreateIndex(
                name: "ix_stock_item_code_active_key",
                table: "stock_item",
                columns: new[] { "code", "active_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_stock_item_deleted_at",
                table: "stock_item",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "ix_stock_item_name",
                table: "stock_item",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_stock_item_stock_group_id",
                table: "stock_item",
                column: "stock_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_stock_movement_employee_id_date",
                table: "stock_movement",
                columns: new[] { "employee_id", "date" });

            migrationBuilder.CreateIndex(
                name: "ix_stock_movement_project_id_date",
                table: "stock_movement",
                columns: new[] { "project_id", "date" });

            migrationBuilder.CreateIndex(
                name: "ix_stock_movement_recorded_by",
                table: "stock_movement",
                column: "recorded_by");

            migrationBuilder.CreateIndex(
                name: "ix_stock_movement_source_type_source_id",
                table: "stock_movement",
                columns: new[] { "source_type", "source_id" });

            migrationBuilder.CreateIndex(
                name: "ix_stock_movement_stock_item_id_date",
                table: "stock_movement",
                columns: new[] { "stock_item_id", "date" });

            migrationBuilder.CreateIndex(
                name: "ix_stock_movement_stock_item_id_project_id",
                table: "stock_movement",
                columns: new[] { "stock_item_id", "project_id" });

            migrationBuilder.CreateIndex(
                name: "ix_stock_movement_supplier_id",
                table: "stock_movement",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "ix_stock_movement_transfer_id",
                table: "stock_movement",
                column: "transfer_id");

            migrationBuilder.CreateIndex(
                name: "ix_subcontract_project_id",
                table: "subcontract",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_subcontract_stage_id",
                table: "subcontract",
                column: "stage_id");

            migrationBuilder.CreateIndex(
                name: "ix_subcontract_supplier_id",
                table: "subcontract",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "ix_subcontract_measurement_approved_by",
                table: "subcontract_measurement",
                column: "approved_by");

            migrationBuilder.CreateIndex(
                name: "ix_subcontract_measurement_date",
                table: "subcontract_measurement",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "ix_subcontract_measurement_subcontract_id_number",
                table: "subcontract_measurement",
                columns: new[] { "subcontract_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_supplier_document_hash_active_key",
                table: "supplier",
                columns: new[] { "document_hash", "active_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_supplier_name",
                table: "supplier",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_team_department_id",
                table: "team",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_name_department_id_active_key",
                table: "team",
                columns: new[] { "name", "department_id", "active_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_team_project_project_id",
                table: "team_project",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_time_entry_employee_id_date",
                table: "time_entry",
                columns: new[] { "employee_id", "date" });

            migrationBuilder.CreateIndex(
                name: "ix_time_entry_employee_id_project_id_date",
                table: "time_entry",
                columns: new[] { "employee_id", "project_id", "date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_time_entry_project_id_date",
                table: "time_entry",
                columns: new[] { "project_id", "date" });

            migrationBuilder.CreateIndex(
                name: "ix_time_entry_recorded_by",
                table: "time_entry",
                column: "recorded_by");

            migrationBuilder.CreateIndex(
                name: "ix_training_type_code",
                table: "training_type",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_active",
                table: "user",
                column: "active");

            migrationBuilder.CreateIndex(
                name: "ix_user_email",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_employee_id",
                table: "user",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_role_id",
                table: "user",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_username",
                table: "user",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_project_project_id",
                table: "user_project",
                column: "project_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointment");

            migrationBuilder.DropTable(
                name: "audit_log");

            migrationBuilder.DropTable(
                name: "employee_team");

            migrationBuilder.DropTable(
                name: "employee_training");

            migrationBuilder.DropTable(
                name: "payment");

            migrationBuilder.DropTable(
                name: "payroll_charge_rate");

            migrationBuilder.DropTable(
                name: "ppe_issue_item");

            migrationBuilder.DropTable(
                name: "project_amendment");

            migrationBuilder.DropTable(
                name: "project_link");

            migrationBuilder.DropTable(
                name: "purchase_request_item");

            migrationBuilder.DropTable(
                name: "role_permission");

            migrationBuilder.DropTable(
                name: "stock_movement");

            migrationBuilder.DropTable(
                name: "subcontract_measurement");

            migrationBuilder.DropTable(
                name: "team_project");

            migrationBuilder.DropTable(
                name: "time_entry");

            migrationBuilder.DropTable(
                name: "user_project");

            migrationBuilder.DropTable(
                name: "training_type");

            migrationBuilder.DropTable(
                name: "account_payable");

            migrationBuilder.DropTable(
                name: "account_receivable");

            migrationBuilder.DropTable(
                name: "ppe_issue");

            migrationBuilder.DropTable(
                name: "purchase_request");

            migrationBuilder.DropTable(
                name: "permission");

            migrationBuilder.DropTable(
                name: "stock_item");

            migrationBuilder.DropTable(
                name: "subcontract");

            migrationBuilder.DropTable(
                name: "stock_group");

            migrationBuilder.DropTable(
                name: "stage");

            migrationBuilder.DropTable(
                name: "supplier");

            migrationBuilder.DropTable(
                name: "stock_category");

            migrationBuilder.DropTable(
                name: "project");

            migrationBuilder.DropTable(
                name: "team");

            migrationBuilder.DropTable(
                name: "client");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "employee");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "job_role");

            migrationBuilder.DropTable(
                name: "employment_regime");

            migrationBuilder.DropTable(
                name: "department");
        }
    }
}
