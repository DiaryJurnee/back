using System.Text.RegularExpressions;

namespace API.Modules.Route.Filter
{
    public class KebabCaseParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            return value == null ? null : value.ToString()!.ToKebabCase();
        }
    }
    public static partial class StringExtension
    {
        [GeneratedRegex("([a-z0-9])([A-Z])", RegexOptions.Compiled)]
        private static partial Regex KebabCaseRule();

        public static string ToKebabCase(this string input)
            => KebabCaseRule().Replace(input, "$1-$2").ToLower().Trim('-');
    }
}