namespace api.Contracts.Responses;
public record EmployeeResponse(string Id, string Name, string Email, string Phone, bool Gender, string? photoUrl);