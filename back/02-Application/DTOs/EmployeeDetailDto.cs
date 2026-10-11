using System;
using System.Collections.Generic;

namespace NoPrumo.Application.DTOs;

public record EmployeeDetailDto
{
    public long Id { get; init; }
    public string? RegistrationNumber { get; init; }
    public string Name { get; init; } = string.Empty;
    public long? JobRoleId { get; init; }
    public string? JobRoleName { get; init; }
    public long? EmploymentRegimeId { get; init; }
    public string? EmploymentRegimeLabel { get; init; }
    public DateOnly? HireDate { get; init; }
    public string? Phone { get; init; }
    public string? DocumentMasked { get; init; }
    public bool Active { get; init; }

    // --- Dados exclusivos da Ficha Completa ---
    public string? CurrentTeamName { get; init; }
    public DateOnly? TeamAllocationDate { get; init; }
    public IReadOnlyList<ProjectDto> CurrentProjects { get; init; } = Array.Empty<ProjectDto>();
    public IReadOnlyList<EmployeeTrainingDetailDto> Trainings { get; init; } = Array.Empty<EmployeeTrainingDetailDto>();
}