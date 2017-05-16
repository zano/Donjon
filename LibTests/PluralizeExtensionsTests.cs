using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lib.Tests {

    public static class PluralizeExtensions
    {
        private static Dictionary<string, string> pluralForms = new Dictionary<string, string> {
            { "letter", "letters" },
            { "corresponds", "correspond" },
            { "It", "They" }
        };

        public static string Pluralize(this string text, bool isPlural)
        {
            if (!isPlural) return text;
            var words = Regex.Split(text, @"\b");
            for (int i = 0; i < words.Length; i++)
            {
                if (pluralForms.ContainsKey(words[i])) words[i] = pluralForms[words[i]];
            }
            return string.Join("", words);
        }
    }

    [TestClass()]
    public class PluralizeExtensionsTests
    {
        private string asSingular = "The letter corresponds properly.\n" + "It must be capitalized.\n";
        readonly string asPlural = "The letters correspond properly.\n" + "They must be capitalized.\n";

        [TestMethod()]
        public void PluralizeTest_Plural()
        {
            var text = asSingular.Pluralize(isPlural: true);

            Assert.AreEqual(asPlural, text);
        }

        [TestMethod()]
        public void PluralizeTest_Singular()
        {
            var text = asSingular.Pluralize(isPlural: false);

            Assert.AreEqual(asSingular, text);
        }
    }
}