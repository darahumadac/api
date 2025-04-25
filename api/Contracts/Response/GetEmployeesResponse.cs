namespace api.Contracts.Responses;
public record GetEmployeesResponse(
    string Id,
    string Name,
    string Email,
    string Phone,
    int Gender,
    string? PhotoUrl,
    int DaysWorked,
    string? CompanyName);

public record GetEmployeeByIdResponse(
    string Id,
    string Name,
    string Email,
    string Phone,
    int Gender,
    string? PhotoUrl,
    string? CompanyId);