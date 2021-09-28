using KillersLibrary.Utilities;
using KillersLibrary.Enums;

namespace KillersLibrary.Extensions {
    public static class StringExtensions {
        #region Text
        public static string IndentWithWhitespace(this string text, int indentation = 4) => StringUtilities.IndentWithWhitespace(text, indentation);
        #endregion

        //Got this from True Love he allowed me to copy it and modified it a bit.
        #region Markdown
        #region Complex Markdown String Extensions
        public static string MakeLink(this string text, string url) => MarkdownUtilities.MakeLink(text, url);
        #endregion

        #region Simple Markdown String Extensions
        public static string ToItalics(this string text) => MarkdownUtilities.ToItalics(text);
        public static string ToBold(this string text) => MarkdownUtilities.ToBold(text);
        public static string ToUnderline(this string text) => MarkdownUtilities.ToUnderline(text);
        public static string ToStrikeThrough(this string text) => MarkdownUtilities.ToStrikeThrough(text);

        public static string ToCodeBlockSingleline(this string text) => MarkdownUtilities.ToCodeBlockSingleline(text);
        public static string ToCodeBlockMultiline(this string text, string languageCode = null) => MarkdownUtilities.ToCodeBlockMultiline(text, languageCode);
        public static string ToCodeBlockMultiline(this string text, CodingLanguages codingLanguages) => MarkdownUtilities.ToCodeBlockMultiline(text, codingLanguages);

        public static string ToBlockQuoteSingleline(this string text) => MarkdownUtilities.ToBlockQuoteSingleline(text);
        public static string ToBlockQuoteMultiline(this string text) => MarkdownUtilities.ToBlockQuoteMultiline(text);

        public static string ToSpoiler(this string text) => MarkdownUtilities.ToSpoiler(text);
        #endregion

        #region Composite Markdown String Extensions
        public static string ToBoldItalics(this string text) => MarkdownUtilities.ToBoldItalics(text);
        public static string ToUnderlineItalics(this string text) => MarkdownUtilities.ToUnderlineItalics(text);
        public static string ToUnderlineBold(this string text) => MarkdownUtilities.ToUnderlineBold(text);
        public static string ToUnderlineBoldItalics(this string text) => MarkdownUtilities.ToUnderlineBoldItalics(text);
        #endregion
        #endregion
    }
}
