namespace NoPrumo.Application.DTOs;

public class EmployeeDto
{
    public long Id { get; set; }
    public string? RegistrationNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public long? JobRoleId { get; set; }
    public string? JobRoleName { get; set; }
    public long? EmploymentRegimeId { get; set; }
    public string? EmploymentRegimeLabel { get; set; }
    
    // Ajustado para DateOnly?
    public DateOnly? HireDate { get; set; }
    
    public string? Phone { get; set; }
    public string? DocumentMasked { get; set; }
    public bool Active { get; set; }
}

public class EmployeeFinancialDto : EmployeeDto
{
    public decimal PayRate { get; set; }
    public decimal AdditionalPercentage { get; set; }
}

public class CreateEmployeeRequest
{
    public string? RegistrationNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public long? JobRoleId { get; set; }
    public long? EmploymentRegimeId { get; set; }
    public decimal PayRate { get; set; }
    public decimal AdditionalPercentage { get; set; }
    
    // Ajustado para DateOnly?
    public DateOnly? HireDate { get; set; }
    
    public string? Phone { get; set; }
    public string? Document { get; set; }
}

public class UpdateEmployeeRequest : CreateEmployeeRequest
{
    // Adicionamos Active explicitamente aqui para garantir o cast
    public bool Active { get; set; }
}