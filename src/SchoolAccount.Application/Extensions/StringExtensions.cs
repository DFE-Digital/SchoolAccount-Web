using System.Text;

namespace SchoolAccount.Application.Extensions;

public static class StringExtensions
{
    public static Guid AsGuid(this string input)
    {
        return new Guid(
            System.Security.Cryptography.SHA256.HashData(Encoding.ASCII.GetBytes(input))
        );
    }
}
