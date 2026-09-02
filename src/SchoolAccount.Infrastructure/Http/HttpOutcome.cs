using SchoolAccount.SharedKernel;

namespace SchoolAccount.Infrastructure.Http;

public readonly struct HttpOutcome<T>(Result<T> result, HttpCallContext context)
    : IEquatable<HttpOutcome<T>>
{
    public Result<T> Result { get; } = result;
    public HttpCallContext Context { get; } = context;

    public override bool Equals(object? obj) => Context.Equals(obj);

    public override int GetHashCode() => Context.GetHashCode();

    public static bool operator ==(HttpOutcome<T> left, HttpOutcome<T> right) => left.Equals(right);

    public static bool operator !=(HttpOutcome<T> left, HttpOutcome<T> right) => !(left == right);

    public bool Equals(HttpOutcome<T> other) =>
        Result.Equals(other.Result) && Context.Equals(other.Context);
}
