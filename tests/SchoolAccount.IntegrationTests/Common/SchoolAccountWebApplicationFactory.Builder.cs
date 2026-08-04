namespace SchoolAccount.IntegrationTests.Common;

public partial class SchoolAccountWebApplicationFactory
{
    public sealed class Builder
    {
        internal bool UseAuthentication { get; private set; }
        internal bool UseNonAuthenticatedUser { get; private set; }

        public Builder WithAuthentication()
        {
            UseAuthentication = true;

            return this;
        }

        public Builder WithNonAuthenticatedUser()
        {
            UseNonAuthenticatedUser = true;

            return this;
        }

        public SchoolAccountWebApplicationFactory Build()
        {
            return new SchoolAccountWebApplicationFactory(this);
        }
    }
}
