using System.Text.Json.Serialization;

namespace ElGuerre.Taskin.Application.Errors;

public sealed record Error(
    string Code,
    string Description,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    IReadOnlyCollection<object>? Values = null)
{
    public static readonly Error None = new(String.Empty, String.Empty);
    public static readonly Error NullValue = new("NULL_VALUE", "Null value was provided");
}