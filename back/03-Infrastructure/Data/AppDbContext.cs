using Microsoft.EntityFrameworkCore;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Infrastructure.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointment { get; set; }

    public virtual DbSet<AuditLog> AuditLog { get; set; }

    public virtual DbSet<StockCategory> StockCategory { get; set; }

    public virtual DbSet<Client> Client { get; set; }

    public virtual DbSet<AccountPayable> AccountPayable { get; set; }

    public virtual DbSet<AccountReceivable> AccountReceivable { get; set; }

    public virtual DbSet<Subcontract> Subcontract { get; set; }

    public virtual DbSet<TeamProject> TeamProject { get; set; }

    public virtual DbSet<Team> Team { get; set; }

    public virtual DbSet<StockMovement> StockMovement { get; set; }

    public virtual DbSet<Stage> Stage { get; set; }

    public virtual DbSet<PpeIssueItem> PpeIssueItem { get; set; }

    public virtual DbSet<PpeIssue> PpeIssue { get; set; }

    public virtual DbSet<Supplier> Supplier { get; set; }

    public virtual DbSet<EmployeeTraining> EmployeeTraining { get; set; }

    public virtual DbSet<EmployeeTeam> EmployeeTeam { get; set; }

    public virtual DbSet<Employee> Employee { get; set; }

    public virtual DbSet<JobRole> JobRole { get; set; }

    public virtual DbSet<StockGroup> StockGroup { get; set; }

    public virtual DbSet<StockItem> StockItem { get; set; }

    public virtual DbSet<SubcontractMeasurement> SubcontractMeasurement { get; set; }

    public virtual DbSet<ProjectAmendment> ProjectAmendment { get; set; }

    public virtual DbSet<ProjectLink> ProjectLink { get; set; }

    public virtual DbSet<Project> Project { get; set; }

    public virtual DbSet<Payment> Payment { get; set; }

    public virtual DbSet<Role> Role { get; set; }

    public virtual DbSet<PayrollChargeRate> PayrollChargeRate { get; set; }

    public virtual DbSet<Permission> Permission { get; set; }

    public virtual DbSet<TimeEntry> TimeEntry { get; set; }

    public virtual DbSet<EmploymentRegime> EmploymentRegime { get; set; }

    public virtual DbSet<Department> Department { get; set; }

    public virtual DbSet<PurchaseRequestItem> PurchaseRequestItem { get; set; }

    public virtual DbSet<PurchaseRequest> PurchaseRequest { get; set; }

    public virtual DbSet<TrainingType> TrainingType { get; set; }

    public virtual DbSet<UserProject> UserProject { get; set; }

    public virtual DbSet<User> User { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => new { e.Date, e.Time });

            entity.HasIndex(e => e.ProjectId);

            entity.HasIndex(e => e.AssignedToId);

            entity.Property(e => e.CompletedAt)
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasColumnType("text");
            entity.Property(e => e.Time)
                .HasColumnType("time");
            entity.Property(e => e.Title)
                .HasMaxLength(220);
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasDefaultValueSql("'project'");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Project).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_appointment_project");

            entity.HasOne(d => d.AssignedTo).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_appointment_assigned_to");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.CreatedAt);

            entity.HasIndex(e => new { e.TableName, e.RecordId });

            entity.HasIndex(e => new { e.UserId, e.CreatedAt });

            entity.Property(e => e.Action)
                .HasMaxLength(20);
            entity.Property(e => e.ChangedFields)
                .HasColumnType("json");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.OldData)
                .HasColumnType("json");
            entity.Property(e => e.NewData)
                .HasColumnType("json");
            entity.Property(e => e.Ip)
                .HasMaxLength(45);
            entity.Property(e => e.TableName)
                .HasMaxLength(100);
            entity.Property(e => e.UserAgent)
                .HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_audit_log_user");
        });

        modelBuilder.Entity<StockCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.Name).IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(60);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.City);

            entity.HasIndex(e => e.DeletedAt);

            entity.HasIndex(e => e.Name);

            entity.HasIndex(e => new { e.DocumentHash, e.ActiveKey }).IsUnique();

            entity.Property(e => e.ActiveKey)
                .HasMaxLength(6)
                .HasComputedColumnSql("ifnull(`deleted_at`,'1970-01-01 00:00:00')", true);
            entity.Property(e => e.District)
                .HasMaxLength(100);
            entity.Property(e => e.Mobile)
                .HasMaxLength(30);
            entity.Property(e => e.PostalCode)
                .HasMaxLength(10);
            entity.Property(e => e.City)
                .HasMaxLength(120);
            entity.Property(e => e.Complement)
                .HasMaxLength(100);
            entity.Property(e => e.ContactName)
                .HasMaxLength(160);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedAt)
                .HasMaxLength(6);
            entity.Property(e => e.DocumentEncrypted)
                .HasMaxLength(512);
            entity.Property(e => e.DocumentHash)
                .HasMaxLength(64)
                .IsFixedLength();
            entity.Property(e => e.DocumentMasked)
                .HasMaxLength(20);
            entity.Property(e => e.Email)
                .HasMaxLength(160);
            entity.Property(e => e.Street)
                .HasMaxLength(255);
            entity.Property(e => e.State)
                .HasMaxLength(2)
                .IsFixedLength();
            entity.Property(e => e.Name)
                .HasMaxLength(160);
            entity.Property(e => e.Number)
                .HasMaxLength(20);
            entity.Property(e => e.Notes)
                .HasColumnType("text");
            entity.Property(e => e.Phone)
                .HasMaxLength(30);
            entity.Property(e => e.PersonType)
                .HasMaxLength(10)
                .HasDefaultValueSql("'company'");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<AccountPayable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.SupplierId);

            entity.HasIndex(e => e.ProjectId);

            entity.HasIndex(e => new { e.SourceType, e.SourceId });

            entity.HasIndex(e => new { e.DueDate, e.Status });

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(255);
            entity.Property(e => e.InvoiceNumber)
                .HasMaxLength(40);
            entity.Property(e => e.Notes)
                .HasColumnType("text");
            entity.Property(e => e.SourceType)
                .HasMaxLength(30);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'pending'");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Amount)
                .HasPrecision(15, 2);
            entity.Property(e => e.PaidAmount)
                .HasPrecision(15, 2);

            entity.HasOne(d => d.Supplier).WithMany(p => p.AccountsPayable)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("fk_account_payable_supplier");

            entity.HasOne(d => d.Project).WithMany(p => p.AccountsPayable)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("fk_account_payable_project");
        });

        modelBuilder.Entity<AccountReceivable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.ClientId);

            entity.HasIndex(e => e.ProjectId);

            entity.HasIndex(e => new { e.DueDate, e.Status });

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(255);
            entity.Property(e => e.InvoiceNumber)
                .HasMaxLength(40);
            entity.Property(e => e.Notes)
                .HasColumnType("text");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'pending'");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Amount)
                .HasPrecision(15, 2);
            entity.Property(e => e.PaidAmount)
                .HasPrecision(15, 2);

            entity.HasOne(d => d.Client).WithMany(p => p.AccountsReceivable)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("fk_account_receivable_client");

            entity.HasOne(d => d.Project).WithMany(p => p.AccountsReceivable)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("fk_account_receivable_project");
        });

        modelBuilder.Entity<Subcontract>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.StageId);

            entity.HasIndex(e => e.SupplierId);

            entity.HasIndex(e => e.ProjectId);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(255);
            entity.Property(e => e.Notes)
                .HasColumnType("text");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(15, 2);
            entity.Property(e => e.PlannedQuantity)
                .HasPrecision(15, 3);
            entity.Property(e => e.InssRetentionPct)
                .HasPrecision(5, 2);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'active'");
            entity.Property(e => e.PriceType)
                .HasMaxLength(20)
                .HasDefaultValueSql("'global'");
            entity.Property(e => e.Unit)
                .HasMaxLength(30);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(15, 2);

            entity.HasOne(d => d.Stage).WithMany(p => p.Subcontracts)
                .HasForeignKey(d => d.StageId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_subcontract_stage");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Subcontracts)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_subcontract_supplier");

            entity.HasOne(d => d.Project).WithMany(p => p.Subcontracts)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_subcontract_project");
        });

        modelBuilder.Entity<TeamProject>(entity =>
        {
            entity.HasKey(e => new { e.TeamId, e.ProjectId, e.StartDate })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity.HasIndex(e => e.ProjectId);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Team).WithMany(p => p.TeamProjects)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("fk_team_project_team");

            entity.HasOne(d => d.Project).WithMany(p => p.TeamProjects)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("fk_team_project_project");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.DepartmentId);

            entity.HasIndex(e => new { e.Name, e.DepartmentId, e.ActiveKey }).IsUnique();

            entity.Property(e => e.ActiveKey)
                .HasMaxLength(6)
                .HasComputedColumnSql("ifnull(`deleted_at`,'1970-01-01 00:00:00')", true);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedAt)
                .HasMaxLength(6);
            entity.Property(e => e.Name)
                .HasMaxLength(120);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Department).WithMany(p => p.Teams)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_team_department");
        });

        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.SupplierId);

            entity.HasIndex(e => e.RecordedBy);

            entity.HasIndex(e => new { e.EmployeeId, e.Date });

            entity.HasIndex(e => new { e.StockItemId, e.Date });

            entity.HasIndex(e => new { e.StockItemId, e.ProjectId });

            entity.HasIndex(e => new { e.ProjectId, e.Date });

            entity.HasIndex(e => new { e.SourceType, e.SourceId });

            entity.HasIndex(e => e.TransferId);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.UnitCost)
                .HasPrecision(15, 2);
            entity.Property(e => e.Date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.InvoiceNumber)
                .HasMaxLength(40);
            entity.Property(e => e.Notes)
                .HasMaxLength(500);
            entity.Property(e => e.SourceType)
                .HasMaxLength(30);
            entity.Property(e => e.Quantity)
                .HasPrecision(15, 3);
            entity.Property(e => e.Type)
                .HasMaxLength(25);
            entity.Property(e => e.Unit)
                .HasMaxLength(30);

            entity.HasOne(d => d.Supplier).WithMany(p => p.StockMovements)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_stock_movement_supplier");

            entity.HasOne(d => d.Employee).WithMany(p => p.StockMovements)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("fk_stock_movement_employee");

            entity.HasOne(d => d.StockItem).WithMany(p => p.StockMovements)
                .HasForeignKey(d => d.StockItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_stock_movement_item");

            entity.HasOne(d => d.Project).WithMany(p => p.StockMovements)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("fk_stock_movement_project");

            entity.HasOne(d => d.RecordedByUser).WithMany(p => p.StockMovements)
                .HasForeignKey(d => d.RecordedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_stock_movement_recorder");
        });

        modelBuilder.Entity<Stage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.MarkedBy);

            entity.HasIndex(e => e.PlannedDate);

            entity.HasIndex(e => e.TeamId);

            entity.HasIndex(e => e.ProjectId);

            entity.HasIndex(e => new { e.ProjectId, e.Status });

            entity.HasIndex(e => e.SupervisorId);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.MarkedAt)
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(180);
            entity.Property(e => e.Notes)
                .HasColumnType("text");
            entity.Property(e => e.Percentage)
                .HasPrecision(5, 2);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'planned'");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Team).WithMany(p => p.Stages)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_stage_team");

            entity.HasOne(d => d.MarkedByUser).WithMany(p => p.Stages)
                .HasForeignKey(d => d.MarkedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_stage_marked_by");

            entity.HasOne(d => d.Project).WithMany(p => p.Stages)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_stage_project");

            entity.HasOne(d => d.Supervisor).WithMany(p => p.Stages)
                .HasForeignKey(d => d.SupervisorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_stage_supervisor");
        });

        modelBuilder.Entity<PpeIssueItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.PpeIssueId);

            entity.HasIndex(e => e.StockItemId);

            entity.HasIndex(e => e.Status);

            entity.Property(e => e.CaSnapshot)
                .HasMaxLength(30);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.ItemNameSnapshot)
                .HasMaxLength(180);
            entity.Property(e => e.Notes)
                .HasMaxLength(500);
            entity.Property(e => e.Quantity)
                .HasPrecision(15, 3)
                .HasDefaultValueSql("'1.000'");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'issued'");
            entity.Property(e => e.Unit)
                .HasMaxLength(30);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.PpeIssue).WithMany(p => p.Items)
                .HasForeignKey(d => d.PpeIssueId)
                .HasConstraintName("fk_ppe_issue_item_issue");

            entity.HasOne(d => d.StockItem).WithMany(p => p.PpeIssueItems)
                .HasForeignKey(d => d.StockItemId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_ppe_issue_item_item");
        });

        modelBuilder.Entity<PpeIssue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => new { e.EmployeeId, e.Date });

            entity.HasIndex(e => e.ProjectId);

            entity.HasIndex(e => e.IssuedById);

            entity.Property(e => e.SignedAt)
                .HasColumnType("datetime");
            entity.Property(e => e.SignatureHash)
                .HasMaxLength(64)
                .IsFixedLength();
            entity.Property(e => e.SignatureUrl)
                .HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Notes)
                .HasColumnType("text");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.PpeIssues)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ppe_issue_employee");

            entity.HasOne(d => d.Project).WithMany(p => p.PpeIssues)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_ppe_issue_project");

            entity.HasOne(d => d.IssuedBy).WithMany(p => p.PpeIssues)
                .HasForeignKey(d => d.IssuedById)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_ppe_issue_issued_by");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.Name);

            entity.HasIndex(e => new { e.DocumentHash, e.ActiveKey }).IsUnique();

            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.ActiveKey)
                .HasMaxLength(6)
                .HasComputedColumnSql("ifnull(`deleted_at`,'1970-01-01 00:00:00')", true);
            entity.Property(e => e.City)
                .HasMaxLength(120);
            entity.Property(e => e.ContactName)
                .HasMaxLength(160);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedAt)
                .HasMaxLength(6);
            entity.Property(e => e.DocumentEncrypted)
                .HasMaxLength(512);
            entity.Property(e => e.DocumentHash)
                .HasMaxLength(64)
                .IsFixedLength();
            entity.Property(e => e.DocumentMasked)
                .HasMaxLength(20);
            entity.Property(e => e.Email)
                .HasMaxLength(160);
            entity.Property(e => e.State)
                .HasMaxLength(2)
                .IsFixedLength();
            entity.Property(e => e.Name)
                .HasMaxLength(160);
            entity.Property(e => e.Notes)
                .HasColumnType("text");
            entity.Property(e => e.Phone)
                .HasMaxLength(30);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeTraining>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.EmployeeId);

            entity.HasIndex(e => e.TrainingTypeId);

            entity.HasIndex(e => e.ExpiryDate);

            entity.Property(e => e.AttachmentUrl)
                .HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Instructor)
                .HasMaxLength(160);
            entity.Property(e => e.Modality)
                .HasMaxLength(20);
            entity.Property(e => e.CertificateNumber)
                .HasMaxLength(80);
            entity.Property(e => e.Notes)
                .HasMaxLength(500);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeTrainings)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("fk_employee_training_employee");

            entity.HasOne(d => d.TrainingType).WithMany(p => p.EmployeeTrainings)
                .HasForeignKey(d => d.TrainingTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_employee_training_type");
        });

        modelBuilder.Entity<EmployeeTeam>(entity =>
        {
            entity.HasKey(e => new { e.EmployeeId, e.TeamId, e.StartDate })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity.HasIndex(e => e.TeamId);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Team).WithMany(p => p.EmployeeTeams)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("fk_employee_team_team");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeTeams)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("fk_employee_team_employee");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.Active);

            entity.HasIndex(e => e.DeletedAt);

            entity.HasIndex(e => e.JobRoleId);

            entity.HasIndex(e => e.EmploymentRegimeId);

            entity.HasIndex(e => new { e.DocumentHash, e.ActiveKey }).IsUnique();

            entity.HasIndex(e => new { e.RegistrationNumber, e.ActiveKey }).IsUnique();

            entity.Property(e => e.AdditionalPercentage)
                .HasPrecision(5, 2);
            entity.Property(e => e.AnonymizedAt)
                .HasColumnType("datetime");
            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.ActiveKey)
                .HasMaxLength(6)
                .HasComputedColumnSql("ifnull(`deleted_at`,'1970-01-01 00:00:00')", true);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedAt)
                .HasMaxLength(6);
            entity.Property(e => e.DocumentEncrypted)
                .HasMaxLength(512);
            entity.Property(e => e.DocumentHash)
                .HasMaxLength(64)
                .IsFixedLength();
            entity.Property(e => e.DocumentMasked)
                .HasMaxLength(20);
            entity.Property(e => e.RegistrationNumber)
                .HasMaxLength(30);
            entity.Property(e => e.Name)
                .HasMaxLength(160);
            entity.Property(e => e.Phone)
                .HasMaxLength(30);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.PayRate)
                .HasPrecision(15, 2);

            entity.HasOne(d => d.JobRole).WithMany(p => p.Employees)
                .HasForeignKey(d => d.JobRoleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_employee_job_role");

            entity.HasOne(d => d.EmploymentRegime).WithMany(p => p.Employees)
                .HasForeignKey(d => d.EmploymentRegimeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_employee_regime");
        });

        modelBuilder.Entity<JobRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.DepartmentId);

            entity.HasIndex(e => new { e.Name, e.DepartmentId }).IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(120);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Department).WithMany(p => p.JobRoles)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_job_role_department");
        });

        modelBuilder.Entity<StockGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.StockCategoryId);

            entity.HasIndex(e => new { e.Name, e.StockCategoryId }).IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(120);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.StockCategory).WithMany(p => p.StockGroups)
                .HasForeignKey(d => d.StockCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_stock_group_category");
        });

        modelBuilder.Entity<StockItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.Active);

            entity.HasIndex(e => e.DeletedAt);

            entity.HasIndex(e => e.StockGroupId);

            entity.HasIndex(e => e.Name);

            entity.HasIndex(e => new { e.Code, e.ActiveKey }).IsUnique();

            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.ActiveKey)
                .HasMaxLength(6)
                .HasComputedColumnSql("ifnull(`deleted_at`,'1970-01-01 00:00:00')", true);
            entity.Property(e => e.Ca)
                .HasMaxLength(30);
            entity.Property(e => e.Code)
                .HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedAt)
                .HasMaxLength(6);
            entity.Property(e => e.Name)
                .HasMaxLength(180);
            entity.Property(e => e.MinQuantity)
                .HasPrecision(15, 3);
            entity.Property(e => e.ReferencePrice)
                .HasPrecision(15, 2);
            entity.Property(e => e.Unit)
                .HasMaxLength(30);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.StockGroup).WithMany(p => p.StockItems)
                .HasForeignKey(d => d.StockGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_stock_item_group");
        });

        modelBuilder.Entity<SubcontractMeasurement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.ApprovedBy);

            entity.HasIndex(e => e.Date);

            entity.HasIndex(e => new { e.SubcontractId, e.Number }).IsUnique();

            entity.Property(e => e.ApprovedAt)
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Notes)
                .HasColumnType("text");
            entity.Property(e => e.Percentage)
                .HasPrecision(5, 2);
            entity.Property(e => e.Quantity)
                .HasPrecision(15, 3);
            entity.Property(e => e.Amount)
                .HasPrecision(15, 2);

            entity.HasOne(d => d.ApprovedByUser).WithMany(p => p.SubcontractMeasurements)
                .HasForeignKey(d => d.ApprovedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_subcontract_measurement_approver");

            entity.HasOne(d => d.Subcontract).WithMany(p => p.Measurements)
                .HasForeignKey(d => d.SubcontractId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_subcontract_measurement_subcontract");
        });

        modelBuilder.Entity<ProjectAmendment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.ApprovedBy);

            entity.HasIndex(e => e.ProjectId);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Reason)
                .HasColumnType("text");
            entity.Property(e => e.Number)
                .HasMaxLength(30);
            entity.Property(e => e.Type)
                .HasMaxLength(20);
            entity.Property(e => e.Amount)
                .HasPrecision(15, 2);

            entity.HasOne(d => d.ApprovedByUser).WithMany(p => p.ProjectAmendments)
                .HasForeignKey(d => d.ApprovedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_project_amendment_approver");

            entity.HasOne(d => d.Project).WithMany(p => p.Amendments)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_project_amendment_project");
        });

        modelBuilder.Entity<ProjectLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.CreatedBy);

            entity.HasIndex(e => e.ProjectId);

            entity.HasIndex(e => e.TokenHash).IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("datetime");
            entity.Property(e => e.RevokedAt)
                .HasColumnType("datetime");
            entity.Property(e => e.Label)
                .HasMaxLength(120);
            entity.Property(e => e.TokenHash)
                .HasMaxLength(64)
                .IsFixedLength();
            entity.Property(e => e.LastAccessAt)
                .HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.ProjectLinks)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_project_link_creator");

            entity.HasOne(d => d.Project).WithMany(p => p.Links)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("fk_project_link_project");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.ClosedBy);

            entity.HasIndex(e => e.ClientId);

            entity.HasIndex(e => new { e.ClientId, e.Status });

            entity.HasIndex(e => e.DeletedAt);

            entity.HasIndex(e => e.ForecastDate);

            entity.HasIndex(e => e.SupervisorId);

            entity.HasIndex(e => e.Status);

            entity.HasIndex(e => new { e.Code, e.ActiveKey }).IsUnique();

            entity.Property(e => e.ActiveKey)
                .HasMaxLength(6)
                .HasComputedColumnSql("ifnull(`deleted_at`,'1970-01-01 00:00:00')", true);
            entity.Property(e => e.District)
                .HasMaxLength(100);
            entity.Property(e => e.PostalCode)
                .HasMaxLength(10);
            entity.Property(e => e.City)
                .HasMaxLength(120);
            entity.Property(e => e.Cno)
                .HasMaxLength(30);
            entity.Property(e => e.Code)
                .HasMaxLength(50);
            entity.Property(e => e.Complement)
                .HasMaxLength(100);
            entity.Property(e => e.ContractAmount)
                .HasPrecision(15, 2);
            entity.Property(e => e.CreaRt)
                .HasMaxLength(40);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedAt)
                .HasMaxLength(6);
            entity.Property(e => e.Description)
                .HasColumnType("text");
            entity.Property(e => e.Street)
                .HasMaxLength(255);
            entity.Property(e => e.State)
                .HasMaxLength(2)
                .IsFixedLength();
            entity.Property(e => e.ClosedAt)
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(180);
            entity.Property(e => e.Number)
                .HasMaxLength(20);
            entity.Property(e => e.Notes)
                .HasColumnType("text");
            entity.Property(e => e.TechnicalManager)
                .HasMaxLength(160);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'planning'");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Client).WithMany(p => p.Projects)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("fk_project_client");

            entity.HasOne(d => d.ClosedByUser).WithMany(p => p.Projects)
                .HasForeignKey(d => d.ClosedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_project_closed_by");

            entity.HasOne(d => d.Supervisor).WithMany(p => p.Projects)
                .HasForeignKey(d => d.SupervisorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_project_supervisor");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.RecordedBy);

            entity.HasIndex(e => e.PaymentDate);

            entity.HasIndex(e => e.AccountPayableId);

            entity.HasIndex(e => e.AccountReceivableId);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(30);
            entity.Property(e => e.Notes)
                .HasMaxLength(500);
            entity.Property(e => e.Amount)
                .HasPrecision(15, 2);

            entity.HasOne(d => d.AccountPayable).WithMany(p => p.Payments)
                .HasForeignKey(d => d.AccountPayableId)
                .HasConstraintName("fk_payment_payable");

            entity.HasOne(d => d.AccountReceivable).WithMany(p => p.Payments)
                .HasForeignKey(d => d.AccountReceivableId)
                .HasConstraintName("fk_payment_receivable");

            entity.HasOne(d => d.RecordedByUser).WithMany(p => p.Payments)
                .HasForeignKey(d => d.RecordedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_payment_recorder");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.Name).IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(255);
            entity.Property(e => e.Name)
                .HasMaxLength(60);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    r => r.HasOne<Permission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .HasConstraintName("fk_role_permission_permission"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("fk_role_permission_role"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.HasIndex(new[] { "PermissionId" });
                    });
        });

        modelBuilder.Entity<PayrollChargeRate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => new { e.EmploymentRegimeId, e.EffectiveStart });

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Source)
                .HasMaxLength(255);
            entity.Property(e => e.Notes)
                .HasColumnType("text");
            entity.Property(e => e.Percentage)
                .HasPrecision(6, 2);

            entity.HasOne(d => d.EmploymentRegime).WithMany(p => p.PayrollChargeRates)
                .HasForeignKey(d => d.EmploymentRegimeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_payroll_charge_rate_regime");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.Code).IsUnique();

            entity.Property(e => e.Code)
                .HasMaxLength(80);
            entity.Property(e => e.Description)
                .HasMaxLength(255);
        });

        modelBuilder.Entity<TimeEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.RecordedBy);

            entity.HasIndex(e => new { e.EmployeeId, e.Date });

            entity.HasIndex(e => new { e.ProjectId, e.Date });

            entity.HasIndex(e => new { e.EmployeeId, e.ProjectId, e.Date }).IsUnique();

            entity.Property(e => e.AdditionalSnapshot)
                .HasPrecision(5, 2);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Cost)
                .HasPrecision(15, 2);
            entity.Property(e => e.ChargesSnapshot)
                .HasPrecision(6, 2);
            entity.Property(e => e.ClockIn)
                .HasColumnType("time");
            entity.Property(e => e.ClockOut)
                .HasColumnType("time");
            entity.Property(e => e.BreakStart)
                .HasColumnType("time");
            entity.Property(e => e.BreakEnd)
                .HasColumnType("time");
            entity.Property(e => e.Hours)
                .HasPrecision(6, 2)
                .HasComputedColumnSql("round((greatest((ifnull(time_to_sec(timediff(`clock_out`,`clock_in`)),0) - ifnull(time_to_sec(timediff(`break_end`,`break_start`)),0)),0) / 3600),2)", true);
            entity.Property(e => e.ExternalId)
                .HasMaxLength(80);
            entity.Property(e => e.Notes)
                .HasMaxLength(500);
            entity.Property(e => e.Source)
                .HasMaxLength(20)
                .HasDefaultValueSql("'manual'");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.HourlyRateSnapshot)
                .HasPrecision(15, 4);

            entity.HasOne(d => d.Employee).WithMany(p => p.TimeEntries)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_time_entry_employee");

            entity.HasOne(d => d.Project).WithMany(p => p.TimeEntries)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_time_entry_project");

            entity.HasOne(d => d.RecordedByUser).WithMany(p => p.TimeEntries)
                .HasForeignKey(d => d.RecordedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_time_entry_recorder");
        });

        modelBuilder.Entity<EmploymentRegime>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.Label).IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(255);
            entity.Property(e => e.MonthlyHours)
                .HasPrecision(6, 2);
            entity.Property(e => e.Label)
                .HasMaxLength(80);
            entity.Property(e => e.Unit)
                .HasMaxLength(40);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.Name).IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(100);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<PurchaseRequestItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.StockItemId);

            entity.HasIndex(e => e.PurchaseRequestId);

            entity.Property(e => e.Notes)
                .HasMaxLength(500);
            entity.Property(e => e.FulfilledQuantity)
                .HasPrecision(15, 3);
            entity.Property(e => e.RequestedQuantity)
                .HasPrecision(15, 3);
            entity.Property(e => e.Unit)
                .HasMaxLength(30);

            entity.HasOne(d => d.StockItem).WithMany(p => p.PurchaseRequestItems)
                .HasForeignKey(d => d.StockItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_purchase_request_item_item");

            entity.HasOne(d => d.PurchaseRequest).WithMany(p => p.Items)
                .HasForeignKey(d => d.PurchaseRequestId)
                .HasConstraintName("fk_purchase_request_item_request");
        });

        modelBuilder.Entity<PurchaseRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.DecidedBy);

            entity.HasIndex(e => e.RequestedBy);

            entity.HasIndex(e => e.Date);

            entity.HasIndex(e => new { e.ProjectId, e.Status });

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.DecidedAt)
                .HasColumnType("datetime");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500);
            entity.Property(e => e.Notes)
                .HasColumnType("text");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'pending'");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.DecidedByUser).WithMany(p => p.PurchaseRequestsDecided)
                .HasForeignKey(d => d.DecidedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_purchase_request_decider");

            entity.HasOne(d => d.Project).WithMany(p => p.PurchaseRequests)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_purchase_request_project");

            entity.HasOne(d => d.RequestedByUser).WithMany(p => p.PurchaseRequestsRequested)
                .HasForeignKey(d => d.RequestedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_purchase_request_requester");
        });

        modelBuilder.Entity<TrainingType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.Code).IsUnique();

            entity.Property(e => e.Code)
                .HasMaxLength(30);
            entity.Property(e => e.Name)
                .HasMaxLength(160);
            entity.Property(e => e.Notes)
                .HasMaxLength(255);
        });

        modelBuilder.Entity<UserProject>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ProjectId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.HasIndex(e => e.ProjectId);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Project).WithMany(p => p.UserProjects)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("fk_user_project_project");

            entity.HasOne(d => d.User).WithMany(p => p.UserProjects)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_project_user");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.Active);

            entity.HasIndex(e => e.EmployeeId);

            entity.HasIndex(e => e.RoleId);

            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasIndex(e => e.Username).IsUnique();

            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.MustChangePassword)
                .HasDefaultValue(false);
            entity.Property(e => e.LockedUntil)
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(160);
            entity.Property(e => e.Name)
                .HasMaxLength(160);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255);
            entity.Property(e => e.LastLoginAt)
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Username)
                .HasMaxLength(80);

            entity.HasOne(d => d.Employee).WithMany(p => p.Users)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_user_employee");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
