using Microsoft.EntityFrameworkCore;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Infrastructure.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agenda> Agenda { get; set; }

    public virtual DbSet<Auditoria> Auditoria { get; set; }

    public virtual DbSet<CategoriaEstoque> CategoriaEstoque { get; set; }

    public virtual DbSet<Cliente> Cliente { get; set; }

    public virtual DbSet<ContasPagar> ContasPagar { get; set; }

    public virtual DbSet<ContasReceber> ContasReceber { get; set; }

    public virtual DbSet<ContratoEmpreitada> ContratoEmpreitada { get; set; }

    public virtual DbSet<EquipeObra> EquipeObra { get; set; }

    public virtual DbSet<Equipe> Equipe { get; set; }

    public virtual DbSet<EstoqueMovimentacao> EstoqueMovimentacao { get; set; }

    public virtual DbSet<Etapa> Etapa { get; set; }

    public virtual DbSet<FichaItem> FichaItem { get; set; }

    public virtual DbSet<Ficha> Ficha { get; set; }

    public virtual DbSet<Fornecedor> Fornecedor { get; set; }

    public virtual DbSet<FuncionarioCapacitacao> FuncionarioCapacitacao { get; set; }

    public virtual DbSet<FuncionarioEquipe> FuncionarioEquipe { get; set; }

    public virtual DbSet<Funcionario> Funcionario { get; set; }

    public virtual DbSet<Funcao> Funcao { get; set; }

    public virtual DbSet<Grupo> Grupo { get; set; }

    public virtual DbSet<ItemEstoque> ItemEstoque { get; set; }

    public virtual DbSet<MedicaoEmpreitada> MedicaoEmpreitada { get; set; }

    public virtual DbSet<ObraAditivo> ObraAditivo { get; set; }

    public virtual DbSet<ObraLink> ObraLink { get; set; }

    public virtual DbSet<Obra> Obra { get; set; }

    public virtual DbSet<Pagamento> Pagamento { get; set; }

    public virtual DbSet<Papel> Papel { get; set; }

    public virtual DbSet<ParametroEncargo> ParametroEncargo { get; set; }

    public virtual DbSet<Permissao> Permissao { get; set; }

    public virtual DbSet<Ponto> Ponto { get; set; }

    public virtual DbSet<Regime> Regime { get; set; }

    public virtual DbSet<Setor> Setor { get; set; }

    public virtual DbSet<SolicitacaoItem> SolicitacaoItem { get; set; }

    public virtual DbSet<SolicitacaoCompra> SolicitacaoCompra { get; set; }

    public virtual DbSet<TipoCapacitacao> TipoCapacitacao { get; set; }

    public virtual DbSet<UsuarioObra> UsuarioObra { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Agenda>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("agenda");

            entity.HasIndex(e => new { e.Data, e.Hora }, "idx_agenda_data_hora");

            entity.HasIndex(e => e.ObraId, "idx_agenda_obra");

            entity.HasIndex(e => e.ResponsavelId, "idx_agenda_responsavel");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Concluido).HasColumnName("concluido");
            entity.Property(e => e.ConcluidoEm)
                .HasColumnType("datetime")
                .HasColumnName("concluido_em");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.Descricao)
                .HasColumnType("text")
                .HasColumnName("descricao");
            entity.Property(e => e.Hora)
                .HasColumnType("time")
                .HasColumnName("hora");
            entity.Property(e => e.ObraId).HasColumnName("obra_id");
            entity.Property(e => e.ResponsavelId).HasColumnName("responsavel_id");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasDefaultValueSql("'obra'")
                .HasColumnName("tipo");
            entity.Property(e => e.Titulo)
                .HasMaxLength(220)
                .HasColumnName("titulo");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Obra).WithMany(p => p.Agenda)
                .HasForeignKey(d => d.ObraId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_agenda_obra");

            entity.HasOne(d => d.Responsavel).WithMany(p => p.Agenda)
                .HasForeignKey(d => d.ResponsavelId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_agenda_responsavel");
        });

        modelBuilder.Entity<Auditoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("auditoria");

            entity.HasIndex(e => e.CreatedAt, "idx_auditoria_data");

            entity.HasIndex(e => new { e.Tabela, e.RegistroId }, "idx_auditoria_tabela_registro");

            entity.HasIndex(e => new { e.UsuarioId, e.CreatedAt }, "idx_auditoria_usuario_data");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Acao)
                .HasMaxLength(20)
                .HasColumnName("acao");
            entity.Property(e => e.CamposAlterados)
                .HasColumnType("json")
                .HasColumnName("campos_alterados");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DadosAnteriores)
                .HasColumnType("json")
                .HasColumnName("dados_anteriores");
            entity.Property(e => e.DadosNovos)
                .HasColumnType("json")
                .HasColumnName("dados_novos");
            entity.Property(e => e.Ip)
                .HasMaxLength(45)
                .HasColumnName("ip");
            entity.Property(e => e.RegistroId).HasColumnName("registro_id");
            entity.Property(e => e.Tabela)
                .HasMaxLength(100)
                .HasColumnName("tabela");
            entity.Property(e => e.UserAgent)
                .HasMaxLength(255)
                .HasColumnName("user_agent");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Auditoria)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_auditoria_usuario");
        });

        modelBuilder.Entity<CategoriasEstoque>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("categorias_estoque");

            entity.HasIndex(e => e.Nome, "uq_categorias_estoque_nome").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ControlaSaldoObra).HasColumnName("controla_saldo_obra");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ExigeDevolucao).HasColumnName("exige_devolucao");
            entity.Property(e => e.Nome)
                .HasMaxLength(60)
                .HasColumnName("nome");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Clientes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("clientes");

            entity.HasIndex(e => e.Cidade, "idx_clientes_cidade");

            entity.HasIndex(e => e.DeletedAt, "idx_clientes_deleted");

            entity.HasIndex(e => e.Nome, "idx_clientes_nome");

            entity.HasIndex(e => new { e.DocumentoHash, e.AtivoKey }, "uq_clientes_documento").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AtivoKey)
                .HasMaxLength(6)
                .HasComputedColumnSql("ifnull(`deleted_at`,'1970-01-01 00:00:00')", true)
                .HasColumnName("ativo_key");
            entity.Property(e => e.Bairro)
                .HasMaxLength(100)
                .HasColumnName("bairro");
            entity.Property(e => e.Celular)
                .HasMaxLength(30)
                .HasColumnName("celular");
            entity.Property(e => e.Cep)
                .HasMaxLength(10)
                .HasColumnName("cep");
            entity.Property(e => e.Cidade)
                .HasMaxLength(120)
                .HasColumnName("cidade");
            entity.Property(e => e.Complemento)
                .HasMaxLength(100)
                .HasColumnName("complemento");
            entity.Property(e => e.Contato)
                .HasMaxLength(160)
                .HasColumnName("contato");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasMaxLength(6)
                .HasColumnName("deleted_at");
            entity.Property(e => e.DocumentoCifrado)
                .HasMaxLength(512)
                .HasColumnName("documento_cifrado");
            entity.Property(e => e.DocumentoHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("documento_hash");
            entity.Property(e => e.DocumentoMascara)
                .HasMaxLength(20)
                .HasColumnName("documento_mascara");
            entity.Property(e => e.Email)
                .HasMaxLength(160)
                .HasColumnName("email");
            entity.Property(e => e.Endereco)
                .HasMaxLength(255)
                .HasColumnName("endereco");
            entity.Property(e => e.Estado)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("estado");
            entity.Property(e => e.Nome)
                .HasMaxLength(160)
                .HasColumnName("nome");
            entity.Property(e => e.Numero)
                .HasMaxLength(20)
                .HasColumnName("numero");
            entity.Property(e => e.Observacoes)
                .HasColumnType("text")
                .HasColumnName("observacoes");
            entity.Property(e => e.Telefone)
                .HasMaxLength(30)
                .HasColumnName("telefone");
            entity.Property(e => e.TipoPessoa)
                .HasMaxLength(10)
                .HasDefaultValueSql("'PJ'")
                .HasColumnName("tipo_pessoa");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<ContasPagar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("contas_pagar");

            entity.HasIndex(e => e.FornecedorId, "idx_contas_pagar_fornecedor");

            entity.HasIndex(e => e.ObraId, "idx_contas_pagar_obra");

            entity.HasIndex(e => new { e.OrigemTipo, e.OrigemId }, "idx_contas_pagar_origem");

            entity.HasIndex(e => new { e.Vencimento, e.Status }, "idx_contas_pagar_vencimento_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Descricao)
                .HasMaxLength(255)
                .HasColumnName("descricao");
            entity.Property(e => e.FornecedorId).HasColumnName("fornecedor_id");
            entity.Property(e => e.NumeroNf)
                .HasMaxLength(40)
                .HasColumnName("numero_nf");
            entity.Property(e => e.ObraId).HasColumnName("obra_id");
            entity.Property(e => e.Observacao)
                .HasColumnType("text")
                .HasColumnName("observacao");
            entity.Property(e => e.OrigemId).HasColumnName("origem_id");
            entity.Property(e => e.OrigemTipo)
                .HasMaxLength(30)
                .HasColumnName("origem_tipo");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'pendente'")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Valor)
                .HasPrecision(15, 2)
                .HasColumnName("valor");
            entity.Property(e => e.ValorPago)
                .HasPrecision(15, 2)
                .HasColumnName("valor_pago");
            entity.Property(e => e.Vencimento).HasColumnName("vencimento");

            entity.HasOne(d => d.Fornecedor).WithMany(p => p.ContasPagar)
                .HasForeignKey(d => d.FornecedorId)
                .HasConstraintName("fk_contas_pagar_fornecedor");

            entity.HasOne(d => d.Obra).WithMany(p => p.ContasPagar)
                .HasForeignKey(d => d.ObraId)
                .HasConstraintName("fk_contas_pagar_obra");
        });

        modelBuilder.Entity<ContasReceber>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("contas_receber");

            entity.HasIndex(e => e.ClienteId, "idx_contas_receber_cliente");

            entity.HasIndex(e => e.ObraId, "idx_contas_receber_obra");

            entity.HasIndex(e => new { e.Vencimento, e.Status }, "idx_contas_receber_vencimento_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClienteId).HasColumnName("cliente_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Descricao)
                .HasMaxLength(255)
                .HasColumnName("descricao");
            entity.Property(e => e.NumeroNf)
                .HasMaxLength(40)
                .HasColumnName("numero_nf");
            entity.Property(e => e.ObraId).HasColumnName("obra_id");
            entity.Property(e => e.Observacao)
                .HasColumnType("text")
                .HasColumnName("observacao");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'pendente'")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Valor)
                .HasPrecision(15, 2)
                .HasColumnName("valor");
            entity.Property(e => e.ValorPago)
                .HasPrecision(15, 2)
                .HasColumnName("valor_pago");
            entity.Property(e => e.Vencimento).HasColumnName("vencimento");

            entity.HasOne(d => d.Cliente).WithMany(p => p.ContasReceber)
                .HasForeignKey(d => d.ClienteId)
                .HasConstraintName("fk_contas_receber_cliente");

            entity.HasOne(d => d.Obra).WithMany(p => p.ContasReceber)
                .HasForeignKey(d => d.ObraId)
                .HasConstraintName("fk_contas_receber_obra");
        });

        modelBuilder.Entity<ContratosEmpreitada>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("contratos_empreitada");

            entity.HasIndex(e => e.EtapaId, "idx_empreitada_etapa");

            entity.HasIndex(e => e.FornecedorId, "idx_empreitada_fornecedor");

            entity.HasIndex(e => e.ObraId, "idx_empreitada_obra");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DataFim).HasColumnName("data_fim");
            entity.Property(e => e.DataInicio).HasColumnName("data_inicio");
            entity.Property(e => e.Descricao)
                .HasMaxLength(255)
                .HasColumnName("descricao");
            entity.Property(e => e.EtapaId).HasColumnName("etapa_id");
            entity.Property(e => e.FornecedorId).HasColumnName("fornecedor_id");
            entity.Property(e => e.ObraId).HasColumnName("obra_id");
            entity.Property(e => e.Observacao)
                .HasColumnType("text")
                .HasColumnName("observacao");
            entity.Property(e => e.PrecoUnitario)
                .HasPrecision(15, 2)
                .HasColumnName("preco_unitario");
            entity.Property(e => e.QuantidadePrevista)
                .HasPrecision(15, 3)
                .HasColumnName("quantidade_prevista");
            entity.Property(e => e.RetencaoInssPct)
                .HasPrecision(5, 2)
                .HasColumnName("retencao_inss_pct");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'ativo'")
                .HasColumnName("status");
            entity.Property(e => e.TipoPreco)
                .HasMaxLength(20)
                .HasDefaultValueSql("'global'")
                .HasColumnName("tipo_preco");
            entity.Property(e => e.Unidade)
                .HasMaxLength(30)
                .HasColumnName("unidade");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.ValorTotal)
                .HasPrecision(15, 2)
                .HasColumnName("valor_total");

            entity.HasOne(d => d.Etapa).WithMany(p => p.ContratosEmpreitada)
                .HasForeignKey(d => d.EtapaId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_empreitada_etapa");

            entity.HasOne(d => d.Fornecedor).WithMany(p => p.ContratosEmpreitada)
                .HasForeignKey(d => d.FornecedorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_empreitada_fornecedor");

            entity.HasOne(d => d.Obra).WithMany(p => p.ContratosEmpreitada)
                .HasForeignKey(d => d.ObraId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_empreitada_obra");
        });

        modelBuilder.Entity<EquipeObras>(entity =>
        {
            entity.HasKey(e => new { e.EquipeId, e.ObraId, e.DataInicio })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity.ToTable("equipe_obras");

            entity.HasIndex(e => e.ObraId, "idx_equipe_obras_obra");

            entity.Property(e => e.EquipeId).HasColumnName("equipe_id");
            entity.Property(e => e.ObraId).HasColumnName("obra_id");
            entity.Property(e => e.DataInicio).HasColumnName("data_inicio");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DataFim).HasColumnName("data_fim");

            entity.HasOne(d => d.Equipe).WithMany(p => p.EquipeObras)
                .HasForeignKey(d => d.EquipeId)
                .HasConstraintName("fk_equipe_obras_equipe");

            entity.HasOne(d => d.Obra).WithMany(p => p.EquipeObras)
                .HasForeignKey(d => d.ObraId)
                .HasConstraintName("fk_equipe_obras_obra");
        });

        modelBuilder.Entity<Equipes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("equipes");

            entity.HasIndex(e => e.SetorId, "idx_equipes_setor");

            entity.HasIndex(e => new { e.Nome, e.SetorId, e.AtivoKey }, "uq_equipes_nome_setor").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AtivoKey)
                .HasMaxLength(6)
                .HasComputedColumnSql("ifnull(`deleted_at`,'1970-01-01 00:00:00')", true)
                .HasColumnName("ativo_key");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasMaxLength(6)
                .HasColumnName("deleted_at");
            entity.Property(e => e.Nome)
                .HasMaxLength(120)
                .HasColumnName("nome");
            entity.Property(e => e.SetorId).HasColumnName("setor_id");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Setor).WithMany(p => p.Equipes)
                .HasForeignKey(d => d.SetorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_equipes_setor");
        });

        modelBuilder.Entity<EstoqueMovimentacoes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("estoque_movimentacoes");

            entity.HasIndex(e => e.FornecedorId, "fk_estoque_mov_fornecedor");

            entity.HasIndex(e => e.RegistradoPor, "fk_estoque_mov_registrador");

            entity.HasIndex(e => new { e.FuncionarioId, e.Data }, "idx_estoque_mov_funcionario");

            entity.HasIndex(e => new { e.ItemId, e.Data }, "idx_estoque_mov_item_data");

            entity.HasIndex(e => new { e.ItemId, e.ObraId }, "idx_estoque_mov_item_obra");

            entity.HasIndex(e => new { e.ObraId, e.Data }, "idx_estoque_mov_obra_data");

            entity.HasIndex(e => new { e.OrigemTipo, e.OrigemId }, "idx_estoque_mov_origem");

            entity.HasIndex(e => e.TransferenciaId, "idx_estoque_mov_transferencia");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CustoUnitario)
                .HasPrecision(15, 2)
                .HasColumnName("custo_unitario");
            entity.Property(e => e.Data)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("data");
            entity.Property(e => e.FornecedorId).HasColumnName("fornecedor_id");
            entity.Property(e => e.FuncionarioId).HasColumnName("funcionario_id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.NumeroNf)
                .HasMaxLength(40)
                .HasColumnName("numero_nf");
            entity.Property(e => e.ObraId).HasColumnName("obra_id");
            entity.Property(e => e.Observacao)
                .HasMaxLength(500)
                .HasColumnName("observacao");
            entity.Property(e => e.OrigemId).HasColumnName("origem_id");
            entity.Property(e => e.OrigemTipo)
                .HasMaxLength(30)
                .HasColumnName("origem_tipo");
            entity.Property(e => e.Quantidade)
                .HasPrecision(15, 3)
                .HasColumnName("quantidade");
            entity.Property(e => e.RegistradoPor).HasColumnName("registrado_por");
            entity.Property(e => e.Tipo)
                .HasMaxLength(25)
                .HasColumnName("tipo");
            entity.Property(e => e.TransferenciaId).HasColumnName("transferencia_id");
            entity.Property(e => e.Unidade)
                .HasMaxLength(30)
                .HasColumnName("unidade");

            entity.HasOne(d => d.Fornecedor).WithMany(p => p.EstoqueMovimentacoes)
                .HasForeignKey(d => d.FornecedorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_estoque_mov_fornecedor");

            entity.HasOne(d => d.Funcionario).WithMany(p => p.EstoqueMovimentacoes)
                .HasForeignKey(d => d.FuncionarioId)
                .HasConstraintName("fk_estoque_mov_funcionario");

            entity.HasOne(d => d.Item).WithMany(p => p.EstoqueMovimentacoes)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_estoque_mov_item");

            entity.HasOne(d => d.Obra).WithMany(p => p.EstoqueMovimentacoes)
                .HasForeignKey(d => d.ObraId)
                .HasConstraintName("fk_estoque_mov_obra");

            entity.HasOne(d => d.RegistradoPorNavigation).WithMany(p => p.EstoqueMovimentacoes)
                .HasForeignKey(d => d.RegistradoPor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_estoque_mov_registrador");
        });

        modelBuilder.Entity<Etapas>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("etapas");

            entity.HasIndex(e => e.MarcadoPor, "fk_etapas_marcado_por");

            entity.HasIndex(e => e.DataPrevista, "idx_etapas_data");

            entity.HasIndex(e => e.EquipeId, "idx_etapas_equipe");

            entity.HasIndex(e => e.ObraId, "idx_etapas_obra");

            entity.HasIndex(e => new { e.ObraId, e.Status }, "idx_etapas_obra_status");

            entity.HasIndex(e => e.ResponsavelId, "idx_etapas_responsavel");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DataConclusao).HasColumnName("data_conclusao");
            entity.Property(e => e.DataInicio).HasColumnName("data_inicio");
            entity.Property(e => e.DataPrevista).HasColumnName("data_prevista");
            entity.Property(e => e.EquipeId).HasColumnName("equipe_id");
            entity.Property(e => e.MarcadoEm)
                .HasColumnType("datetime")
                .HasColumnName("marcado_em");
            entity.Property(e => e.MarcadoPor).HasColumnName("marcado_por");
            entity.Property(e => e.Nome)
                .HasMaxLength(180)
                .HasColumnName("nome");
            entity.Property(e => e.ObraId).HasColumnName("obra_id");
            entity.Property(e => e.Observacao)
                .HasColumnType("text")
                .HasColumnName("observacao");
            entity.Property(e => e.Ordem).HasColumnName("ordem");
            entity.Property(e => e.Percentual)
                .HasPrecision(5, 2)
                .HasColumnName("percentual");
            entity.Property(e => e.ResponsavelId).HasColumnName("responsavel_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'prevista'")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Equipe).WithMany(p => p.Etapas)
                .HasForeignKey(d => d.EquipeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_etapas_equipe");

            entity.HasOne(d => d.MarcadoPorNavigation).WithMany(p => p.Etapas)
                .HasForeignKey(d => d.MarcadoPor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_etapas_marcado_por");

            entity.HasOne(d => d.Obra).WithMany(p => p.Etapas)
                .HasForeignKey(d => d.ObraId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_etapas_obra");

            entity.HasOne(d => d.Responsavel).WithMany(p => p.Etapas)
                .HasForeignKey(d => d.ResponsavelId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_etapas_responsavel");
        });

        modelBuilder.Entity<FichaItens>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ficha_itens");

            entity.HasIndex(e => e.FichaId, "idx_ficha_itens_ficha");

            entity.HasIndex(e => e.ItemId, "idx_ficha_itens_item");

            entity.HasIndex(e => e.Status, "idx_ficha_itens_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CaSnapshot)
                .HasMaxLength(30)
                .HasColumnName("ca_snapshot");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DataDevolucao).HasColumnName("data_devolucao");
            entity.Property(e => e.DataEntrega).HasColumnName("data_entrega");
            entity.Property(e => e.FichaId).HasColumnName("ficha_id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ItemSnapshot)
                .HasMaxLength(180)
                .HasColumnName("item_snapshot");
            entity.Property(e => e.Observacao)
                .HasMaxLength(500)
                .HasColumnName("observacao");
            entity.Property(e => e.Quantidade)
                .HasPrecision(15, 3)
                .HasDefaultValueSql("'1.000'")
                .HasColumnName("quantidade");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'entregue'")
                .HasColumnName("status");
            entity.Property(e => e.Unidade)
                .HasMaxLength(30)
                .HasColumnName("unidade");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Ficha).WithMany(p => p.FichaItens)
                .HasForeignKey(d => d.FichaId)
                .HasConstraintName("fk_ficha_itens_ficha");

            entity.HasOne(d => d.Item).WithMany(p => p.FichaItens)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_ficha_itens_item");
        });

        modelBuilder.Entity<Fichas>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("fichas");

            entity.HasIndex(e => new { e.FuncionarioId, e.Data }, "idx_fichas_funcionario_data");

            entity.HasIndex(e => e.ObraId, "idx_fichas_obra");

            entity.HasIndex(e => e.ResponsavelId, "idx_fichas_responsavel");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssinadoEm)
                .HasColumnType("datetime")
                .HasColumnName("assinado_em");
            entity.Property(e => e.AssinaturaHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("assinatura_hash");
            entity.Property(e => e.AssinaturaUrl)
                .HasMaxLength(500)
                .HasColumnName("assinatura_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.FuncionarioId).HasColumnName("funcionario_id");
            entity.Property(e => e.ObraId).HasColumnName("obra_id");
            entity.Property(e => e.Observacao)
                .HasColumnType("text")
                .HasColumnName("observacao");
            entity.Property(e => e.ResponsavelId).HasColumnName("responsavel_id");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Funcionario).WithMany(p => p.Fichas)
                .HasForeignKey(d => d.FuncionarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fichas_funcionario");

            entity.HasOne(d => d.Obra).WithMany(p => p.Fichas)
                .HasForeignKey(d => d.ObraId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_fichas_obra");

            entity.HasOne(d => d.Responsavel).WithMany(p => p.Fichas)
                .HasForeignKey(d => d.ResponsavelId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_fichas_responsavel");
        });

        modelBuilder.Entity<Fornecedores>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("fornecedores");

            entity.HasIndex(e => e.Nome, "idx_fornecedores_nome");

            entity.HasIndex(e => new { e.DocumentoHash, e.AtivoKey }, "uq_fornecedores_documento").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ativo)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("ativo");
            entity.Property(e => e.AtivoKey)
                .HasMaxLength(6)
                .HasComputedColumnSql("ifnull(`deleted_at`,'1970-01-01 00:00:00')", true)
                .HasColumnName("ativo_key");
            entity.Property(e => e.Cidade)
                .HasMaxLength(120)
                .HasColumnName("cidade");
            entity.Property(e => e.Contato)
                .HasMaxLength(160)
                .HasColumnName("contato");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasMaxLength(6)
                .HasColumnName("deleted_at");
            entity.Property(e => e.DocumentoCifrado)
                .HasMaxLength(512)
                .HasColumnName("documento_cifrado");
            entity.Property(e => e.DocumentoHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("documento_hash");
            entity.Property(e => e.DocumentoMascara)
                .HasMaxLength(20)
                .HasColumnName("documento_mascara");
            entity.Property(e => e.Email)
                .HasMaxLength(160)
                .HasColumnName("email");
            entity.Property(e => e.Estado)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("estado");
            entity.Property(e => e.Nome)
                .HasMaxLength(160)
                .HasColumnName("nome");
            entity.Property(e => e.Observacoes)
                .HasColumnType("text")
                .HasColumnName("observacoes");
            entity.Property(e => e.Telefone)
                .HasMaxLength(30)
                .HasColumnName("telefone");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<FuncionarioCapacitacoes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("funcionario_capacitacoes");

            entity.HasIndex(e => e.FuncionarioId, "idx_capacitacoes_funcionario");

            entity.HasIndex(e => e.TipoId, "idx_capacitacoes_tipo");

            entity.HasIndex(e => e.DataValidade, "idx_capacitacoes_validade");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnexoUrl)
                .HasMaxLength(500)
                .HasColumnName("anexo_url");
            entity.Property(e => e.CargaHoraria).HasColumnName("carga_horaria");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DataEmissao).HasColumnName("data_emissao");
            entity.Property(e => e.DataValidade).HasColumnName("data_validade");
            entity.Property(e => e.FuncionarioId).HasColumnName("funcionario_id");
            entity.Property(e => e.Instrutor)
                .HasMaxLength(160)
                .HasColumnName("instrutor");
            entity.Property(e => e.Modalidade)
                .HasMaxLength(20)
                .HasColumnName("modalidade");
            entity.Property(e => e.NumeroCertificado)
                .HasMaxLength(80)
                .HasColumnName("numero_certificado");
            entity.Property(e => e.Observacao)
                .HasMaxLength(500)
                .HasColumnName("observacao");
            entity.Property(e => e.TipoId).HasColumnName("tipo_id");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Funcionario).WithMany(p => p.FuncionarioCapacitacoes)
                .HasForeignKey(d => d.FuncionarioId)
                .HasConstraintName("fk_capacitacoes_funcionario");

            entity.HasOne(d => d.Tipo).WithMany(p => p.FuncionarioCapacitacoes)
                .HasForeignKey(d => d.TipoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_capacitacoes_tipo");
        });

        modelBuilder.Entity<FuncionarioEquipes>(entity =>
        {
            entity.HasKey(e => new { e.FuncionarioId, e.EquipeId, e.DataInicio })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity.ToTable("funcionario_equipes");

            entity.HasIndex(e => e.EquipeId, "idx_funcionario_equipes_equipe");

            entity.Property(e => e.FuncionarioId).HasColumnName("funcionario_id");
            entity.Property(e => e.EquipeId).HasColumnName("equipe_id");
            entity.Property(e => e.DataInicio).HasColumnName("data_inicio");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DataFim).HasColumnName("data_fim");

            entity.HasOne(d => d.Equipe).WithMany(p => p.FuncionarioEquipes)
                .HasForeignKey(d => d.EquipeId)
                .HasConstraintName("fk_funcionario_equipes_equipe");

            entity.HasOne(d => d.Funcionario).WithMany(p => p.FuncionarioEquipes)
                .HasForeignKey(d => d.FuncionarioId)
                .HasConstraintName("fk_funcionario_equipes_funcionario");
        });

        modelBuilder.Entity<Funcionarios>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("funcionarios");

            entity.HasIndex(e => e.Ativo, "idx_funcionarios_ativo");

            entity.HasIndex(e => e.DeletedAt, "idx_funcionarios_deleted");

            entity.HasIndex(e => e.FuncaoId, "idx_funcionarios_funcao");

            entity.HasIndex(e => e.RegimeId, "idx_funcionarios_regime");

            entity.HasIndex(e => new { e.DocumentoHash, e.AtivoKey }, "uq_funcionarios_documento").IsUnique();

            entity.HasIndex(e => new { e.Matricula, e.AtivoKey }, "uq_funcionarios_matricula").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdicionalPercentual)
                .HasPrecision(5, 2)
                .HasColumnName("adicional_percentual");
            entity.Property(e => e.AnonimizadoEm)
                .HasColumnType("datetime")
                .HasColumnName("anonimizado_em");
            entity.Property(e => e.Ativo)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("ativo");
            entity.Property(e => e.AtivoKey)
                .HasMaxLength(6)
                .HasComputedColumnSql("ifnull(`deleted_at`,'1970-01-01 00:00:00')", true)
                .HasColumnName("ativo_key");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DataAdmissao).HasColumnName("data_admissao");
            entity.Property(e => e.DataDemissao).HasColumnName("data_demissao");
            entity.Property(e => e.DeletedAt)
                .HasMaxLength(6)
                .HasColumnName("deleted_at");
            entity.Property(e => e.DocumentoCifrado)
                .HasMaxLength(512)
                .HasColumnName("documento_cifrado");
            entity.Property(e => e.DocumentoHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("documento_hash");
            entity.Property(e => e.DocumentoMascara)
                .HasMaxLength(20)
                .HasColumnName("documento_mascara");
            entity.Property(e => e.FuncaoId).HasColumnName("funcao_id");
            entity.Property(e => e.Matricula)
                .HasMaxLength(30)
                .HasColumnName("matricula");
            entity.Property(e => e.Nome)
                .HasMaxLength(160)
                .HasColumnName("nome");
            entity.Property(e => e.RegimeId).HasColumnName("regime_id");
            entity.Property(e => e.Telefone)
                .HasMaxLength(30)
                .HasColumnName("telefone");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Valor)
                .HasPrecision(15, 2)
                .HasColumnName("valor");

            entity.HasOne(d => d.Funcao).WithMany(p => p.Funcionarios)
                .HasForeignKey(d => d.FuncaoId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_funcionarios_funcao");

            entity.HasOne(d => d.Regime).WithMany(p => p.Funcionarios)
                .HasForeignKey(d => d.RegimeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_funcionarios_regime");
        });

        modelBuilder.Entity<Funcoes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("funcoes");

            entity.HasIndex(e => e.SetorId, "idx_funcoes_setor");

            entity.HasIndex(e => new { e.Nome, e.SetorId }, "uq_funcoes_nome_setor").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Nome)
                .HasMaxLength(120)
                .HasColumnName("nome");
            entity.Property(e => e.SetorId).HasColumnName("setor_id");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Setor).WithMany(p => p.Funcoes)
                .HasForeignKey(d => d.SetorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_funcoes_setor");
        });

        modelBuilder.Entity<Grupos>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("grupos");

            entity.HasIndex(e => e.CategoriaId, "idx_grupos_categoria");

            entity.HasIndex(e => new { e.Nome, e.CategoriaId }, "uq_grupos_nome_categoria").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoriaId).HasColumnName("categoria_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Nome)
                .HasMaxLength(120)
                .HasColumnName("nome");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Grupos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_grupos_categoria");
        });

        modelBuilder.Entity<ItensEstoque>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("itens_estoque");

            entity.HasIndex(e => e.Ativo, "idx_itens_estoque_ativo");

            entity.HasIndex(e => e.DeletedAt, "idx_itens_estoque_deleted");

            entity.HasIndex(e => e.GrupoId, "idx_itens_estoque_grupo");

            entity.HasIndex(e => e.Item, "idx_itens_estoque_item");

            entity.HasIndex(e => new { e.Codigo, e.AtivoKey }, "uq_itens_estoque_codigo").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ativo)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("ativo");
            entity.Property(e => e.AtivoKey)
                .HasMaxLength(6)
                .HasComputedColumnSql("ifnull(`deleted_at`,'1970-01-01 00:00:00')", true)
                .HasColumnName("ativo_key");
            entity.Property(e => e.Ca)
                .HasMaxLength(30)
                .HasColumnName("ca");
            entity.Property(e => e.CaValidade).HasColumnName("ca_validade");
            entity.Property(e => e.Codigo)
                .HasMaxLength(50)
                .HasColumnName("codigo");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasMaxLength(6)
                .HasColumnName("deleted_at");
            entity.Property(e => e.GrupoId).HasColumnName("grupo_id");
            entity.Property(e => e.Item)
                .HasMaxLength(180)
                .HasColumnName("item");
            entity.Property(e => e.Minimo)
                .HasPrecision(15, 3)
                .HasColumnName("minimo");
            entity.Property(e => e.PrecoRef)
                .HasPrecision(15, 2)
                .HasColumnName("preco_ref");
            entity.Property(e => e.Unidade)
                .HasMaxLength(30)
                .HasColumnName("unidade");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Grupo).WithMany(p => p.ItensEstoque)
                .HasForeignKey(d => d.GrupoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_itens_estoque_grupo");
        });

        modelBuilder.Entity<MedicoesEmpreitada>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("medicoes_empreitada");

            entity.HasIndex(e => e.AprovadoPor, "fk_medicoes_aprovador");

            entity.HasIndex(e => e.Data, "idx_medicoes_data");

            entity.HasIndex(e => new { e.ContratoId, e.Numero }, "uq_medicoes_contrato_numero").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AprovadoEm)
                .HasColumnType("datetime")
                .HasColumnName("aprovado_em");
            entity.Property(e => e.AprovadoPor).HasColumnName("aprovado_por");
            entity.Property(e => e.ContratoId).HasColumnName("contrato_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.Numero).HasColumnName("numero");
            entity.Property(e => e.Observacao)
                .HasColumnType("text")
                .HasColumnName("observacao");
            entity.Property(e => e.Percentual)
                .HasPrecision(5, 2)
                .HasColumnName("percentual");
            entity.Property(e => e.Quantidade)
                .HasPrecision(15, 3)
                .HasColumnName("quantidade");
            entity.Property(e => e.Valor)
                .HasPrecision(15, 2)
                .HasColumnName("valor");

            entity.HasOne(d => d.AprovadoPorNavigation).WithMany(p => p.MedicoesEmpreitada)
                .HasForeignKey(d => d.AprovadoPor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_medicoes_aprovador");

            entity.HasOne(d => d.Contrato).WithMany(p => p.MedicoesEmpreitada)
                .HasForeignKey(d => d.ContratoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_medicoes_contrato");
        });

        modelBuilder.Entity<ObraAditivos>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("obra_aditivos");

            entity.HasIndex(e => e.AprovadoPor, "fk_obra_aditivos_aprovador");

            entity.HasIndex(e => e.ObraId, "idx_obra_aditivos_obra");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AprovadoPor).HasColumnName("aprovado_por");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.DiasPrazo).HasColumnName("dias_prazo");
            entity.Property(e => e.Motivo)
                .HasColumnType("text")
                .HasColumnName("motivo");
            entity.Property(e => e.Numero)
                .HasMaxLength(30)
                .HasColumnName("numero");
            entity.Property(e => e.ObraId).HasColumnName("obra_id");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasColumnName("tipo");
            entity.Property(e => e.Valor)
                .HasPrecision(15, 2)
                .HasColumnName("valor");

            entity.HasOne(d => d.AprovadoPorNavigation).WithMany(p => p.ObraAditivos)
                .HasForeignKey(d => d.AprovadoPor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_obra_aditivos_aprovador");

            entity.HasOne(d => d.Obra).WithMany(p => p.ObraAditivos)
                .HasForeignKey(d => d.ObraId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_obra_aditivos_obra");
        });

        modelBuilder.Entity<ObraLinks>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("obra_links");

            entity.HasIndex(e => e.CriadoPor, "fk_obra_links_criador");

            entity.HasIndex(e => e.ObraId, "idx_obra_links_obra");

            entity.HasIndex(e => e.TokenHash, "uq_obra_links_token").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CriadoPor).HasColumnName("criado_por");
            entity.Property(e => e.ExpiraEm)
                .HasColumnType("datetime")
                .HasColumnName("expira_em");
            entity.Property(e => e.ObraId).HasColumnName("obra_id");
            entity.Property(e => e.RevogadoEm)
                .HasColumnType("datetime")
                .HasColumnName("revogado_em");
            entity.Property(e => e.Rotulo)
                .HasMaxLength(120)
                .HasColumnName("rotulo");
            entity.Property(e => e.TokenHash)
                .HasMaxLength(64)
                .IsFixedLength()
                .HasColumnName("token_hash");
            entity.Property(e => e.TotalAcessos).HasColumnName("total_acessos");
            entity.Property(e => e.UltimoAcesso)
                .HasColumnType("datetime")
                .HasColumnName("ultimo_acesso");

            entity.HasOne(d => d.CriadoPorNavigation).WithMany(p => p.ObraLinks)
                .HasForeignKey(d => d.CriadoPor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_obra_links_criador");

            entity.HasOne(d => d.Obra).WithMany(p => p.ObraLinks)
                .HasForeignKey(d => d.ObraId)
                .HasConstraintName("fk_obra_links_obra");
        });

        modelBuilder.Entity<Obras>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("obras");

            entity.HasIndex(e => e.FechadaPor, "fk_obras_fechada_por");

            entity.HasIndex(e => e.ClienteId, "idx_obras_cliente");

            entity.HasIndex(e => new { e.ClienteId, e.Status }, "idx_obras_cliente_status");

            entity.HasIndex(e => e.DeletedAt, "idx_obras_deleted");

            entity.HasIndex(e => e.DataPrevisao, "idx_obras_previsao");

            entity.HasIndex(e => e.ResponsavelId, "idx_obras_responsavel");

            entity.HasIndex(e => e.Status, "idx_obras_status");

            entity.HasIndex(e => new { e.Codigo, e.AtivoKey }, "uq_obras_codigo").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AtivoKey)
                .HasMaxLength(6)
                .HasComputedColumnSql("ifnull(`deleted_at`,'1970-01-01 00:00:00')", true)
                .HasColumnName("ativo_key");
            entity.Property(e => e.Bairro)
                .HasMaxLength(100)
                .HasColumnName("bairro");
            entity.Property(e => e.Cep)
                .HasMaxLength(10)
                .HasColumnName("cep");
            entity.Property(e => e.Cidade)
                .HasMaxLength(120)
                .HasColumnName("cidade");
            entity.Property(e => e.ClienteId).HasColumnName("cliente_id");
            entity.Property(e => e.Cno)
                .HasMaxLength(30)
                .HasColumnName("cno");
            entity.Property(e => e.Codigo)
                .HasMaxLength(50)
                .HasColumnName("codigo");
            entity.Property(e => e.Complemento)
                .HasMaxLength(100)
                .HasColumnName("complemento");
            entity.Property(e => e.ContratoValor)
                .HasPrecision(15, 2)
                .HasColumnName("contrato_valor");
            entity.Property(e => e.CreaRt)
                .HasMaxLength(40)
                .HasColumnName("crea_rt");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DataConclusao).HasColumnName("data_conclusao");
            entity.Property(e => e.DataInicio).HasColumnName("data_inicio");
            entity.Property(e => e.DataPrevisao).HasColumnName("data_previsao");
            entity.Property(e => e.DeletedAt)
                .HasMaxLength(6)
                .HasColumnName("deleted_at");
            entity.Property(e => e.Descricao)
                .HasColumnType("text")
                .HasColumnName("descricao");
            entity.Property(e => e.Endereco)
                .HasMaxLength(255)
                .HasColumnName("endereco");
            entity.Property(e => e.Estado)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("estado");
            entity.Property(e => e.FechadaEm)
                .HasColumnType("datetime")
                .HasColumnName("fechada_em");
            entity.Property(e => e.FechadaPor).HasColumnName("fechada_por");
            entity.Property(e => e.Nome)
                .HasMaxLength(180)
                .HasColumnName("nome");
            entity.Property(e => e.Numero)
                .HasMaxLength(20)
                .HasColumnName("numero");
            entity.Property(e => e.Observacoes)
                .HasColumnType("text")
                .HasColumnName("observacoes");
            entity.Property(e => e.ResponsavelId).HasColumnName("responsavel_id");
            entity.Property(e => e.ResponsavelTecnico)
                .HasMaxLength(160)
                .HasColumnName("responsavel_tecnico");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'planejamento'")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Obras)
                .HasForeignKey(d => d.ClienteId)
                .HasConstraintName("fk_obras_cliente");

            entity.HasOne(d => d.FechadaPorNavigation).WithMany(p => p.Obras)
                .HasForeignKey(d => d.FechadaPor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_obras_fechada_por");

            entity.HasOne(d => d.Responsavel).WithMany(p => p.Obras)
                .HasForeignKey(d => d.ResponsavelId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_obras_responsavel");
        });

        modelBuilder.Entity<Pagamentos>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("pagamentos");

            entity.HasIndex(e => e.RegistradoPor, "fk_pagamentos_registrador");

            entity.HasIndex(e => e.DataPagamento, "idx_pagamentos_data");

            entity.HasIndex(e => e.ContaPagarId, "idx_pagamentos_pagar");

            entity.HasIndex(e => e.ContaReceberId, "idx_pagamentos_receber");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContaPagarId).HasColumnName("conta_pagar_id");
            entity.Property(e => e.ContaReceberId).HasColumnName("conta_receber_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DataPagamento)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("data_pagamento");
            entity.Property(e => e.FormaPagamento)
                .HasMaxLength(30)
                .HasColumnName("forma_pagamento");
            entity.Property(e => e.Observacao)
                .HasMaxLength(500)
                .HasColumnName("observacao");
            entity.Property(e => e.RegistradoPor).HasColumnName("registrado_por");
            entity.Property(e => e.Valor)
                .HasPrecision(15, 2)
                .HasColumnName("valor");

            entity.HasOne(d => d.ContaPagar).WithMany(p => p.Pagamentos)
                .HasForeignKey(d => d.ContaPagarId)
                .HasConstraintName("fk_pagamentos_pagar");

            entity.HasOne(d => d.ContaReceber).WithMany(p => p.Pagamentos)
                .HasForeignKey(d => d.ContaReceberId)
                .HasConstraintName("fk_pagamentos_receber");

            entity.HasOne(d => d.RegistradoPorNavigation).WithMany(p => p.Pagamentos)
                .HasForeignKey(d => d.RegistradoPor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_pagamentos_registrador");
        });

        modelBuilder.Entity<Papeis>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("papeis");

            entity.HasIndex(e => e.Nome, "uq_papeis_nome").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Descricao)
                .HasMaxLength(255)
                .HasColumnName("descricao");
            entity.Property(e => e.Nome)
                .HasMaxLength(60)
                .HasColumnName("nome");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasMany(d => d.Permissao).WithMany(p => p.Papel)
                .UsingEntity<Dictionary<string, object>>(
                    "PapelPermissoes",
                    r => r.HasOne<Permissoes>().WithMany()
                        .HasForeignKey("PermissaoId")
                        .HasConstraintName("fk_papel_permissoes_permissao"),
                    l => l.HasOne<Papeis>().WithMany()
                        .HasForeignKey("PapelId")
                        .HasConstraintName("fk_papel_permissoes_papel"),
                    j =>
                    {
                        j.HasKey("PapelId", "PermissaoId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("papel_permissoes");
                        j.HasIndex(new[] { "PermissaoId" }, "idx_papel_permissoes_permissao");
                        j.IndexerProperty<long>("PapelId").HasColumnName("papel_id");
                        j.IndexerProperty<long>("PermissaoId").HasColumnName("permissao_id");
                    });
        });

        modelBuilder.Entity<ParametrosEncargos>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("parametros_encargos");

            entity.HasIndex(e => new { e.RegimeId, e.VigenciaInicio }, "idx_param_encargos_regime_vig");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Fonte)
                .HasMaxLength(255)
                .HasColumnName("fonte");
            entity.Property(e => e.Observacao)
                .HasColumnType("text")
                .HasColumnName("observacao");
            entity.Property(e => e.Percentual)
                .HasPrecision(6, 2)
                .HasColumnName("percentual");
            entity.Property(e => e.RegimeId).HasColumnName("regime_id");
            entity.Property(e => e.VigenciaFim).HasColumnName("vigencia_fim");
            entity.Property(e => e.VigenciaInicio).HasColumnName("vigencia_inicio");

            entity.HasOne(d => d.Regime).WithMany(p => p.ParametrosEncargos)
                .HasForeignKey(d => d.RegimeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_param_encargos_regime");
        });

        modelBuilder.Entity<Permissoes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("permissoes");

            entity.HasIndex(e => e.Chave, "uq_permissoes_chave").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Chave)
                .HasMaxLength(80)
                .HasColumnName("chave");
            entity.Property(e => e.Descricao)
                .HasMaxLength(255)
                .HasColumnName("descricao");
        });

        modelBuilder.Entity<Ponto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ponto");

            entity.HasIndex(e => e.RegistradoPor, "fk_ponto_registrador");

            entity.HasIndex(e => new { e.FuncionarioId, e.Data }, "idx_ponto_funcionario_data");

            entity.HasIndex(e => new { e.ObraId, e.Data }, "idx_ponto_obra_data");

            entity.HasIndex(e => new { e.FuncionarioId, e.ObraId, e.Data }, "uq_ponto_funcionario_obra_data").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdicionalSnapshot)
                .HasPrecision(5, 2)
                .HasColumnName("adicional_snapshot");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Custo)
                .HasPrecision(15, 2)
                .HasColumnName("custo");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.EncargosSnapshot)
                .HasPrecision(6, 2)
                .HasColumnName("encargos_snapshot");
            entity.Property(e => e.FuncionarioId).HasColumnName("funcionario_id");
            entity.Property(e => e.HoraEntrada)
                .HasColumnType("time")
                .HasColumnName("hora_entrada");
            entity.Property(e => e.HoraSaida)
                .HasColumnType("time")
                .HasColumnName("hora_saida");
            entity.Property(e => e.HoraSaidaIntervalo)
                .HasColumnType("time")
                .HasColumnName("hora_saida_intervalo");
            entity.Property(e => e.HoraVoltaIntervalo)
                .HasColumnType("time")
                .HasColumnName("hora_volta_intervalo");
            entity.Property(e => e.Horas)
                .HasPrecision(6, 2)
                .HasComputedColumnSql("round((greatest((ifnull(time_to_sec(timediff(`hora_saida`,`hora_entrada`)),0) - ifnull(time_to_sec(timediff(`hora_volta_intervalo`,`hora_saida_intervalo`)),0)),0) / 3600),2)", true)
                .HasColumnName("horas");
            entity.Property(e => e.IdExterno)
                .HasMaxLength(80)
                .HasColumnName("id_externo");
            entity.Property(e => e.ObraId).HasColumnName("obra_id");
            entity.Property(e => e.Observacao)
                .HasMaxLength(500)
                .HasColumnName("observacao");
            entity.Property(e => e.Origem)
                .HasMaxLength(20)
                .HasDefaultValueSql("'manual'")
                .HasColumnName("origem");
            entity.Property(e => e.RegistradoPor).HasColumnName("registrado_por");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.ValorHoraSnapshot)
                .HasPrecision(15, 4)
                .HasColumnName("valor_hora_snapshot");

            entity.HasOne(d => d.Funcionario).WithMany(p => p.Ponto)
                .HasForeignKey(d => d.FuncionarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ponto_funcionario");

            entity.HasOne(d => d.Obra).WithMany(p => p.Ponto)
                .HasForeignKey(d => d.ObraId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ponto_obra");

            entity.HasOne(d => d.RegistradoPorNavigation).WithMany(p => p.Ponto)
                .HasForeignKey(d => d.RegistradoPor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_ponto_registrador");
        });

        modelBuilder.Entity<Regimes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("regimes");

            entity.HasIndex(e => e.Rotulo, "uq_regimes_rotulo").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Descricao)
                .HasMaxLength(255)
                .HasColumnName("descricao");
            entity.Property(e => e.HorasMes)
                .HasPrecision(6, 2)
                .HasColumnName("horas_mes");
            entity.Property(e => e.Rotulo)
                .HasMaxLength(80)
                .HasColumnName("rotulo");
            entity.Property(e => e.Unidade)
                .HasMaxLength(40)
                .HasColumnName("unidade");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Setores>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("setores");

            entity.HasIndex(e => e.Nome, "uq_setores_nome").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .HasColumnName("nome");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<SolicitacaoItens>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("solicitacao_itens");

            entity.HasIndex(e => e.ItemId, "idx_solicitacao_itens_item");

            entity.HasIndex(e => e.SolicitacaoId, "idx_solicitacao_itens_solicitacao");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Observacao)
                .HasMaxLength(500)
                .HasColumnName("observacao");
            entity.Property(e => e.QtdAtendida)
                .HasPrecision(15, 3)
                .HasColumnName("qtd_atendida");
            entity.Property(e => e.QtdSolicitada)
                .HasPrecision(15, 3)
                .HasColumnName("qtd_solicitada");
            entity.Property(e => e.SolicitacaoId).HasColumnName("solicitacao_id");
            entity.Property(e => e.Unidade)
                .HasMaxLength(30)
                .HasColumnName("unidade");

            entity.HasOne(d => d.Item).WithMany(p => p.SolicitacaoItens)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_solicitacao_itens_item");

            entity.HasOne(d => d.Solicitacao).WithMany(p => p.SolicitacaoItens)
                .HasForeignKey(d => d.SolicitacaoId)
                .HasConstraintName("fk_solicitacao_itens_solicitacao");
        });

        modelBuilder.Entity<SolicitacoesCompra>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("solicitacoes_compra");

            entity.HasIndex(e => e.DecididoPor, "fk_solicitacoes_decisor");

            entity.HasIndex(e => e.SolicitadoPor, "fk_solicitacoes_solicitante");

            entity.HasIndex(e => e.Data, "idx_solicitacoes_data");

            entity.HasIndex(e => new { e.ObraId, e.Status }, "idx_solicitacoes_obra_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.DataNecessidade).HasColumnName("data_necessidade");
            entity.Property(e => e.DecididoEm)
                .HasColumnType("datetime")
                .HasColumnName("decidido_em");
            entity.Property(e => e.DecididoPor).HasColumnName("decidido_por");
            entity.Property(e => e.MotivoRecusa)
                .HasMaxLength(500)
                .HasColumnName("motivo_recusa");
            entity.Property(e => e.ObraId).HasColumnName("obra_id");
            entity.Property(e => e.Observacao)
                .HasColumnType("text")
                .HasColumnName("observacao");
            entity.Property(e => e.SolicitadoPor).HasColumnName("solicitado_por");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'pendente'")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.DecididoPorNavigation).WithMany(p => p.SolicitacoesCompraDecididoPorNavigation)
                .HasForeignKey(d => d.DecididoPor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_solicitacoes_decisor");

            entity.HasOne(d => d.Obra).WithMany(p => p.SolicitacoesCompra)
                .HasForeignKey(d => d.ObraId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_solicitacoes_obra");

            entity.HasOne(d => d.SolicitadoPorNavigation).WithMany(p => p.SolicitacoesCompraSolicitadoPorNavigation)
                .HasForeignKey(d => d.SolicitadoPor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_solicitacoes_solicitante");
        });

        modelBuilder.Entity<TiposCapacitacao>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tipos_capacitacao");

            entity.HasIndex(e => e.Codigo, "uq_tipos_capacitacao_codigo").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CargaHorariaMin).HasColumnName("carga_horaria_min");
            entity.Property(e => e.Codigo)
                .HasMaxLength(30)
                .HasColumnName("codigo");
            entity.Property(e => e.ExigePresencial).HasColumnName("exige_presencial");
            entity.Property(e => e.Nome)
                .HasMaxLength(160)
                .HasColumnName("nome");
            entity.Property(e => e.Observacao)
                .HasMaxLength(255)
                .HasColumnName("observacao");
            entity.Property(e => e.ValidadeMeses).HasColumnName("validade_meses");
        });

        modelBuilder.Entity<UsuarioObras>(entity =>
        {
            entity.HasKey(e => new { e.UsuarioId, e.ObraId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("usuario_obras");

            entity.HasIndex(e => e.ObraId, "idx_usuario_obras_obra");

            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
            entity.Property(e => e.ObraId).HasColumnName("obra_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Obra).WithMany(p => p.UsuarioObras)
                .HasForeignKey(d => d.ObraId)
                .HasConstraintName("fk_usuario_obras_obra");

            entity.HasOne(d => d.Usuario).WithMany(p => p.UsuarioObras)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("fk_usuario_obras_usuario");
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("usuarios");

            entity.HasIndex(e => e.Ativo, "idx_usuarios_ativo");

            entity.HasIndex(e => e.FuncionarioId, "idx_usuarios_funcionario");

            entity.HasIndex(e => e.PapelId, "idx_usuarios_papel");

            entity.HasIndex(e => e.Email, "uq_usuarios_email").IsUnique();

            entity.HasIndex(e => e.Usuario, "uq_usuarios_usuario").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ativo)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("ativo");
            entity.Property(e => e.BloqueadoAte)
                .HasColumnType("datetime")
                .HasColumnName("bloqueado_ate");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(160)
                .HasColumnName("email");
            entity.Property(e => e.FuncionarioId).HasColumnName("funcionario_id");
            entity.Property(e => e.Nome)
                .HasMaxLength(160)
                .HasColumnName("nome");
            entity.Property(e => e.PapelId).HasColumnName("papel_id");
            entity.Property(e => e.SenhaHash)
                .HasMaxLength(255)
                .HasColumnName("senha_hash");
            entity.Property(e => e.TentativasFalhas).HasColumnName("tentativas_falhas");
            entity.Property(e => e.UltimoLogin)
                .HasColumnType("datetime")
                .HasColumnName("ultimo_login");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Usuario)
                .HasMaxLength(80)
                .HasColumnName("usuario");

            entity.HasOne(d => d.Funcionario).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.FuncionarioId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_usuarios_funcionario");

            entity.HasOne(d => d.Papel).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.PapelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usuarios_papel");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
