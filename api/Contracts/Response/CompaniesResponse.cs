namespace api.Contracts.Responses;

public record GetCompaniesResponse(
    string Id,
    string Name,
    string Description,
    int EmployeeCount,
    string Location,
    string? PhotoUrl);
public record GetCompanyResponse(
    string Id,
    string Name,
    string Description,
    string Location,
    string? PhotoUrl);