using NoPrumo.Application.DTOs;
using NoPrumo.Application.Services;
using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;
using NoPrumo.Tests.Fakes;

namespace NoPrumo.Tests.Application;

public class DashboardServiceTests
{
    private static readonly DateOnly Today = new(2026, 10, 12);

    private readonly FakeDashboardRepository repository = new();
    private readonly FakeStockMovementService stockMovementService = new();
    private readonly DashboardService service;

    public DashboardServiceTests()
    {
        service = new DashboardService(repository, stockMovementService, new FixedTimeProvider(Today.ToDateTime(TimeOnly.MinValue)));
    }

    [Fact]
    public async Task GetAsync_WithoutPermissions_EveryCardIsNull()
    {
        var dashboard = await service.GetAsync(new HashSet<string>(), CancellationToken.None);

        Assert.Equal(new DashboardDto(null, null, null, null, null, null), dashboard);
    }

    [Fact]
    public async Task GetAsync_Projects_CountsOnlyUnderwayAndTheirLateStages()
    {
        repository.Projects.AddRange(
        [
            new Project
            {
                Status = ProjectStatus.InProgress,
                ForecastDate = Today.AddDays(-1),
                Stages =
                [
                    new Stage { PlannedDate = Today.AddDays(-3), Status = StageStatus.InProgress },
                    new Stage { PlannedDate = Today.AddDays(-3), Status = StageStatus.Completed },
                ],
            },
            new Project { Status = ProjectStatus.Planning, ForecastDate = Today.AddDays(60) },
            new Project
            {
                Status = ProjectStatus.Completed,
                ForecastDate = Today.AddDays(-90),
                Stages = [new Stage { PlannedDate = Today.AddDays(-90), Status = StageStatus.InProgress }],
            },
        ]);

        var dashboard = await service.GetAsync(new HashSet<string> { "view_projects" }, CancellationToken.None);

        Assert.Equal(2, dashboard.ActiveProjects);
        Assert.Equal(1, dashboard.LateProjects);
        Assert.Equal(1, dashboard.LateStages);
        Assert.Null(dashboard.StockItemsBelowMinimum);
        Assert.Null(dashboard.TrainingsNeedingAttention);
    }

    [Fact]
    public async Task GetAsync_Stock_CountsItemsBelowMinimum()
    {
        stockMovementService.BelowMinimum.Add(new StockBelowMinimumDto(2, "Cimento", "saco", 50m, 10m));

        var dashboard = await service.GetAsync(new HashSet<string> { "view_stock" }, CancellationToken.None);

        Assert.Equal(1, dashboard.StockItemsBelowMinimum);
        Assert.Null(dashboard.ActiveProjects);
    }

    [Fact]
    public async Task GetAsync_Safety_CountsOnlyTheLatestCertificateOfEachType()
    {
        repository.Employees.Add(new Employee
        {
            EmployeeTrainings =
            [
                // NR-35 vencido, mas renovado: não conta.
                new EmployeeTraining { Id = 1, TrainingTypeId = 1, IssueDate = new DateOnly(2024, 1, 1), ExpiryDate = Today.AddDays(-10) },
                new EmployeeTraining { Id = 2, TrainingTypeId = 1, IssueDate = new DateOnly(2026, 9, 1), ExpiryDate = Today.AddYears(2) },
                // NR-18 vencendo em 20 dias: conta.
                new EmployeeTraining { Id = 3, TrainingTypeId = 2, IssueDate = new DateOnly(2025, 1, 1), ExpiryDate = Today.AddDays(20) },
            ],
        });
        repository.UnsignedPpeIssues = 3;

        var dashboard = await service.GetAsync(new HashSet<string> { "manage_safety" }, CancellationToken.None);

        Assert.Equal(1, dashboard.TrainingsNeedingAttention);
        Assert.Equal(3, dashboard.UnsignedPpeIssues);
    }
}
