namespace NoPrumo.Application.DTOs;

public class JobRoleDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long DepartmentId { get; set; }
    public bool Active { get; set; }
}

public class CreateJobRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public long DepartmentId { get; set; }
}

public class UpdateJobRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public long DepartmentId { get; set; }
    public bool Active { get; set; }
}