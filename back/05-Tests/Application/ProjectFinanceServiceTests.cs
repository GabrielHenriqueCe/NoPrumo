using NoPrumo.Application.Results;
using NoPrumo.Application.Services;
using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;
using NoPrumo.Tests.Fakes;

namespace NoPrumo.Tests.Application;

public class ProjectFinanceServiceTests
{
    private static readonly DateOnly Monday = new(2026, 8, 3);

    private readonly FakeProjectFinanceRepository repository = new();
    private readonly ProjectFinanceService service;

    public ProjectFinanceServiceTests()
    {
        service = new ProjectFinanceService(repository, new FixedTimeProvider(new DateTime(2026, 10, 12)));

        repository.Projects.Add(new Project
        {
            Id = 2,
            Code = "OB-2026-01",
            Name = "Residencial Aurora",
            Status = ProjectStatus.InProgress,
            ContractAmount = 1_000_000m,
            Amendments = [new ProjectAmendment { Amount = 100_000m }],
            Stages = [new Stage { Percentage = 40m }, new Stage { Percentage = 0m }],
        });
    }

    [Fact]
    public async Task GetAsync_UnknownProject_ReturnsNull()
    {
        Assert.Null(await service.GetAsync(99, CancellationToken.None));
    }

    [Fact]
    public async Task GetAsync_SumsMaterialAndLaborAgainstTheRevisedContract()
    {
        repository.MaterialByDay.AddRange([new DailyAmount(Monday, 30_000m), new DailyAmount(Monday.AddDays(2), 25_000m)]);
        repository.LaborByDay.AddRange([new DailyAmount(Monday, 1_000m), new DailyAmount(Monday.AddDays(1), 1_200m)]);

        var financials = await service.GetAsync(2, CancellationToken.None);

        Assert.NotNull(financials);
        Assert.Equal(100_000m, financials.AmendmentsAmount);
        Assert.Equal(1_100_000m, financials.RevisedContractAmount);
        Assert.Equal(55_000m, financials.MaterialCost);
        Assert.Equal(2_200m, financials.LaborCost);
        Assert.Equal(57_200m, financials.TotalCost);
        Assert.Equal(5.2m, financials.SpentPercentage);
        Assert.Equal(20m, financials.ProgressPercentage);
        Assert.False(financials.CostAheadOfProgress);
        Assert.Equal(1_042_800m, financials.Margin);
    }

    [Fact]
    public async Task GetAsync_CostByDay_MergesBothSourcesInOrderAndAccumulates()
    {
        repository.MaterialByDay.AddRange([new DailyAmount(Monday.AddDays(2), 500m), new DailyAmount(Monday, 100m)]);
        repository.LaborByDay.AddRange([new DailyAmount(Monday, 50m), new DailyAmount(Monday.AddDays(1), 70m)]);

        var costByDay = (await service.GetAsync(2, CancellationToken.None))!.CostByDay;

        Assert.Equal([Monday, Monday.AddDays(1), Monday.AddDays(2)], costByDay.Select(day => day.Date));
        Assert.Equal([150m, 220m, 720m], costByDay.Select(day => day.CumulativeCost));
        Assert.Equal(0m, costByDay[1].Material);
        Assert.Equal(0m, costByDay[2].Labor);
    }

    [Fact]
    public async Task GetAsync_WithoutAnyCost_HasAnEmptySeries()
    {
        var financials = await service.GetAsync(2, CancellationToken.None);

        Assert.Empty(financials!.CostByDay);
        Assert.Equal(0m, financials.TotalCost);
    }

    [Fact]
    public async Task ListClosingAsync_ProjectWithoutCost_ShowsZeroInsteadOfMissing()
    {
        repository.Projects.Add(new Project { Id = 3, Code = "OB-2026-02", Name = "Galpão", ContractAmount = 500_000m });
        repository.MaterialByProject[2] = 300_000m;
        repository.LaborByProject[2] = 200_000m;

        var rows = await service.ListClosingAsync(CancellationToken.None);

        var aurora = rows.Single(row => row.ProjectId == 2);
        Assert.Equal(500_000m, aurora.TotalCost);
        Assert.Equal(600_000m, aurora.Margin);
        Assert.True(aurora.CostAheadOfProgress);

        var warehouse = rows.Single(row => row.ProjectId == 3);
        Assert.Equal(0m, warehouse.TotalCost);
        Assert.Equal(500_000m, warehouse.Margin);
    }
}
