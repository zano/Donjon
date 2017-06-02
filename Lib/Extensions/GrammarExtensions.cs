using System.Text.RegularExpressions;
using Humanizer;

namespace Lib.Extensions {
    public static class GrammarExtensions {
        /// <summary>
        /// Usage: "car".Count(5) → "5 cars"; "bike".Count(1) → "1 bike";
        /// </summary>
        /// <param name="noun"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Counted(this string noun, int count)
            => count > 2
                ? noun.ToQuantity(count)
                : noun.ToQuantity(count, ShowQuantityAs.Words);

        /// <summary>
        /// Naive implementation of indefinited article selection
        /// </summary>
        /// <param name="noun"></param>
        /// <returns></returns>
        public static string A(this string noun) {
            var article = Regex.IsMatch(noun, @"^[aeiou]", RegexOptions.IgnoreCase) ? "an" : "a";
            return article + " " + noun;
        }
    }
}