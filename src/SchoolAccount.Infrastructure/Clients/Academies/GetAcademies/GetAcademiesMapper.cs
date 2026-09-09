using System.Collections.ObjectModel;
using GovUK.Dfe.AcademiesApi.Client.Contracts;
using SchoolAccount.Application.Features.Academies.GetAcademies;

namespace SchoolAccount.Infrastructure.Clients.Academies.GetAcademies;

public static class GetAcademiesMapper
{
    public static GetAcademyTrustResponse ToTrustResponse(
        TrustDto trust,
        ObservableCollection<EstablishmentDto> trustEstablishments
    )
    {
        return new GetAcademyTrustResponse
        {
            Name =
                trust.Name
                ?? throw new InvalidOperationException(
                    $"Trust {trust.Name} returned without a name"
                ),
            Ukprn =
                trust.Ukprn
                ?? throw new InvalidOperationException("Trust returned without a UKPRN"),
            Type = ToNameAndCodeResponse(trust.Type),
            GroupUid = trust.GroupUid,
            Establishments = trustEstablishments.Select(ToEstablishmentResponse).ToList(),
        };
    }

    public static GetAcademyEstablishmentResponse ToEstablishmentResponse(
        EstablishmentDto establishment
    )
    {
        return new GetAcademyEstablishmentResponse
        {
            Ukprn = establishment.Ukprn ?? string.Empty,
            EstablishmentName = establishment.Name ?? string.Empty,
            Urn = establishment.Urn ?? string.Empty,
            EstablishmentNumber = establishment.EstablishmentNumber ?? string.Empty,
            LocalAuthorityCode = establishment.LocalAuthorityCode ?? string.Empty,
            LocalAuthorityName = establishment.LocalAuthorityName ?? string.Empty,
            EstablishmentType = ToNameAndCodeResponse(establishment.EstablishmentType),
            EstablishmentGroupType = ToNameAndCodeResponse(establishment.EstablishmentGroupType),
            PhaseOfEducation = ToNameAndCodeResponse(establishment.PhaseOfEducation),
        };
    }

    private static GetAcademyNameAndCodeResponse? ToNameAndCodeResponse(NameAndCodeDto? nameAndCode)
    {
        return nameAndCode is null
            ? null
            : new GetAcademyNameAndCodeResponse
            {
                Name = nameAndCode.Name ?? string.Empty,
                Code = nameAndCode.Code ?? string.Empty,
            };
    }
}
