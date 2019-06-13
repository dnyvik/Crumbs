using System.Text.RegularExpressions;

namespace Crumbs.History
{
    internal static class StringExtensions
    {
        public static string SplitCamelCase(this string value)
        {
            return Regex.Replace(value, "(\\B[A-Z])", " $1");
        }
    }
}
