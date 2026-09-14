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
                name: "categorias_estoque",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    controla_saldo_obra = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    exige_devolucao = table.Column<bool>(type: "tinyint(1)", nullable: false),
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
                name: "clientes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tipo_pessoa = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, defaultValueSql: "'PJ'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    documento_cifrado = table.Column<byte[]>(type: "varbinary(512)", maxLength: 512, nullable: true),
                    documento_hash = table.Column<string>(type: "char(64)", fixedLength: true, maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    documento_mascara = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contato = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    celular = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    endereco = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numero = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    complemento = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    bairro = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cidade = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    estado = table.Column<string>(type: "char(2)", fixedLength: true, maxLength: 2, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cep = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observacoes = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    ativo_key = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true, computedColumnSql: "ifnull(`deleted_at`,'1970-01-01 00:00:00')", stored: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "fornecedores",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    documento_cifrado = table.Column<byte[]>(type: "varbinary(512)", maxLength: 512, nullable: true),
                    documento_hash = table.Column<string>(type: "char(64)", fixedLength: true, maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    documento_mascara = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contato = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cidade = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    estado = table.Column<string>(type: "char(2)", fixedLength: true, maxLength: 2, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observacoes = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    ativo_key = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true, computedColumnSql: "ifnull(`deleted_at`,'1970-01-01 00:00:00')", stored: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "papeis",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
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
                name: "permissoes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    chave = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "regimes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    rotulo = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    unidade = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    horas_mes = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    descricao = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
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
                name: "setores",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
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
                name: "tipos_capacitacao",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    codigo = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nome = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    validade_meses = table.Column<int>(type: "int", nullable: true),
                    carga_horaria_min = table.Column<int>(type: "int", nullable: true),
                    exige_presencial = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    observacao = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "grupos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    categoria_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_grupos_categoria",
                        column: x => x.categoria_id,
                        principalTable: "categorias_estoque",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "papel_permissoes",
                columns: table => new
                {
                    papel_id = table.Column<long>(type: "bigint", nullable: false),
                    permissao_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.papel_id, x.permissao_id })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "fk_papel_permissoes_papel",
                        column: x => x.papel_id,
                        principalTable: "papeis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_papel_permissoes_permissao",
                        column: x => x.permissao_id,
                        principalTable: "permissoes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "parametros_encargos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    regime_id = table.Column<long>(type: "bigint", nullable: false),
                    percentual = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    vigencia_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    vigencia_fim = table.Column<DateOnly>(type: "date", nullable: true),
                    fonte = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observacao = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_param_encargos_regime",
                        column: x => x.regime_id,
                        principalTable: "regimes",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "equipes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    setor_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    ativo_key = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true, computedColumnSql: "ifnull(`deleted_at`,'1970-01-01 00:00:00')", stored: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_equipes_setor",
                        column: x => x.setor_id,
                        principalTable: "setores",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "funcoes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    setor_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_funcoes_setor",
                        column: x => x.setor_id,
                        principalTable: "setores",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "itens_estoque",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    grupo_id = table.Column<long>(type: "bigint", nullable: false),
                    codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    item = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    unidade = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    minimo = table.Column<decimal>(type: "decimal(15,3)", precision: 15, scale: 3, nullable: false),
                    preco_ref = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    ca = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ca_validade = table.Column<DateOnly>(type: "date", nullable: true),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    ativo_key = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true, computedColumnSql: "ifnull(`deleted_at`,'1970-01-01 00:00:00')", stored: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_itens_estoque_grupo",
                        column: x => x.grupo_id,
                        principalTable: "grupos",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "funcionarios",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    matricula = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nome = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    funcao_id = table.Column<long>(type: "bigint", nullable: true),
                    regime_id = table.Column<long>(type: "bigint", nullable: true),
                    valor = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    adicional_percentual = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    data_admissao = table.Column<DateOnly>(type: "date", nullable: true),
                    data_demissao = table.Column<DateOnly>(type: "date", nullable: true),
                    telefone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    documento_cifrado = table.Column<byte[]>(type: "varbinary(512)", maxLength: 512, nullable: true),
                    documento_hash = table.Column<string>(type: "char(64)", fixedLength: true, maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    documento_mascara = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'"),
                    anonimizado_em = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    ativo_key = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true, computedColumnSql: "ifnull(`deleted_at`,'1970-01-01 00:00:00')", stored: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_funcionarios_funcao",
                        column: x => x.funcao_id,
                        principalTable: "funcoes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_funcionarios_regime",
                        column: x => x.regime_id,
                        principalTable: "regimes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "funcionario_capacitacoes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    funcionario_id = table.Column<long>(type: "bigint", nullable: false),
                    tipo_id = table.Column<long>(type: "bigint", nullable: false),
                    data_emissao = table.Column<DateOnly>(type: "date", nullable: false),
                    data_validade = table.Column<DateOnly>(type: "date", nullable: true),
                    carga_horaria = table.Column<int>(type: "int", nullable: true),
                    modalidade = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    instrutor = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numero_certificado = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    anexo_url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observacao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_capacitacoes_funcionario",
                        column: x => x.funcionario_id,
                        principalTable: "funcionarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_capacitacoes_tipo",
                        column: x => x.tipo_id,
                        principalTable: "tipos_capacitacao",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "funcionario_equipes",
                columns: table => new
                {
                    funcionario_id = table.Column<long>(type: "bigint", nullable: false),
                    equipe_id = table.Column<long>(type: "bigint", nullable: false),
                    data_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    data_fim = table.Column<DateOnly>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.funcionario_id, x.equipe_id, x.data_inicio })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });
                    table.ForeignKey(
                        name: "fk_funcionario_equipes_equipe",
                        column: x => x.equipe_id,
                        principalTable: "equipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_funcionario_equipes_funcionario",
                        column: x => x.funcionario_id,
                        principalTable: "funcionarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    usuario = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    senha_hash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nome = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    papel_id = table.Column<long>(type: "bigint", nullable: false),
                    funcionario_id = table.Column<long>(type: "bigint", nullable: true),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'"),
                    ultimo_login = table.Column<DateTime>(type: "datetime", nullable: true),
                    tentativas_falhas = table.Column<int>(type: "int", nullable: false),
                    bloqueado_ate = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_usuarios_funcionario",
                        column: x => x.funcionario_id,
                        principalTable: "funcionarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_usuarios_papel",
                        column: x => x.papel_id,
                        principalTable: "papeis",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "auditoria",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    tabela = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    registro_id = table.Column<long>(type: "bigint", nullable: true),
                    acao = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    campos_alterados = table.Column<string>(type: "json", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dados_anteriores = table.Column<string>(type: "json", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dados_novos = table.Column<string>(type: "json", nullable: true, collation: "utf8mb4_0900_ai_ci")
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
                        name: "fk_auditoria_usuario",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "obras",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cliente_id = table.Column<long>(type: "bigint", nullable: true),
                    nome = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cno = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    endereco = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numero = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    complemento = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    bairro = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cidade = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    estado = table.Column<string>(type: "char(2)", fixedLength: true, maxLength: 2, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cep = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contrato_valor = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'planejamento'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    responsavel_id = table.Column<long>(type: "bigint", nullable: true),
                    responsavel_tecnico = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    crea_rt = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_inicio = table.Column<DateOnly>(type: "date", nullable: true),
                    data_previsao = table.Column<DateOnly>(type: "date", nullable: true),
                    data_conclusao = table.Column<DateOnly>(type: "date", nullable: true),
                    fechada_em = table.Column<DateTime>(type: "datetime", nullable: true),
                    fechada_por = table.Column<long>(type: "bigint", nullable: true),
                    observacoes = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true),
                    ativo_key = table.Column<DateTime>(type: "datetime(6)", maxLength: 6, nullable: true, computedColumnSql: "ifnull(`deleted_at`,'1970-01-01 00:00:00')", stored: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_obras_cliente",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_obras_fechada_por",
                        column: x => x.fechada_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_obras_responsavel",
                        column: x => x.responsavel_id,
                        principalTable: "funcionarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "agenda",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    data = table.Column<DateOnly>(type: "date", nullable: false),
                    hora = table.Column<TimeOnly>(type: "time", nullable: true),
                    titulo = table.Column<string>(type: "varchar(220)", maxLength: 220, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    obra_id = table.Column<long>(type: "bigint", nullable: true),
                    responsavel_id = table.Column<long>(type: "bigint", nullable: true),
                    tipo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'obra'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    concluido = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    concluido_em = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_agenda_obra",
                        column: x => x.obra_id,
                        principalTable: "obras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_agenda_responsavel",
                        column: x => x.responsavel_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "contas_pagar",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fornecedor_id = table.Column<long>(type: "bigint", nullable: true),
                    obra_id = table.Column<long>(type: "bigint", nullable: true),
                    descricao = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numero_nf = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    valor = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    valor_pago = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    vencimento = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'pendente'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    origem_tipo = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    origem_id = table.Column<long>(type: "bigint", nullable: true),
                    observacao = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_contas_pagar_fornecedor",
                        column: x => x.fornecedor_id,
                        principalTable: "fornecedores",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_contas_pagar_obra",
                        column: x => x.obra_id,
                        principalTable: "obras",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "contas_receber",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cliente_id = table.Column<long>(type: "bigint", nullable: true),
                    obra_id = table.Column<long>(type: "bigint", nullable: true),
                    descricao = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numero_nf = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    valor = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    valor_pago = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    vencimento = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'pendente'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observacao = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_contas_receber_cliente",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_contas_receber_obra",
                        column: x => x.obra_id,
                        principalTable: "obras",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "equipe_obras",
                columns: table => new
                {
                    equipe_id = table.Column<long>(type: "bigint", nullable: false),
                    obra_id = table.Column<long>(type: "bigint", nullable: false),
                    data_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    data_fim = table.Column<DateOnly>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.equipe_id, x.obra_id, x.data_inicio })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });
                    table.ForeignKey(
                        name: "fk_equipe_obras_equipe",
                        column: x => x.equipe_id,
                        principalTable: "equipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_equipe_obras_obra",
                        column: x => x.obra_id,
                        principalTable: "obras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "estoque_movimentacoes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    item_id = table.Column<long>(type: "bigint", nullable: false),
                    obra_id = table.Column<long>(type: "bigint", nullable: true),
                    funcionario_id = table.Column<long>(type: "bigint", nullable: true),
                    tipo = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    quantidade = table.Column<decimal>(type: "decimal(15,3)", precision: 15, scale: 3, nullable: false),
                    unidade = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    custo_unitario = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    fornecedor_id = table.Column<long>(type: "bigint", nullable: true),
                    numero_nf = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    origem_tipo = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    origem_id = table.Column<long>(type: "bigint", nullable: true),
                    transferencia_id = table.Column<long>(type: "bigint", nullable: true),
                    observacao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    registrado_por = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_estoque_mov_fornecedor",
                        column: x => x.fornecedor_id,
                        principalTable: "fornecedores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_estoque_mov_funcionario",
                        column: x => x.funcionario_id,
                        principalTable: "funcionarios",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_estoque_mov_item",
                        column: x => x.item_id,
                        principalTable: "itens_estoque",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_estoque_mov_obra",
                        column: x => x.obra_id,
                        principalTable: "obras",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_estoque_mov_registrador",
                        column: x => x.registrado_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "etapas",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    obra_id = table.Column<long>(type: "bigint", nullable: false),
                    nome = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ordem = table.Column<int>(type: "int", nullable: false),
                    equipe_id = table.Column<long>(type: "bigint", nullable: true),
                    responsavel_id = table.Column<long>(type: "bigint", nullable: true),
                    data_prevista = table.Column<DateOnly>(type: "date", nullable: true),
                    data_inicio = table.Column<DateOnly>(type: "date", nullable: true),
                    data_conclusao = table.Column<DateOnly>(type: "date", nullable: true),
                    percentual = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'prevista'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observacao = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    marcado_por = table.Column<long>(type: "bigint", nullable: true),
                    marcado_em = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_etapas_equipe",
                        column: x => x.equipe_id,
                        principalTable: "equipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_etapas_marcado_por",
                        column: x => x.marcado_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_etapas_obra",
                        column: x => x.obra_id,
                        principalTable: "obras",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_etapas_responsavel",
                        column: x => x.responsavel_id,
                        principalTable: "funcionarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "fichas",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    funcionario_id = table.Column<long>(type: "bigint", nullable: false),
                    obra_id = table.Column<long>(type: "bigint", nullable: true),
                    data = table.Column<DateOnly>(type: "date", nullable: false),
                    observacao = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    responsavel_id = table.Column<long>(type: "bigint", nullable: true),
                    assinado_em = table.Column<DateTime>(type: "datetime", nullable: true),
                    assinatura_url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    assinatura_hash = table.Column<string>(type: "char(64)", fixedLength: true, maxLength: 64, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_fichas_funcionario",
                        column: x => x.funcionario_id,
                        principalTable: "funcionarios",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_fichas_obra",
                        column: x => x.obra_id,
                        principalTable: "obras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_fichas_responsavel",
                        column: x => x.responsavel_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "obra_aditivos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    obra_id = table.Column<long>(type: "bigint", nullable: false),
                    tipo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numero = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data = table.Column<DateOnly>(type: "date", nullable: false),
                    valor = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    dias_prazo = table.Column<int>(type: "int", nullable: false),
                    motivo = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    aprovado_por = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_obra_aditivos_aprovador",
                        column: x => x.aprovado_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_obra_aditivos_obra",
                        column: x => x.obra_id,
                        principalTable: "obras",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "obra_links",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    obra_id = table.Column<long>(type: "bigint", nullable: false),
                    token_hash = table.Column<string>(type: "char(64)", fixedLength: true, maxLength: 64, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    rotulo = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expira_em = table.Column<DateTime>(type: "datetime", nullable: true),
                    revogado_em = table.Column<DateTime>(type: "datetime", nullable: true),
                    ultimo_acesso = table.Column<DateTime>(type: "datetime", nullable: true),
                    total_acessos = table.Column<int>(type: "int", nullable: false),
                    criado_por = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_obra_links_criador",
                        column: x => x.criado_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_obra_links_obra",
                        column: x => x.obra_id,
                        principalTable: "obras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "ponto",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    funcionario_id = table.Column<long>(type: "bigint", nullable: false),
                    obra_id = table.Column<long>(type: "bigint", nullable: false),
                    data = table.Column<DateOnly>(type: "date", nullable: false),
                    hora_entrada = table.Column<TimeOnly>(type: "time", nullable: true),
                    hora_saida_intervalo = table.Column<TimeOnly>(type: "time", nullable: true),
                    hora_volta_intervalo = table.Column<TimeOnly>(type: "time", nullable: true),
                    hora_saida = table.Column<TimeOnly>(type: "time", nullable: true),
                    horas = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true, computedColumnSql: "round((greatest((ifnull(time_to_sec(timediff(`hora_saida`,`hora_entrada`)),0) - ifnull(time_to_sec(timediff(`hora_volta_intervalo`,`hora_saida_intervalo`)),0)),0) / 3600),2)", stored: true),
                    valor_hora_snapshot = table.Column<decimal>(type: "decimal(15,4)", precision: 15, scale: 4, nullable: false),
                    adicional_snapshot = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    encargos_snapshot = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    custo = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    origem = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'manual'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_externo = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    registrado_por = table.Column<long>(type: "bigint", nullable: true),
                    observacao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_ponto_funcionario",
                        column: x => x.funcionario_id,
                        principalTable: "funcionarios",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ponto_obra",
                        column: x => x.obra_id,
                        principalTable: "obras",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ponto_registrador",
                        column: x => x.registrado_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "solicitacoes_compra",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    obra_id = table.Column<long>(type: "bigint", nullable: false),
                    solicitado_por = table.Column<long>(type: "bigint", nullable: true),
                    data = table.Column<DateOnly>(type: "date", nullable: false),
                    data_necessidade = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'pendente'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observacao = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    decidido_por = table.Column<long>(type: "bigint", nullable: true),
                    decidido_em = table.Column<DateTime>(type: "datetime", nullable: true),
                    motivo_recusa = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_solicitacoes_decisor",
                        column: x => x.decidido_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_solicitacoes_obra",
                        column: x => x.obra_id,
                        principalTable: "obras",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_solicitacoes_solicitante",
                        column: x => x.solicitado_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "usuario_obras",
                columns: table => new
                {
                    usuario_id = table.Column<long>(type: "bigint", nullable: false),
                    obra_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.usuario_id, x.obra_id })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "fk_usuario_obras_obra",
                        column: x => x.obra_id,
                        principalTable: "obras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_usuario_obras_usuario",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "pagamentos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    conta_receber_id = table.Column<long>(type: "bigint", nullable: true),
                    conta_pagar_id = table.Column<long>(type: "bigint", nullable: true),
                    valor = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    data_pagamento = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    forma_pagamento = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    registrado_por = table.Column<long>(type: "bigint", nullable: true),
                    observacao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_pagamentos_pagar",
                        column: x => x.conta_pagar_id,
                        principalTable: "contas_pagar",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_pagamentos_receber",
                        column: x => x.conta_receber_id,
                        principalTable: "contas_receber",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_pagamentos_registrador",
                        column: x => x.registrado_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "contratos_empreitada",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    obra_id = table.Column<long>(type: "bigint", nullable: false),
                    fornecedor_id = table.Column<long>(type: "bigint", nullable: true),
                    etapa_id = table.Column<long>(type: "bigint", nullable: true),
                    descricao = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tipo_preco = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'global'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    valor_total = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    preco_unitario = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: true),
                    unidade = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    quantidade_prevista = table.Column<decimal>(type: "decimal(15,3)", precision: 15, scale: 3, nullable: true),
                    retencao_inss_pct = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    data_inicio = table.Column<DateOnly>(type: "date", nullable: true),
                    data_fim = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'ativo'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observacao = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_empreitada_etapa",
                        column: x => x.etapa_id,
                        principalTable: "etapas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_empreitada_fornecedor",
                        column: x => x.fornecedor_id,
                        principalTable: "fornecedores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_empreitada_obra",
                        column: x => x.obra_id,
                        principalTable: "obras",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "ficha_itens",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ficha_id = table.Column<long>(type: "bigint", nullable: false),
                    item_id = table.Column<long>(type: "bigint", nullable: true),
                    item_snapshot = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ca_snapshot = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    quantidade = table.Column<decimal>(type: "decimal(15,3)", precision: 15, scale: 3, nullable: false, defaultValueSql: "'1.000'"),
                    unidade = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValueSql: "'entregue'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_entrega = table.Column<DateOnly>(type: "date", nullable: true),
                    data_devolucao = table.Column<DateOnly>(type: "date", nullable: true),
                    observacao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_ficha_itens_ficha",
                        column: x => x.ficha_id,
                        principalTable: "fichas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ficha_itens_item",
                        column: x => x.item_id,
                        principalTable: "itens_estoque",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "solicitacao_itens",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    solicitacao_id = table.Column<long>(type: "bigint", nullable: false),
                    item_id = table.Column<long>(type: "bigint", nullable: false),
                    qtd_solicitada = table.Column<decimal>(type: "decimal(15,3)", precision: 15, scale: 3, nullable: false),
                    qtd_atendida = table.Column<decimal>(type: "decimal(15,3)", precision: 15, scale: 3, nullable: false),
                    unidade = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observacao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_solicitacao_itens_item",
                        column: x => x.item_id,
                        principalTable: "itens_estoque",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_solicitacao_itens_solicitacao",
                        column: x => x.solicitacao_id,
                        principalTable: "solicitacoes_compra",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "medicoes_empreitada",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    contrato_id = table.Column<long>(type: "bigint", nullable: false),
                    numero = table.Column<int>(type: "int", nullable: false),
                    data = table.Column<DateOnly>(type: "date", nullable: false),
                    quantidade = table.Column<decimal>(type: "decimal(15,3)", precision: 15, scale: 3, nullable: true),
                    percentual = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    valor = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: false),
                    aprovado_por = table.Column<long>(type: "bigint", nullable: true),
                    aprovado_em = table.Column<DateTime>(type: "datetime", nullable: true),
                    observacao = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_medicoes_aprovador",
                        column: x => x.aprovado_por,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_medicoes_contrato",
                        column: x => x.contrato_id,
                        principalTable: "contratos_empreitada",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "ix_agenda_data_hora",
                table: "agenda",
                columns: new[] { "data", "hora" });

            migrationBuilder.CreateIndex(
                name: "ix_agenda_obra_id",
                table: "agenda",
                column: "obra_id");

            migrationBuilder.CreateIndex(
                name: "ix_agenda_responsavel_id",
                table: "agenda",
                column: "responsavel_id");

            migrationBuilder.CreateIndex(
                name: "ix_auditoria_created_at",
                table: "auditoria",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_auditoria_tabela_registro_id",
                table: "auditoria",
                columns: new[] { "tabela", "registro_id" });

            migrationBuilder.CreateIndex(
                name: "ix_auditoria_usuario_id_created_at",
                table: "auditoria",
                columns: new[] { "usuario_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_categorias_estoque_nome",
                table: "categorias_estoque",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_clientes_cidade",
                table: "clientes",
                column: "cidade");

            migrationBuilder.CreateIndex(
                name: "ix_clientes_deleted_at",
                table: "clientes",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "ix_clientes_documento_hash_ativo_key",
                table: "clientes",
                columns: new[] { "documento_hash", "ativo_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_clientes_nome",
                table: "clientes",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "ix_contas_pagar_fornecedor_id",
                table: "contas_pagar",
                column: "fornecedor_id");

            migrationBuilder.CreateIndex(
                name: "ix_contas_pagar_obra_id",
                table: "contas_pagar",
                column: "obra_id");

            migrationBuilder.CreateIndex(
                name: "ix_contas_pagar_origem_tipo_origem_id",
                table: "contas_pagar",
                columns: new[] { "origem_tipo", "origem_id" });

            migrationBuilder.CreateIndex(
                name: "ix_contas_pagar_vencimento_status",
                table: "contas_pagar",
                columns: new[] { "vencimento", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_contas_receber_cliente_id",
                table: "contas_receber",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "ix_contas_receber_obra_id",
                table: "contas_receber",
                column: "obra_id");

            migrationBuilder.CreateIndex(
                name: "ix_contas_receber_vencimento_status",
                table: "contas_receber",
                columns: new[] { "vencimento", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_contratos_empreitada_etapa_id",
                table: "contratos_empreitada",
                column: "etapa_id");

            migrationBuilder.CreateIndex(
                name: "ix_contratos_empreitada_fornecedor_id",
                table: "contratos_empreitada",
                column: "fornecedor_id");

            migrationBuilder.CreateIndex(
                name: "ix_contratos_empreitada_obra_id",
                table: "contratos_empreitada",
                column: "obra_id");

            migrationBuilder.CreateIndex(
                name: "ix_equipe_obras_obra_id",
                table: "equipe_obras",
                column: "obra_id");

            migrationBuilder.CreateIndex(
                name: "ix_equipes_nome_setor_id_ativo_key",
                table: "equipes",
                columns: new[] { "nome", "setor_id", "ativo_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_equipes_setor_id",
                table: "equipes",
                column: "setor_id");

            migrationBuilder.CreateIndex(
                name: "ix_estoque_movimentacoes_fornecedor_id",
                table: "estoque_movimentacoes",
                column: "fornecedor_id");

            migrationBuilder.CreateIndex(
                name: "ix_estoque_movimentacoes_funcionario_id_data",
                table: "estoque_movimentacoes",
                columns: new[] { "funcionario_id", "data" });

            migrationBuilder.CreateIndex(
                name: "ix_estoque_movimentacoes_item_id_data",
                table: "estoque_movimentacoes",
                columns: new[] { "item_id", "data" });

            migrationBuilder.CreateIndex(
                name: "ix_estoque_movimentacoes_item_id_obra_id",
                table: "estoque_movimentacoes",
                columns: new[] { "item_id", "obra_id" });

            migrationBuilder.CreateIndex(
                name: "ix_estoque_movimentacoes_obra_id_data",
                table: "estoque_movimentacoes",
                columns: new[] { "obra_id", "data" });

            migrationBuilder.CreateIndex(
                name: "ix_estoque_movimentacoes_origem_tipo_origem_id",
                table: "estoque_movimentacoes",
                columns: new[] { "origem_tipo", "origem_id" });

            migrationBuilder.CreateIndex(
                name: "ix_estoque_movimentacoes_registrado_por",
                table: "estoque_movimentacoes",
                column: "registrado_por");

            migrationBuilder.CreateIndex(
                name: "ix_estoque_movimentacoes_transferencia_id",
                table: "estoque_movimentacoes",
                column: "transferencia_id");

            migrationBuilder.CreateIndex(
                name: "ix_etapas_data_prevista",
                table: "etapas",
                column: "data_prevista");

            migrationBuilder.CreateIndex(
                name: "ix_etapas_equipe_id",
                table: "etapas",
                column: "equipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_etapas_marcado_por",
                table: "etapas",
                column: "marcado_por");

            migrationBuilder.CreateIndex(
                name: "ix_etapas_obra_id",
                table: "etapas",
                column: "obra_id");

            migrationBuilder.CreateIndex(
                name: "ix_etapas_obra_id_status",
                table: "etapas",
                columns: new[] { "obra_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_etapas_responsavel_id",
                table: "etapas",
                column: "responsavel_id");

            migrationBuilder.CreateIndex(
                name: "ix_ficha_itens_ficha_id",
                table: "ficha_itens",
                column: "ficha_id");

            migrationBuilder.CreateIndex(
                name: "ix_ficha_itens_item_id",
                table: "ficha_itens",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "ix_ficha_itens_status",
                table: "ficha_itens",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_fichas_funcionario_id_data",
                table: "fichas",
                columns: new[] { "funcionario_id", "data" });

            migrationBuilder.CreateIndex(
                name: "ix_fichas_obra_id",
                table: "fichas",
                column: "obra_id");

            migrationBuilder.CreateIndex(
                name: "ix_fichas_responsavel_id",
                table: "fichas",
                column: "responsavel_id");

            migrationBuilder.CreateIndex(
                name: "ix_fornecedores_documento_hash_ativo_key",
                table: "fornecedores",
                columns: new[] { "documento_hash", "ativo_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_fornecedores_nome",
                table: "fornecedores",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "ix_funcionario_capacitacoes_data_validade",
                table: "funcionario_capacitacoes",
                column: "data_validade");

            migrationBuilder.CreateIndex(
                name: "ix_funcionario_capacitacoes_funcionario_id",
                table: "funcionario_capacitacoes",
                column: "funcionario_id");

            migrationBuilder.CreateIndex(
                name: "ix_funcionario_capacitacoes_tipo_id",
                table: "funcionario_capacitacoes",
                column: "tipo_id");

            migrationBuilder.CreateIndex(
                name: "ix_funcionario_equipes_equipe_id",
                table: "funcionario_equipes",
                column: "equipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_funcionarios_ativo",
                table: "funcionarios",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "ix_funcionarios_deleted_at",
                table: "funcionarios",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "ix_funcionarios_documento_hash_ativo_key",
                table: "funcionarios",
                columns: new[] { "documento_hash", "ativo_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_funcionarios_funcao_id",
                table: "funcionarios",
                column: "funcao_id");

            migrationBuilder.CreateIndex(
                name: "ix_funcionarios_matricula_ativo_key",
                table: "funcionarios",
                columns: new[] { "matricula", "ativo_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_funcionarios_regime_id",
                table: "funcionarios",
                column: "regime_id");

            migrationBuilder.CreateIndex(
                name: "ix_funcoes_nome_setor_id",
                table: "funcoes",
                columns: new[] { "nome", "setor_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_funcoes_setor_id",
                table: "funcoes",
                column: "setor_id");

            migrationBuilder.CreateIndex(
                name: "ix_grupos_categoria_id",
                table: "grupos",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "ix_grupos_nome_categoria_id",
                table: "grupos",
                columns: new[] { "nome", "categoria_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_itens_estoque_ativo",
                table: "itens_estoque",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "ix_itens_estoque_codigo_ativo_key",
                table: "itens_estoque",
                columns: new[] { "codigo", "ativo_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_itens_estoque_deleted_at",
                table: "itens_estoque",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "ix_itens_estoque_grupo_id",
                table: "itens_estoque",
                column: "grupo_id");

            migrationBuilder.CreateIndex(
                name: "ix_itens_estoque_item",
                table: "itens_estoque",
                column: "item");

            migrationBuilder.CreateIndex(
                name: "ix_medicoes_empreitada_aprovado_por",
                table: "medicoes_empreitada",
                column: "aprovado_por");

            migrationBuilder.CreateIndex(
                name: "ix_medicoes_empreitada_contrato_id_numero",
                table: "medicoes_empreitada",
                columns: new[] { "contrato_id", "numero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_medicoes_empreitada_data",
                table: "medicoes_empreitada",
                column: "data");

            migrationBuilder.CreateIndex(
                name: "ix_obra_aditivos_aprovado_por",
                table: "obra_aditivos",
                column: "aprovado_por");

            migrationBuilder.CreateIndex(
                name: "ix_obra_aditivos_obra_id",
                table: "obra_aditivos",
                column: "obra_id");

            migrationBuilder.CreateIndex(
                name: "ix_obra_links_criado_por",
                table: "obra_links",
                column: "criado_por");

            migrationBuilder.CreateIndex(
                name: "ix_obra_links_obra_id",
                table: "obra_links",
                column: "obra_id");

            migrationBuilder.CreateIndex(
                name: "ix_obra_links_token_hash",
                table: "obra_links",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_obras_cliente_id",
                table: "obras",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "ix_obras_cliente_id_status",
                table: "obras",
                columns: new[] { "cliente_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_obras_codigo_ativo_key",
                table: "obras",
                columns: new[] { "codigo", "ativo_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_obras_data_previsao",
                table: "obras",
                column: "data_previsao");

            migrationBuilder.CreateIndex(
                name: "ix_obras_deleted_at",
                table: "obras",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "ix_obras_fechada_por",
                table: "obras",
                column: "fechada_por");

            migrationBuilder.CreateIndex(
                name: "ix_obras_responsavel_id",
                table: "obras",
                column: "responsavel_id");

            migrationBuilder.CreateIndex(
                name: "ix_obras_status",
                table: "obras",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_pagamentos_conta_pagar_id",
                table: "pagamentos",
                column: "conta_pagar_id");

            migrationBuilder.CreateIndex(
                name: "ix_pagamentos_conta_receber_id",
                table: "pagamentos",
                column: "conta_receber_id");

            migrationBuilder.CreateIndex(
                name: "ix_pagamentos_data_pagamento",
                table: "pagamentos",
                column: "data_pagamento");

            migrationBuilder.CreateIndex(
                name: "ix_pagamentos_registrado_por",
                table: "pagamentos",
                column: "registrado_por");

            migrationBuilder.CreateIndex(
                name: "ix_papeis_nome",
                table: "papeis",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_papel_permissoes_permissao_id",
                table: "papel_permissoes",
                column: "permissao_id");

            migrationBuilder.CreateIndex(
                name: "ix_parametros_encargos_regime_id_vigencia_inicio",
                table: "parametros_encargos",
                columns: new[] { "regime_id", "vigencia_inicio" });

            migrationBuilder.CreateIndex(
                name: "ix_permissoes_chave",
                table: "permissoes",
                column: "chave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ponto_funcionario_id_data",
                table: "ponto",
                columns: new[] { "funcionario_id", "data" });

            migrationBuilder.CreateIndex(
                name: "ix_ponto_funcionario_id_obra_id_data",
                table: "ponto",
                columns: new[] { "funcionario_id", "obra_id", "data" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ponto_obra_id_data",
                table: "ponto",
                columns: new[] { "obra_id", "data" });

            migrationBuilder.CreateIndex(
                name: "ix_ponto_registrado_por",
                table: "ponto",
                column: "registrado_por");

            migrationBuilder.CreateIndex(
                name: "ix_regimes_rotulo",
                table: "regimes",
                column: "rotulo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_setores_nome",
                table: "setores",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_solicitacao_itens_item_id",
                table: "solicitacao_itens",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "ix_solicitacao_itens_solicitacao_id",
                table: "solicitacao_itens",
                column: "solicitacao_id");

            migrationBuilder.CreateIndex(
                name: "ix_solicitacoes_compra_data",
                table: "solicitacoes_compra",
                column: "data");

            migrationBuilder.CreateIndex(
                name: "ix_solicitacoes_compra_decidido_por",
                table: "solicitacoes_compra",
                column: "decidido_por");

            migrationBuilder.CreateIndex(
                name: "ix_solicitacoes_compra_obra_id_status",
                table: "solicitacoes_compra",
                columns: new[] { "obra_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_solicitacoes_compra_solicitado_por",
                table: "solicitacoes_compra",
                column: "solicitado_por");

            migrationBuilder.CreateIndex(
                name: "ix_tipos_capacitacao_codigo",
                table: "tipos_capacitacao",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_usuario_obras_obra_id",
                table: "usuario_obras",
                column: "obra_id");

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_ativo",
                table: "usuarios",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_funcionario_id",
                table: "usuarios",
                column: "funcionario_id");

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_papel_id",
                table: "usuarios",
                column: "papel_id");

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_usuario",
                table: "usuarios",
                column: "usuario",
                unique: true);

            // ---------- CHECK constraints (40) ----------
            migrationBuilder.Sql(@"ALTER TABLE parametros_encargos ADD CONSTRAINT chk_param_encargos_percentual CHECK (percentual >= 0);");
            migrationBuilder.Sql(@"ALTER TABLE parametros_encargos ADD CONSTRAINT chk_param_encargos_vigencia CHECK (vigencia_fim IS NULL OR vigencia_fim >= vigencia_inicio);");
            migrationBuilder.Sql(@"ALTER TABLE clientes ADD CONSTRAINT chk_clientes_tipo CHECK (tipo_pessoa IN ('PF','PJ'));");
            migrationBuilder.Sql(@"ALTER TABLE funcionarios ADD CONSTRAINT chk_funcionarios_valor CHECK (valor >= 0);");
            migrationBuilder.Sql(@"ALTER TABLE funcionarios ADD CONSTRAINT chk_funcionarios_adicional CHECK (adicional_percentual >= 0);");
            migrationBuilder.Sql(@"ALTER TABLE funcionarios ADD CONSTRAINT chk_funcionarios_datas CHECK (data_demissao IS NULL OR data_admissao IS NULL OR data_demissao >= data_admissao);");
            migrationBuilder.Sql(@"ALTER TABLE funcionario_capacitacoes ADD CONSTRAINT chk_capacitacoes_datas CHECK (data_validade IS NULL OR data_validade >= data_emissao);");
            migrationBuilder.Sql(@"ALTER TABLE obras ADD CONSTRAINT chk_obras_status CHECK (status IN ('planejamento','andamento','paralisada','concluida','cancelada'));");
            migrationBuilder.Sql(@"ALTER TABLE obras ADD CONSTRAINT chk_obras_contrato CHECK (contrato_valor >= 0);");
            migrationBuilder.Sql(@"ALTER TABLE obras ADD CONSTRAINT chk_obras_datas CHECK (data_conclusao IS NULL OR data_inicio IS NULL OR data_conclusao >= data_inicio);");
            migrationBuilder.Sql(@"ALTER TABLE obra_aditivos ADD CONSTRAINT chk_obra_aditivos_tipo CHECK (tipo IN ('valor','prazo','valor_prazo'));");
            migrationBuilder.Sql(@"ALTER TABLE equipe_obras ADD CONSTRAINT chk_equipe_obras_datas CHECK (data_fim IS NULL OR data_fim >= data_inicio);");
            migrationBuilder.Sql(@"ALTER TABLE funcionario_equipes ADD CONSTRAINT chk_funcionario_equipes_datas CHECK (data_fim IS NULL OR data_fim >= data_inicio);");
            migrationBuilder.Sql(@"ALTER TABLE etapas ADD CONSTRAINT chk_etapas_status CHECK (status IN ('prevista','andamento','concluida','cancelada'));");
            migrationBuilder.Sql(@"ALTER TABLE etapas ADD CONSTRAINT chk_etapas_percentual CHECK (percentual >= 0 AND percentual <= 100);");
            migrationBuilder.Sql(@"ALTER TABLE etapas ADD CONSTRAINT chk_etapas_datas CHECK (data_conclusao IS NULL OR data_inicio IS NULL OR data_conclusao >= data_inicio);");
            migrationBuilder.Sql(@"ALTER TABLE itens_estoque ADD CONSTRAINT chk_itens_estoque_minimo CHECK (minimo >= 0);");
            migrationBuilder.Sql(@"ALTER TABLE itens_estoque ADD CONSTRAINT chk_itens_estoque_preco CHECK (preco_ref >= 0);");
            migrationBuilder.Sql(@"ALTER TABLE solicitacoes_compra ADD CONSTRAINT chk_solicitacoes_status CHECK (status IN ('pendente','aprovada','recusada','atendida','cancelada'));");
            migrationBuilder.Sql(@"ALTER TABLE solicitacao_itens ADD CONSTRAINT chk_solicitacao_itens_qtd CHECK (qtd_solicitada > 0 AND qtd_atendida >= 0);");
            migrationBuilder.Sql(@"ALTER TABLE estoque_movimentacoes ADD CONSTRAINT chk_estoque_mov_tipo CHECK (tipo IN ( 'entrada','saida','devolucao', 'ajuste_entrada','ajuste_saida', 'transferencia_entrada','transferencia_saida'));");
            migrationBuilder.Sql(@"ALTER TABLE estoque_movimentacoes ADD CONSTRAINT chk_estoque_mov_quantidade CHECK (quantidade > 0);");
            migrationBuilder.Sql(@"ALTER TABLE estoque_movimentacoes ADD CONSTRAINT chk_estoque_mov_custo CHECK (custo_unitario >= 0);");
            migrationBuilder.Sql(@"ALTER TABLE ficha_itens ADD CONSTRAINT chk_ficha_itens_status CHECK (status IN ('entregue','pendente','devolvido','perdido','danificado'));");
            migrationBuilder.Sql(@"ALTER TABLE ficha_itens ADD CONSTRAINT chk_ficha_itens_quantidade CHECK (quantidade > 0);");
            migrationBuilder.Sql(@"ALTER TABLE ficha_itens ADD CONSTRAINT chk_ficha_itens_datas CHECK (data_devolucao IS NULL OR data_entrega IS NULL OR data_devolucao >= data_entrega);");
            migrationBuilder.Sql(@"ALTER TABLE ponto ADD CONSTRAINT chk_ponto_custo CHECK (custo >= 0);");
            migrationBuilder.Sql(@"ALTER TABLE ponto ADD CONSTRAINT chk_ponto_origem CHECK (origem IN ('manual','dispositivo','importacao'));");
            migrationBuilder.Sql(@"ALTER TABLE contratos_empreitada ADD CONSTRAINT chk_empreitada_tipo CHECK (tipo_preco IN ('global','unitario'));");
            migrationBuilder.Sql(@"ALTER TABLE contratos_empreitada ADD CONSTRAINT chk_empreitada_status CHECK (status IN ('ativo','concluido','cancelado'));");
            migrationBuilder.Sql(@"ALTER TABLE contratos_empreitada ADD CONSTRAINT chk_empreitada_valores CHECK (valor_total >= 0);");
            migrationBuilder.Sql(@"ALTER TABLE medicoes_empreitada ADD CONSTRAINT chk_medicoes_valor CHECK (valor >= 0);");
            migrationBuilder.Sql(@"ALTER TABLE agenda ADD CONSTRAINT chk_agenda_tipo CHECK (tipo IN ('obra','entrega','medicao','reuniao','vistoria','outro'));");
            migrationBuilder.Sql(@"ALTER TABLE contas_receber ADD CONSTRAINT chk_contas_receber_valor CHECK (valor > 0 AND valor_pago >= 0);");
            migrationBuilder.Sql(@"ALTER TABLE contas_receber ADD CONSTRAINT chk_contas_receber_status CHECK (status IN ('pendente','parcial','pago','cancelado'));");
            migrationBuilder.Sql(@"ALTER TABLE contas_pagar ADD CONSTRAINT chk_contas_pagar_valor CHECK (valor > 0 AND valor_pago >= 0);");
            migrationBuilder.Sql(@"ALTER TABLE contas_pagar ADD CONSTRAINT chk_contas_pagar_status CHECK (status IN ('pendente','parcial','pago','cancelado'));");
            migrationBuilder.Sql(@"ALTER TABLE pagamentos ADD CONSTRAINT chk_pagamentos_valor CHECK (valor > 0);");
            migrationBuilder.Sql(@"ALTER TABLE pagamentos ADD CONSTRAINT chk_pagamentos_origem CHECK ((conta_receber_id IS NOT NULL AND conta_pagar_id IS NULL) OR (conta_receber_id IS NULL AND conta_pagar_id IS NOT NULL));");
            migrationBuilder.Sql(@"ALTER TABLE auditoria ADD CONSTRAINT chk_auditoria_acao CHECK (acao IN ('INSERT','UPDATE','DELETE','LOGIN','LOGOUT','LOGIN_FALHA','ACESSO_LINK'));");

            // ---------- Views (10) ----------
            migrationBuilder.Sql(@"
CREATE OR REPLACE VIEW vw_saldo_obra AS
SELECT
  m.obra_id,
  m.item_id,
  i.item,
  i.unidade,
  i.minimo,
  SUM(CASE
        WHEN m.tipo IN ('entrada','devolucao','ajuste_entrada','transferencia_entrada')
          THEN m.quantidade ELSE -m.quantidade
      END) AS saldo
FROM estoque_movimentacoes m
JOIN itens_estoque i ON i.id = m.item_id
WHERE m.obra_id IS NOT NULL
GROUP BY m.obra_id, m.item_id, i.item, i.unidade, i.minimo;
");

            migrationBuilder.Sql(@"
CREATE OR REPLACE VIEW vw_saldo_deposito AS
SELECT
  m.item_id,
  i.item,
  i.unidade,
  i.minimo,
  SUM(CASE
        WHEN m.tipo IN ('entrada','devolucao','ajuste_entrada','transferencia_entrada')
          THEN m.quantidade ELSE -m.quantidade
      END) AS saldo
FROM estoque_movimentacoes m
JOIN itens_estoque i ON i.id = m.item_id
WHERE m.obra_id IS NULL
GROUP BY m.item_id, i.item, i.unidade, i.minimo;
");

            migrationBuilder.Sql(@"
CREATE OR REPLACE VIEW vw_estoque_baixo AS
SELECT 'deposito' AS escopo, NULL AS obra_id, item_id, item, unidade, saldo, minimo
FROM vw_saldo_deposito WHERE saldo <= minimo
UNION ALL
SELECT 'obra' AS escopo, obra_id, item_id, item, unidade, saldo, minimo
FROM vw_saldo_obra WHERE saldo <= minimo;
");

            migrationBuilder.Sql(@"
CREATE OR REPLACE VIEW vw_obras_custos AS
SELECT
  o.id AS obra_id,
  o.codigo,
  o.nome,
  o.contrato_valor
    + COALESCE((SELECT SUM(a.valor) FROM obra_aditivos a WHERE a.obra_id = o.id), 0.00)
    AS contrato_atual,
  COALESCE((SELECT SUM(m.quantidade * m.custo_unitario)
            FROM estoque_movimentacoes m
            WHERE m.obra_id = o.id AND m.tipo = 'entrada'), 0.00) AS custo_material,
  COALESCE((SELECT SUM(p.custo) FROM ponto p WHERE p.obra_id = o.id), 0.00) AS custo_mao_obra,
  COALESCE((SELECT SUM(me.valor)
            FROM medicoes_empreitada me
            JOIN contratos_empreitada ce ON ce.id = me.contrato_id
            WHERE ce.obra_id = o.id), 0.00) AS custo_empreitada
FROM obras o
WHERE o.deleted_at IS NULL;
");

            migrationBuilder.Sql(@"
CREATE OR REPLACE VIEW vw_obras_resultado AS
SELECT
  c.*,
  (c.custo_material + c.custo_mao_obra + c.custo_empreitada) AS custo_total,
  (c.contrato_atual - c.custo_material - c.custo_mao_obra - c.custo_empreitada) AS resultado
FROM vw_obras_custos c;
");

            migrationBuilder.Sql(@"
CREATE OR REPLACE VIEW vw_obras_situacao AS
SELECT
  o.id AS obra_id,
  o.codigo,
  o.nome,
  o.status,
  o.data_previsao,
  o.fechada_em,
  CASE
    WHEN o.status IN ('concluida','cancelada') THEN o.status
    WHEN o.data_previsao IS NOT NULL AND o.data_previsao < CURDATE() THEN 'atrasada'
    ELSE o.status
  END AS situacao,
  (SELECT COUNT(*) FROM etapas e WHERE e.obra_id = o.id) AS total_etapas,
  (SELECT COUNT(*) FROM etapas e WHERE e.obra_id = o.id AND e.status = 'concluida') AS etapas_concluidas
FROM obras o
WHERE o.deleted_at IS NULL;
");

            migrationBuilder.Sql(@"
CREATE OR REPLACE VIEW vw_etapas_situacao AS
SELECT
  e.id AS etapa_id,
  e.obra_id,
  e.nome,
  e.status,
  e.percentual,
  e.data_prevista,
  CASE
    WHEN e.status IN ('concluida','cancelada') THEN e.status
    WHEN e.data_prevista IS NOT NULL AND e.data_prevista < CURDATE() THEN 'atrasada'
    ELSE e.status
  END AS situacao
FROM etapas e;
");

            migrationBuilder.Sql(@"
CREATE OR REPLACE VIEW vw_clientes_financeiro AS
SELECT
  c.id AS cliente_id,
  c.nome,
  COALESCE(SUM(cr.valor - cr.valor_pago), 0.00) AS total_a_receber
FROM clientes c
LEFT JOIN contas_receber cr
       ON cr.cliente_id = c.id
      AND cr.status IN ('pendente','parcial')
WHERE c.deleted_at IS NULL
GROUP BY c.id, c.nome;
");

            migrationBuilder.Sql(@"
CREATE OR REPLACE VIEW vw_obras_financeiro AS
SELECT
  o.id AS obra_id,
  o.codigo,
  o.nome,
  o.contrato_valor,
  COALESCE(SUM(cr.valor - cr.valor_pago), 0.00) AS total_a_receber
FROM obras o
LEFT JOIN contas_receber cr
       ON cr.obra_id = o.id
      AND cr.status IN ('pendente','parcial')
WHERE o.deleted_at IS NULL
GROUP BY o.id, o.codigo, o.nome, o.contrato_valor;
");

            migrationBuilder.Sql(@"
CREATE OR REPLACE VIEW vw_capacitacoes_alerta AS
SELECT
  f.id AS funcionario_id,
  f.nome,
  t.codigo,
  t.nome AS capacitacao,
  c.data_validade,
  DATEDIFF(c.data_validade, CURDATE()) AS dias_restantes,
  CASE
    WHEN c.data_validade < CURDATE() THEN 'vencida'
    WHEN c.data_validade <= DATE_ADD(CURDATE(), INTERVAL 30 DAY) THEN 'vence_em_30d'
    ELSE 'ok'
  END AS situacao
FROM funcionario_capacitacoes c
JOIN funcionarios f ON f.id = c.funcionario_id
JOIN tipos_capacitacao t ON t.id = c.tipo_id
WHERE f.ativo = TRUE
  AND f.deleted_at IS NULL
  AND c.data_validade IS NOT NULL;
");

            migrationBuilder.Sql(@"ALTER TABLE agenda ALTER COLUMN concluido SET DEFAULT 0;");
            migrationBuilder.Sql(@"ALTER TABLE categorias_estoque ALTER COLUMN controla_saldo_obra SET DEFAULT 0;");
            migrationBuilder.Sql(@"ALTER TABLE categorias_estoque ALTER COLUMN exige_devolucao SET DEFAULT 0;");
            migrationBuilder.Sql(@"ALTER TABLE contas_pagar ALTER COLUMN valor_pago SET DEFAULT 0.00;");
            migrationBuilder.Sql(@"ALTER TABLE contas_receber ALTER COLUMN valor_pago SET DEFAULT 0.00;");
            migrationBuilder.Sql(@"ALTER TABLE contratos_empreitada ALTER COLUMN valor_total SET DEFAULT 0.00;");
            migrationBuilder.Sql(@"ALTER TABLE contratos_empreitada ALTER COLUMN retencao_inss_pct SET DEFAULT 0.00;");
            migrationBuilder.Sql(@"ALTER TABLE estoque_movimentacoes ALTER COLUMN custo_unitario SET DEFAULT 0.00;");
            migrationBuilder.Sql(@"ALTER TABLE etapas ALTER COLUMN ordem SET DEFAULT 0;");
            migrationBuilder.Sql(@"ALTER TABLE etapas ALTER COLUMN percentual SET DEFAULT 0.00;");
            migrationBuilder.Sql(@"ALTER TABLE funcionarios ALTER COLUMN valor SET DEFAULT 0.00;");
            migrationBuilder.Sql(@"ALTER TABLE funcionarios ALTER COLUMN adicional_percentual SET DEFAULT 0.00;");
            migrationBuilder.Sql(@"ALTER TABLE itens_estoque ALTER COLUMN minimo SET DEFAULT 0.000;");
            migrationBuilder.Sql(@"ALTER TABLE itens_estoque ALTER COLUMN preco_ref SET DEFAULT 0.00;");
            migrationBuilder.Sql(@"ALTER TABLE obra_aditivos ALTER COLUMN valor SET DEFAULT 0.00;");
            migrationBuilder.Sql(@"ALTER TABLE obra_aditivos ALTER COLUMN dias_prazo SET DEFAULT 0;");
            migrationBuilder.Sql(@"ALTER TABLE obra_links ALTER COLUMN total_acessos SET DEFAULT 0;");
            migrationBuilder.Sql(@"ALTER TABLE obras ALTER COLUMN contrato_valor SET DEFAULT 0.00;");
            migrationBuilder.Sql(@"ALTER TABLE ponto ALTER COLUMN valor_hora_snapshot SET DEFAULT 0.0000;");
            migrationBuilder.Sql(@"ALTER TABLE ponto ALTER COLUMN adicional_snapshot SET DEFAULT 0.00;");
            migrationBuilder.Sql(@"ALTER TABLE ponto ALTER COLUMN encargos_snapshot SET DEFAULT 0.00;");
            migrationBuilder.Sql(@"ALTER TABLE ponto ALTER COLUMN custo SET DEFAULT 0.00;");
            migrationBuilder.Sql(@"ALTER TABLE solicitacao_itens ALTER COLUMN qtd_atendida SET DEFAULT 0.000;");
            migrationBuilder.Sql(@"ALTER TABLE tipos_capacitacao ALTER COLUMN exige_presencial SET DEFAULT 0;");
            migrationBuilder.Sql(@"ALTER TABLE usuarios ALTER COLUMN tentativas_falhas SET DEFAULT 0;");

            // ---------- Seeds (16) ----------
            migrationBuilder.Sql(@"
INSERT INTO papeis (nome, descricao) VALUES
  ('admin',             'Diretoria e administracao'),
  ('engenheiro',        'Responsavel tecnico da obra'),
  ('mestre',            'Mestre de obras e encarregados'),
  ('tecnico_seguranca', 'SESMT - EPI, capacitacoes e NR'),
  ('almoxarife',        'Almoxarifado do canteiro'),
  ('compras',           'Cotacao e compra de material');
");

            migrationBuilder.Sql(@"
INSERT INTO permissoes (chave, descricao) VALUES
  ('ver_financeiro',      'Ver contrato, custos, margem e financeiro'),
  ('gerir_financeiro',    'Lancar contas e pagamentos'),
  ('ver_todas_obras',     'Ver obras alem das atribuidas'),
  ('gerir_obras',         'Criar e editar obras'),
  ('gerir_etapas',        'Marcar andamento das etapas'),
  ('lancar_ponto',        'Apontar efetivo'),
  ('ver_salarios',        'Ver remuneracao de funcionarios'),
  ('gerir_funcionarios',  'Cadastrar e editar funcionarios'),
  ('solicitar_compra',    'Abrir solicitacao de compra'),
  ('aprovar_compra',      'Aprovar e efetivar compra'),
  ('movimentar_estoque',  'Registrar entrada e saida de material'),
  ('gerir_epi',           'Entregar EPI e emitir ficha'),
  ('gerir_capacitacoes',  'Registrar NR, ASO e certificados'),
  ('gerir_usuarios',      'Administrar usuarios e papeis');
");

            migrationBuilder.Sql(@"
INSERT INTO papel_permissoes (papel_id, permissao_id)
SELECT (SELECT id FROM papeis WHERE nome = 'admin'), id FROM permissoes;
");

            migrationBuilder.Sql(@"
INSERT INTO papel_permissoes (papel_id, permissao_id)
SELECT (SELECT id FROM papeis WHERE nome = 'mestre'), id
FROM permissoes
WHERE chave IN ('gerir_etapas','lancar_ponto','solicitar_compra',
                'movimentar_estoque','gerir_epi');
");

            migrationBuilder.Sql(@"
INSERT INTO papel_permissoes (papel_id, permissao_id)
SELECT (SELECT id FROM papeis WHERE nome = 'engenheiro'), id
FROM permissoes
WHERE chave IN ('gerir_etapas','lancar_ponto','solicitar_compra',
                'movimentar_estoque','gerir_epi','gerir_obras','ver_todas_obras');
");

            migrationBuilder.Sql(@"
INSERT INTO papel_permissoes (papel_id, permissao_id)
SELECT (SELECT id FROM papeis WHERE nome = 'tecnico_seguranca'), id
FROM permissoes
WHERE chave IN ('gerir_epi','gerir_capacitacoes','ver_todas_obras');
");

            migrationBuilder.Sql(@"
INSERT INTO papel_permissoes (papel_id, permissao_id)
SELECT (SELECT id FROM papeis WHERE nome = 'almoxarife'), id
FROM permissoes
WHERE chave IN ('movimentar_estoque','gerir_epi','solicitar_compra');
");

            migrationBuilder.Sql(@"
INSERT INTO papel_permissoes (papel_id, permissao_id)
SELECT (SELECT id FROM papeis WHERE nome = 'compras'), id
FROM permissoes
WHERE chave IN ('aprovar_compra','movimentar_estoque','ver_financeiro');
");

            migrationBuilder.Sql(@"
INSERT INTO setores (nome) VALUES
  ('Administrativo'), ('Alvenaria'), ('Acabamento'), ('Estrutura'),
  ('Terraplanagem'), ('Eletrica'), ('Hidraulica'), ('Seguranca do Trabalho');
");

            migrationBuilder.Sql(@"
INSERT INTO funcoes (nome, setor_id) VALUES
  ('Mestre de obras',       (SELECT id FROM setores WHERE nome='Administrativo')),
  ('Engenheiro civil',      (SELECT id FROM setores WHERE nome='Administrativo')),
  ('Encarregado',           (SELECT id FROM setores WHERE nome='Administrativo')),
  ('Almoxarife',            (SELECT id FROM setores WHERE nome='Administrativo')),
  ('Tecnico de seguranca',  (SELECT id FROM setores WHERE nome='Seguranca do Trabalho')),
  ('Pedreiro',              (SELECT id FROM setores WHERE nome='Alvenaria')),
  ('Servente',              (SELECT id FROM setores WHERE nome='Alvenaria')),
  ('Armador',               (SELECT id FROM setores WHERE nome='Estrutura')),
  ('Carpinteiro',           (SELECT id FROM setores WHERE nome='Estrutura')),
  ('Pintor',                (SELECT id FROM setores WHERE nome='Acabamento')),
  ('Azulejista',            (SELECT id FROM setores WHERE nome='Acabamento')),
  ('Eletricista',           (SELECT id FROM setores WHERE nome='Eletrica')),
  ('Encanador',             (SELECT id FROM setores WHERE nome='Hidraulica')),
  ('Operador de maquinas',  (SELECT id FROM setores WHERE nome='Terraplanagem'));
");

            migrationBuilder.Sql(@"
INSERT INTO regimes (rotulo, unidade, horas_mes, descricao) VALUES
  ('CLT mensalista', 'mes',  220.00, 'custo_hora = valor / horas_mes'),
  ('CLT horista',    'hora',   NULL, 'Valor por hora trabalhada'),
  ('Diarista',       'dia',    NULL, 'Valor por dia'),
  ('Empreitada',     'global', NULL, 'Custo vem de contratos_empreitada');
");

            migrationBuilder.Sql(@"
INSERT INTO parametros_encargos (regime_id, percentual, vigencia_inicio, fonte, observacao) VALUES
  ((SELECT id FROM regimes WHERE rotulo='CLT horista'),    115.60, '2026-09-13',
   'SINAPI Encargos Sociais - Florianopolis/SC - ref. dez/2025 - sem desoneracao',
   'Congelado em 13/09/2026.'),
  ((SELECT id FROM regimes WHERE rotulo='CLT mensalista'),  71.80, '2026-09-13',
   'SINAPI Encargos Sociais - Florianopolis/SC - ref. dez/2025 - sem desoneracao',
   'Congelado em 13/09/2026.'),
  ((SELECT id FROM regimes WHERE rotulo='Diarista'),          0.00, '2026-09-13',
   'Nao aplicavel', 'Diaria contratada com valor cheio.'),
  ((SELECT id FROM regimes WHERE rotulo='Empreitada'),        0.00, '2026-09-13',
   'Nao aplicavel', 'Ha retencao de 11% na NF em cessao de mao de obra.');
");

            migrationBuilder.Sql(@"
INSERT INTO tipos_capacitacao (codigo, nome, validade_meses, carga_horaria_min, exige_presencial, observacao) VALUES
  ('NR-35', 'Trabalho em Altura',                    24,  8, TRUE,
   'Obrigatorio acima de 2m. Desde 07/2026 exige treinamento presencial.'),
  ('NR-10', 'Seguranca em Instalacoes Eletricas',    24, 40, FALSE,
   'Basico 40h. SEP exige complementar de mais 40h.'),
  ('NR-18', 'Integracao - Industria da Construcao',  24,  6, FALSE,
   'Admissional e periodico, especifico por obra.'),
  ('NR-33', 'Espacos Confinados',                    12, 16, FALSE,
   'Poco, reservatorio, galeria, fossa.'),
  ('NR-11', 'Movimentacao de Cargas',                24, 16, FALSE,
   'Guincho, talha, empilhadeira.'),
  ('NR-12', 'Maquinas e Equipamentos',               24,  8, FALSE,
   'Capacitacao especifica por maquina.'),
  ('NR-06', 'Uso de EPI',                            NULL, 2, FALSE,
   'Complementa a ficha de entrega.'),
  ('NR-20', 'Inflamaveis e Combustiveis',            24,  8, FALSE,
   'Aplicavel se houver tanque no canteiro.'),
  ('NR-05', 'CIPA',                                  NULL, 20, FALSE,
   'Somente membros eleitos. Validade igual ao mandato.'),
  ('ASO',   'Atestado de Saude Ocupacional',         12, NULL, TRUE,
   'NR-7. Vencido impede o trabalhador de atuar.'),
  ('GRUA',  'Operador de Grua',                      24, 40, TRUE,
   'NR-18. Exige ASO especifico.'),
  ('CNH',   'Carteira Nacional de Habilitacao',      NULL, NULL, FALSE,
   'Para quem dirige veiculo da empresa.'),
  ('CREA',  'Registro Profissional CREA',            NULL, NULL, FALSE,
   'Responsavel tecnico.');
");

            migrationBuilder.Sql(@"
INSERT INTO categorias_estoque (nome, controla_saldo_obra, exige_devolucao) VALUES
  ('consumo',    TRUE,  FALSE),
  ('epi',        FALSE, FALSE),
  ('ferramenta', FALSE, TRUE);
");

            migrationBuilder.Sql(@"
INSERT INTO grupos (nome, categoria_id) VALUES
  ('Estrutura',             (SELECT id FROM categorias_estoque WHERE nome='consumo')),
  ('Alvenaria',             (SELECT id FROM categorias_estoque WHERE nome='consumo')),
  ('Acabamento',            (SELECT id FROM categorias_estoque WHERE nome='consumo')),
  ('Eletrica',              (SELECT id FROM categorias_estoque WHERE nome='consumo')),
  ('Hidraulica',            (SELECT id FROM categorias_estoque WHERE nome='consumo')),
  ('Protecao da cabeca',    (SELECT id FROM categorias_estoque WHERE nome='epi')),
  ('Protecao dos pes',      (SELECT id FROM categorias_estoque WHERE nome='epi')),
  ('Protecao das maos',     (SELECT id FROM categorias_estoque WHERE nome='epi')),
  ('Protecao contra queda', (SELECT id FROM categorias_estoque WHERE nome='epi')),
  ('Protecao respiratoria', (SELECT id FROM categorias_estoque WHERE nome='epi')),
  ('Ferramenta manual',     (SELECT id FROM categorias_estoque WHERE nome='ferramenta')),
  ('Ferramenta eletrica',   (SELECT id FROM categorias_estoque WHERE nome='ferramenta'));
");

            migrationBuilder.Sql(@"
INSERT INTO usuarios (usuario, senha_hash, nome, email, papel_id) VALUES
  ('admin',
   'TROCAR_POR_HASH_BCRYPT_COST_12',
   'Administrador',
   'admin@noprumo.local',
   (SELECT id FROM papeis WHERE nome = 'admin'));
");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS vw_saldo_obra;");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS vw_saldo_deposito;");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS vw_estoque_baixo;");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS vw_obras_custos;");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS vw_obras_resultado;");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS vw_obras_situacao;");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS vw_etapas_situacao;");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS vw_clientes_financeiro;");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS vw_obras_financeiro;");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS vw_capacitacoes_alerta;");

            migrationBuilder.DropTable(
                name: "agenda");

            migrationBuilder.DropTable(
                name: "auditoria");

            migrationBuilder.DropTable(
                name: "equipe_obras");

            migrationBuilder.DropTable(
                name: "estoque_movimentacoes");

            migrationBuilder.DropTable(
                name: "ficha_itens");

            migrationBuilder.DropTable(
                name: "funcionario_capacitacoes");

            migrationBuilder.DropTable(
                name: "funcionario_equipes");

            migrationBuilder.DropTable(
                name: "medicoes_empreitada");

            migrationBuilder.DropTable(
                name: "obra_aditivos");

            migrationBuilder.DropTable(
                name: "obra_links");

            migrationBuilder.DropTable(
                name: "pagamentos");

            migrationBuilder.DropTable(
                name: "papel_permissoes");

            migrationBuilder.DropTable(
                name: "parametros_encargos");

            migrationBuilder.DropTable(
                name: "ponto");

            migrationBuilder.DropTable(
                name: "solicitacao_itens");

            migrationBuilder.DropTable(
                name: "usuario_obras");

            migrationBuilder.DropTable(
                name: "fichas");

            migrationBuilder.DropTable(
                name: "tipos_capacitacao");

            migrationBuilder.DropTable(
                name: "contratos_empreitada");

            migrationBuilder.DropTable(
                name: "contas_pagar");

            migrationBuilder.DropTable(
                name: "contas_receber");

            migrationBuilder.DropTable(
                name: "permissoes");

            migrationBuilder.DropTable(
                name: "itens_estoque");

            migrationBuilder.DropTable(
                name: "solicitacoes_compra");

            migrationBuilder.DropTable(
                name: "etapas");

            migrationBuilder.DropTable(
                name: "fornecedores");

            migrationBuilder.DropTable(
                name: "grupos");

            migrationBuilder.DropTable(
                name: "equipes");

            migrationBuilder.DropTable(
                name: "obras");

            migrationBuilder.DropTable(
                name: "categorias_estoque");

            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "funcionarios");

            migrationBuilder.DropTable(
                name: "papeis");

            migrationBuilder.DropTable(
                name: "funcoes");

            migrationBuilder.DropTable(
                name: "regimes");

            migrationBuilder.DropTable(
                name: "setores");
        }
    }
}
