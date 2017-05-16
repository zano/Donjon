using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lib {
    public static class GrammarExtensions {
        /// <summary>
        /// Usage: "car".Count(5) → "5 cars"; "bike".Count(1) → "1 bike";
        /// </summary>
        /// <param name="noun"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Count(this string noun, int count)
            => count > 2
                ? noun.ToQuantity(count)
                : noun.ToQuantity(count, ShowQuantityAs.Words);

        public static string A(this string noun) {
            var article = Regex.IsMatch(noun, @"^[aeiou]", RegexOptions.IgnoreCase) ? "an" : "a";
            return article + " " + noun;
        }
    }
}