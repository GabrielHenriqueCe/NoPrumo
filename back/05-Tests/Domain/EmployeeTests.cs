using NoPrumo.Domain.Entities;

namespace NoPrumo.Tests.Domain;

public class EmployeeTests
{
    private const long Nr35 = 1;
    private const long Nr18 = 2;

    [Fact]
    public void LatestTrainings_KeepsOnlyTheNewestOfEachType()
    {
        var renewed = new EmployeeTraining { Id = 2, TrainingTypeId = Nr35, IssueDate = new DateOnly(2026, 6, 1) };
        var employee = new Employee
        {
            EmployeeTrainings =
            [
                new EmployeeTraining { Id = 1, TrainingTypeId = Nr35, IssueDate = new DateOnly(2024, 6, 1) },
                renewed,
                new EmployeeTraining { Id = 3, TrainingTypeId = Nr18, IssueDate = new DateOnly(2025, 1, 10) },
            ],
        };

        var latest = employee.LatestTrainings().ToList();

        Assert.Equal(2, latest.Count);
        Assert.Contains(renewed, latest);
    }

    [Fact]
    public void LatestTrainings_SameIssueDate_TheLastRecordedWins()
    {
        var employee = new Employee
        {
            EmployeeTrainings =
            [
                new EmployeeTraining { Id = 7, TrainingTypeId = Nr35, IssueDate = new DateOnly(2026, 6, 1) },
                new EmployeeTraining { Id = 9, TrainingTypeId = Nr35, IssueDate = new DateOnly(2026, 6, 1) },
            ],
        };

        Assert.Equal(9, employee.LatestTrainings().Single().Id);
    }

    [Theory]
    [InlineData("Valdir Kühn")]
    [InlineData("valdir kuhn")]
    [InlineData("  VALDIR   KÜHN ")]
    public void IsNamed_IgnoresCaseAccentsAndExtraSpaces(string typedName)
    {
        Assert.True(new Employee { Name = "Valdir Kühn" }.IsNamed(typedName));
    }

    [Fact]
    public void IsNamed_AnotherName_IsFalse()
    {
        Assert.False(new Employee { Name = "Valdir Kühn" }.IsNamed("Valdir"));
    }
}
