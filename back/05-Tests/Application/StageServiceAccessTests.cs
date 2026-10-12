using NoPrumo.Application.Access;
using NoPrumo.Application.Services;
using NoPrumo.Domain.Entities;
using NoPrumo.Tests.Fakes;

namespace NoPrumo.Tests.Application;

// O mestre (sem view_all_projects) só enxerga etapas das obras que ele supervisiona.
public class StageServiceAccessTests
{
    private const long ForemanEmployeeId = 1;

    private readonly FakeStageRepository stageRepository = new();
    private readonly FakeProjectRepository projectRepository = new();
    private readonly StageService service;

    public StageServiceAccessTests()
    {
        service = new StageService(stageRepository, projectRepository, new FixedTimeProvider(new DateTime(2026, 10, 12)));

        var his = new Project { Id = 2, Name = "Residencial Aurora", SupervisorId = ForemanEmployeeId };
        var other = new Project { Id = 3, Name = "Galpão Itoupava", SupervisorId = 99 };
        projectRepository.Projects.AddRange([his, other]);

        stageRepository.Stages.AddRange(
        [
            new Stage { Id = 10, ProjectId = 2, Project = his, Name = "Fundação" },
            new Stage { Id = 20, ProjectId = 3, Project = other, Name = "Estrutura" },
        ]);
    }

    private static ProjectAccess Foreman(long? employeeId = ForemanEmployeeId) => new(false, employeeId);

    [Fact]
    public async Task ListAsync_ForemanOnHisProject_SeesTheStages()
    {
        var page = await service.ListAsync(1, 10, null, 2, Foreman(), CancellationToken.None);

        Assert.Equal("Fundação", Assert.Single(page.Items).Name);
    }

    [Fact]
    public async Task ListAsync_ForemanOnAnotherProject_SeesNothing()
    {
        var page = await service.ListAsync(1, 10, null, 3, Foreman(), CancellationToken.None);

        Assert.Empty(page.Items);
        Assert.Equal(0, page.Total);
    }

    [Fact]
    public async Task ListAsync_ForemanWithoutProject_DoesNotGetTheGeneralList()
    {
        var page = await service.ListAsync(1, 10, null, null, Foreman(), CancellationToken.None);

        Assert.Empty(page.Items);
    }

    [Fact]
    public async Task ListAsync_UserNotLinkedToAnEmployee_SeesNothing()
    {
        var page = await service.ListAsync(1, 10, null, 2, Foreman(employeeId: null), CancellationToken.None);

        Assert.Empty(page.Items);
    }

    [Fact]
    public async Task ListAsync_SeesAllProjects_GetsEveryStage()
    {
        var page = await service.ListAsync(1, 10, null, null, ProjectAccess.AllProjects, CancellationToken.None);

        Assert.Equal(2, page.Items.Count);
    }

    [Fact]
    public async Task GetByIdAsync_StageOfAnotherProject_IsNotFoundForTheForeman()
    {
        Assert.Null(await service.GetByIdAsync(20, Foreman(), CancellationToken.None));
        Assert.NotNull(await service.GetByIdAsync(10, Foreman(), CancellationToken.None));
    }
}
