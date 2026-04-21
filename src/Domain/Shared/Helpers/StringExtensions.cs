using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shared.Helpers
{
    public static class StringExtensions
    {
        public static string RemoveWhitespace(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            StringBuilder resultBuilder = new StringBuilder();
            bool previousCharWasWhitespace = false;

            foreach (char c in input)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (!previousCharWasWhitespace)
                    {
                        resultBuilder.Append(' ');
                    }
                    previousCharWasWhitespace = true;
                }
                else
                {
                    resultBuilder.Append(c);
                    previousCharWasWhitespace = false;
                }
            }

            return resultBuilder.ToString();
        }

        public static string Default(this string input)
        {
            if (input.IsNullEmptyOrWhiteSpace())
            {
                return "";
            }
            return input.RemoveWhitespace();
        }
        public static bool IsStringNumber(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            if (int.TryParse(input, out int result))
            {
                return true;
            }
            else if (double.TryParse(input, out double doubleResult))
            {
                return true;
            }
            else if (decimal.TryParse(input, out decimal decimalResult))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool IsNullEmptyOrWhiteSpace(this string input)
        {
            return string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input) ? true : false;
        }
        public static bool? IsBoolean(this string input)
        {
            if (UtilityService.IsNullEmptyOrWhiteSpace(input))
            {
                return null;
            }
            if (bool.TryParse(input, out bool result))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string ToProperCase(this string str)
        {
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(str.ToLower());
        }
    }
}
