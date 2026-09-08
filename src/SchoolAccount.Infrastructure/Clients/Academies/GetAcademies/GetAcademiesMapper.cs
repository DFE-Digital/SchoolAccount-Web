using System.Collections.ObjectModel;
using GovUK.Dfe.AcademiesApi.Client.Contracts;
using SchoolAccount.Application.Features.Academies;

namespace SchoolAccount.Infrastructure.Clients.Academies.GetAcademies;

public class GetAcademiesMapper
{
    public static GetAcademyTrustResponse ToTrustResponse(
        TrustDto trust,
        ObservableCollection<EstablishmentDto> trustEstablishments
    ) =>
        new()
        {
            Name = trust.Name,
            Ukprn = trust.Ukprn,
            Type = ToNameAndCodeResponse(trust.Type),
            GroupUid = trust.GroupUid,
            Establishments = trustEstablishments.Select(establishment =>
                ToEstablishmentResponse(establishment)
            ),
        };

    public static GetAcademyEstablishmentResponse ToEstablishmentResponse(
        EstablishmentDto establishment
    ) =>
        new()
        {
            Urn = establishment.Urn,
            Ukprn = establishment.Ukprn,
            EstablishmentNumber = establishment.EstablishmentNumber,
            EstablishmentName = establishment.Name,
            LocalAuthorityCode = establishment.LocalAuthorityCode,
            LocalAuthorityName = establishment.LocalAuthorityName,
            EstablishmentType = ToNameAndCodeResponse(establishment.EstablishmentType),
            EstablishmentGroupType = ToNameAndCodeResponse(establishment.EstablishmentGroupType),
            PhaseOfEducation = ToNameAndCodeResponse(establishment.PhaseOfEducation),
        };

    private static GetAcademyNameAndCodeResponse ToNameAndCodeResponse(
        NameAndCodeDto nameAndCode
    ) => new() { Name = nameAndCode.Name, Code = nameAndCode.Code };
}
