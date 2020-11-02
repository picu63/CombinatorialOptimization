using GraphManager.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GraphManager.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainsAny(this string text, IEnumerable<string> values)
        {
            foreach (var item in values)
            {
                if (text.Contains(item))
                    return true;
            }

            return false;
        }
        public static string Remove(this string text, string findAndRemove)
        {
            return text.Replace(findAndRemove, "");
        }

        public static string SkipFirstLetter(this string text)
        {
            return text.Remove(0, 1);
        }

        public static string SkipLastLetter(this string text)
        {
            return text.Remove(text.Length - 1, 1);
        }

        public static bool IsBracketed(this string text, Brackets brackets)
        {
            return text.IsBracketed(brackets.Opening, brackets.Closing);
        }

        public static bool IsNotBracketed(this string text, Brackets brackets)
        {
            return text.IsNotBracketed(brackets.Opening, brackets.Closing);
        }

        public static bool IsBracketed(this string text, char begin, char end)
        {
            if (text.Length < 2) return false;

            return 
                text.First().Equals(begin)
                &&
                text.Last().Equals(end);
        }

        public static bool IsNotBracketed(this string text, char begin, char end)
        {
            if (text.Length < 2) return true;

            return
                text.First().NotEquals(begin)
                ||
                text.Last().NotEquals(end);
        }

        public static bool NotEquals(this char text, char obj)
        {
            return !text.Equals(obj);
        }

        public static bool NotEquals(this string text, string? obj)
        {
            return !text.Equals(obj);
        }

        public static bool NotEquals(this string text, string? obj, StringComparison comparisonType)
        {
            return !text.Equals(obj, comparisonType);
        }

        public static string TakeFirstPart(this string text)
        {
            return text.TakeNthWord(1);
        }

        public static string TakeSecondPart(this string text)
        {
            return text.TakeNthWord(2);
        }

        public static string TakeThirdPart(this string text)
        {
            var name = TakeFirstPart(text);
            var type = TakeSecondPart(text);

            return text
                .Remove(name)
                .Remove(type)
                .Trim();

        }

        public static string TakeNthWord(this string text, int wordNumber, int firstWordIndex = 1)
        {
            var parts = text.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var howManyParts = parts.Count();

            if ((wordNumber - firstWordIndex) > (howManyParts - 1)) return string.Empty;

            return parts[wordNumber - firstWordIndex];
        }

        public static bool IsNot(this string word, string toCompare, bool caseSensitive = false)
        {
            return !word.Equals(toCompare, caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool Is(this string word, string toCompare, bool caseSensitive = false)
        {
            return word.Equals(toCompare, caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool IsNot(this string word, Regex regex)
        {
            return !regex.Match(word).Value.Equals(word);
        }

        public static bool Is(this string word, Regex regex)
        {
            return regex.Match(word).Value.Equals(word);
        }

        public static string RemoveEndOfLineCharacters(this string text)
        {
            return text
                .Replace("\r", "")
                .Replace("\n", "");
        }

        public static string RemoveDoubleSpaces(this string text)
        {
            return text
                .Replace("  ", " ");
        }

    }
}
