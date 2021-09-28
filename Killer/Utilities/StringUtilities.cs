using System.Text;

namespace KillersLibrary.Utilities {
    public class StringUtilities {
        public static string IndentWithWhitespace(string text, int indentation = 4) {
            var sb = new StringBuilder(text ?? "");
            sb.Insert(0, " ", indentation);

            return sb.ToString();
        }
    }
}
