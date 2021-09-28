using System;
using KillersLibrary.Enums;

namespace KillersLibrary.Utilities {
    public class MarkdownUtilities {
        //Got this from True Love he allowed me to copy it and modified it a bit.
        #region Complex Markdown
        public static string MakeLink(string text, string url) => $"[{text}]({url})";
        #endregion

        #region Simple Markdown
        public static string ToItalics(string text) => $"*{text}*";
        public static string ToBold(string text) => $"**{text}**";
        public static string ToUnderline(string text) => $"__{text}__";
        public static string ToStrikeThrough(string text) => $"~~{text}~~";

        public static string ToCodeBlockSingleline(string text) => $"`{text}`";
        public static string ToCodeBlockMultiline(string text, string languageCode = null) => $"```{languageCode ?? ""}\n{text}```";
        public static string ToCodeBlockMultiline(string text, CodingLanguages codingLanguages) => $"```{Enum.GetName(typeof(CodingLanguages), codingLanguages)}\n{text}```";

        public static string ToBlockQuoteSingleline(string text) => $"> {text}";
        public static string ToBlockQuoteMultiline(string text) => $">>> {text}";

        public static string ToSpoiler(string text) => $"||{text}||";
        #endregion

        #region Composite Markdown
        public static string ToBoldItalics(string text) => ToItalics(ToBold(text));
        public static string ToUnderlineItalics(string text) => ToUnderline(ToItalics(text));
        public static string ToUnderlineBold(string text) => ToUnderline(ToBold(text));
        public static string ToUnderlineBoldItalics(string text) => ToUnderline(ToBold(ToItalics(text)));
        #endregion
    }
}
