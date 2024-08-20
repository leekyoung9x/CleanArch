using System.Text.RegularExpressions;

namespace CleanArch.Infrastructure.Validator
{
    public class SqlInjectionValidator
    {
        // Regex pattern to identify SQL injection attempts
        private static readonly Regex SqlInjectionPattern = new Regex(
            @"(''|--|/\*|\*/|;|DROP\s+TABLE|UPDATE\s+SET|INSERT\s+INTO|SELECT\s+FROM|UNION\s+ALL|CREATE\s+TABLE|ALTER\s+TABLE|TRUNCATE\s+TABLE)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Validate input string to check for SQL injection patterns.
        /// </summary>
        /// <param name="input">The input string to validate.</param>
        /// <returns>True if the input is safe, otherwise false.</returns>
        public static bool IsSafe(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return true;
            }

            // Check if the input matches the SQL injection pattern
            return !SqlInjectionPattern.IsMatch(input);
        }
    }
}