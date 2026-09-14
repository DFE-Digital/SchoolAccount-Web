namespace SchoolAccount.SharedKernel.Authentication;

public static class OrganisationExtensions
{
    public static bool HasEstablishments(this Organisation organisation) =>
        organisation.Category.Value
            is OrganisationCategory.LocalAuthority
                or OrganisationCategory.MultiAcademyTrust
                or OrganisationCategory.SingleAcademyTrust;
}
