namespace SchoolAccount.Application.Features.Academies.GetAcademies;

public class GetAcademyNameAndCodeResponse
{
    public required string Name { get; init; } = string.Empty;

    public string Code { get; init; } = string.Empty;
}
