using System.Text.RegularExpressions;

namespace api_integration.Presenter.API.src.RouteTransformer
{
     public partial class SpinCaseTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            var input = value?.ToString();
            if (string.IsNullOrEmpty(input)) return null;

            // Convert to spin-case
            return MyRegex().Replace(input, "$1-$2").ToLowerInvariant();
        }

        [GeneratedRegex("([a-z])([A-Z])")]
        private static partial Regex MyRegex();
    }
}