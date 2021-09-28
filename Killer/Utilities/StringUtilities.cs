using System.Text;

namespace KillersLibrary.Utilities {
    public class StringUtilities {
        public static string IndentWithWhitespace(string text, int indentationAmount = 4) {
            Preconditions.NotNullOrEmpty(text, nameof(text));

            var sb = new StringBuilder(text);
            sb.Insert(0, " ", indentationAmount);

            return sb.ToString();
        }

        public static string IndentWithCharacter(string text, string indentationCharacter, int indentationAmount = 4) {
            Preconditions.NotNullOrEmpty(indentationCharacter, nameof(indentationCharacter));

            var sb = new StringBuilder(text ?? "");
            sb.Insert(0, indentationCharacter, indentationAmount);

            return sb.ToString();
        }
    }
}
