using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace SchoolAccount.MockIdentityProvider;

/// <summary>
/// Signs ID tokens with an RSA key generated at startup. Restarting the
/// provider invalidates nothing in the web app, because the app only checks
/// the token once, when the user signs in.
/// </summary>
internal sealed class IdTokenIssuer : IDisposable
{
    private static readonly TimeSpan _lifetime = TimeSpan.FromHours(1);

    private readonly RSA _rsa = RSA.Create(2048);
    private readonly RsaSecurityKey _signingKey;
    private readonly MockIdentityProviderSettings _settings;
    private readonly TimeProvider _timeProvider;

    public IdTokenIssuer(IOptions<MockIdentityProviderSettings> settings, TimeProvider timeProvider)
    {
        _settings = settings.Value;
        _timeProvider = timeProvider;
        _signingKey = new RsaSecurityKey(_rsa) { KeyId = Guid.NewGuid().ToString("N") };
    }

    public JsonWebKey PublicKey
    {
        get
        {
            var publicOnly = new RsaSecurityKey(
                _rsa.ExportParameters(includePrivateParameters: false)
            )
            {
                KeyId = _signingKey.KeyId,
            };
            var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(publicOnly);
            jwk.Use = "sig";
            jwk.Alg = SecurityAlgorithms.RsaSha256;
            return jwk;
        }
    }

    public string Issue(TestUser user, string clientId, string nonce)
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _settings.Issuer,
            Audience = clientId,
            IssuedAt = now,
            NotBefore = now,
            Expires = now.Add(_lifetime),
            SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.RsaSha256),
            Claims = new Dictionary<string, object>
            {
                ["sub"] = user.Sub.ToString(),
                ["nonce"] = nonce,
                ["given_name"] = user.GivenName,
                ["family_name"] = user.FamilyName,
                ["email"] = user.Email,
                ["organisation"] = user.Organisation,
            },
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }

    public void Dispose() => _rsa.Dispose();
}
