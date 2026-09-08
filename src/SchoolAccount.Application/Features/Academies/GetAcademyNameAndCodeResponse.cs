namespace SchoolAccount.Application.Features.Academies;

public class GetAcademyNameAndCodeResponse
{
    public required string Name { get; init; } = string.Empty;

    public string Code { get; init; } = string.Empty;
}
