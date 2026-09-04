using System.Security.Cryptography;
using System.Text;

namespace SchoolAccount.Application.Extensions;

public static class StringExtensions
{
    public static Guid AsGuid(this string input)
    {
        var bytes = Encoding.ASCII.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        return new Guid(hash.AsSpan(0, 16));
    }
}
