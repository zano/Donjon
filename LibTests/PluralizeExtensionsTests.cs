using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Globalization;

namespace LibTests {
    public static class PluralizeExtensions {
        private static readonly Dictionary<string, string> PluralForms = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            { "letter", "letters" },
            { "corresponds", "correspond" },
            { "it", "they" }
        };

        public static T GetValueOrKey<T>(this Dictionary<T, T> dict, T key) 
            => dict.TryGetValue(key, out T value) ? value : key;

        public static string Pluralize(this string text, bool isPlural) {
            if (!isPlural) return text;
            var words = Regex.Split(text, @"\b");
            for (int i = 0; i < words.Length; i++) {
                if (PluralForms.TryGetValue(words[i], out string pluralized)) words[i] = ToCase(words[i], pluralized);
            }

            //return string.Join("", words.Select(s => PluralForms.ContainsKey(s) ? ToCase(s, PluralForms[s]) : s));

            return string.Join("", words);
        }

        static readonly TextInfo TextInfo = CultureInfo.InvariantCulture.TextInfo;

        public static string FixCase(string template, string input) {
            if (template.All(c => !char.IsLower(c))) return TextInfo.ToUpper(input);
            if (template.All(c => !char.IsUpper(c))) return TextInfo.ToLower(input);
            if (char.IsUpper(template[0]) && template.Skip(1).All(c => !char.IsUpper(c))) return TextInfo.ToTitleCase(input);
            return input;
        }

        public static string ToCase(string template, string input) {
            var t = TextInfo;
            // valid cases: ABC Abc abc, A0C, A0c
            // unhandled cases: AbC aBc aBC 0bc 0BC 0Bc

            if (!char.IsLetterOrDigit(template[0])) return input;
            if (char.IsUpper(template[0])) {
                if (template.Skip(1).All(c => !char.IsUpper(c))) return t.ToTitleCase(input);
                if (template.Skip(1).All(c => !char.IsLower(c))) return t.ToUpper(input);
            }
            if (template.All(c => !char.IsUpper(c))) return t.ToLower(input);
            return input;
        }
    }

    [TestFixture]
    public class PluralizeExtensionsTests {
        [Test]
        [TestCase("Xxx", "abC", "Abc")]
        [TestCase("xxx", "abC", "abc")]
        [TestCase("XXX", "abC", "ABC")]
        [TestCase("A0C", "a0C", "A0C")]
        [TestCase("A0c", "a0C", "A0c")]
        [TestCase("ÅÄÖ", "åäö", "ÅÄÖ")]
        public void DoesConvert(string template, string input, string expect) {
            var convert = PluralizeExtensions.ToCase(template);
            var result = convert(input);
            Assert.That(result, Is.EqualTo(expect));
        }

        [Test]
        [TestCase("AbC", "xxx")]
        [TestCase("aBc", "xxx")]
        [TestCase("aBC", "xxx")]
        [TestCase("0bc", "xxx")]
        [TestCase("0BC", "xxx")]
        [TestCase("0Bc", "xxx")]
        public void DoesntConvert(string template, string input) {
            var convert = PluralizeExtensions.ToCase(template);
            Func<string, string> nop = x => x;
            var result = convert(input);
            Assert.That(result, Is.EqualTo(input));
        }

        [Test]
        [TestCase("The letter corresponds properly.\n" + "It must be capitalized.\n")]
        public void DoesntPluralize(string input) {
            var result = input.Pluralize(isPlural: false);

            Assert.That(result, Is.EqualTo(input));
        }

        [Test]
        [TestCase(
            "The letter corresponds properly.\n" + "It must be capitalized.\n",
            "The letters correspond properly.\n" + "They must be capitalized.\n")]
        public void DoesPluralize(string input, string expect) {
            var result = input.Pluralize(isPlural: true);

            Assert.That(result, Is.EqualTo(expect));
        }

        [Test]
        [TestCase(
            "The Letter Corresponds properly.\n" + "it must be capitalized.\n",
            "The Letters Correspond properly.\n" + "they must be capitalized.\n")]
        [TestCase(
            "THE LETTER CORRESPONDS PROPERLY.\n" + "IT MUST BE CAPITALIZED.\n",
            "THE LETTERS CORRESPOND PROPERLY.\n" + "THEY MUST BE CAPITALIZED.\n")]
        public void DoesPluralizeWithCorrectCase(string input, string expect) {
            var result = input.Pluralize(isPlural: true);

            Assert.That(result, Is.EqualTo(expect));
        }
    }
}