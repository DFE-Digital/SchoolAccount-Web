namespace SchoolAccount.SharedKernel.Authentication;

public static class OrganisationExtensions
{
    public static bool HasEstablishments(this Organisation organisation)
    {
        return organisation.Category.Value
            is OrganisationCategory.LocalAuthority
                or OrganisationCategory.MultiAcademyTrust
                or OrganisationCategory.SingleAcademyTrust;
    }

    public static bool IsTrust(this Organisation organisation)
    {
        return organisation.Category.Value
            is OrganisationCategory.MultiAcademyTrust
                or OrganisationCategory.SingleAcademyTrust;
    }
}
