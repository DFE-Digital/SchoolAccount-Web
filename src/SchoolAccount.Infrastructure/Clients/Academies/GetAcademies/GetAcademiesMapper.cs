using GovUK.Dfe.AcademiesApi.Client.Contracts;
using SchoolAccount.Application.Features.Academies.GetAcademies;

namespace SchoolAccount.Infrastructure.Clients.Academies.GetAcademies;

public static class GetAcademiesMapper
{
    public static GetAcademyTrustResponse ToTrustResponse(
        TrustDto trust,
        IReadOnlyList<GetAcademyEstablishmentResponse> trustEstablishments
    )
    {
        ArgumentException.ThrowIfNullOrEmpty(trust.Name);
        ArgumentException.ThrowIfNullOrEmpty(trust.Ukprn);

        return new GetAcademyTrustResponse
        {
            Name = trust.Name,
            Ukprn = trust.Ukprn,
            Type = ToNameAndCodeResponse(trust.Type),
            GroupUid = trust.GroupUid,
            Establishments = trustEstablishments,
        };
    }

    public static GetAcademyEstablishmentResponse ToEstablishmentResponse(
        EstablishmentDto establishment
    )
    {
        ArgumentException.ThrowIfNullOrEmpty(establishment.Name);
        ArgumentException.ThrowIfNullOrEmpty(establishment.Ukprn);

        return new GetAcademyEstablishmentResponse
        {
            Ukprn = establishment.Ukprn,
            EstablishmentName = establishment.Name,
            Urn = establishment.Urn,
            EstablishmentNumber = establishment.EstablishmentNumber,
            LocalAuthorityCode = establishment.LocalAuthorityCode,
            LocalAuthorityName = establishment.LocalAuthorityName,
            EstablishmentType = ToNameAndCodeResponse(establishment.EstablishmentType),
            EstablishmentGroupType = ToNameAndCodeResponse(establishment.EstablishmentGroupType),
            PhaseOfEducation = ToNameAndCodeResponse(establishment.PhaseOfEducation),
        };
    }

    private static GetAcademyNameAndCodeResponse? ToNameAndCodeResponse(NameAndCodeDto? nameAndCode)
    {
        return nameAndCode is null || string.IsNullOrEmpty(nameAndCode.Name)
            ? null
            : new GetAcademyNameAndCodeResponse
            {
                Name = nameAndCode.Name,
                Code = nameAndCode.Code,
            };
    }
}
