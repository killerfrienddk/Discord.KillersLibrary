using System.Text;

namespace KillersLibrary.Utilities {
    public class StringUtilities {
        /// <summary>Indents using whitespace like.</summary>
        /// <param name="text">The text you want to be indented.</param>
        /// <param name="indentationAmount">The amount of spaces you want the <paramref name="text"/> to be intented with.</param>
        /// <returns>Indented text.</returns>
        public static string IndentWithWhitespace(string text, int indentationAmount = 4) => IndentWithCharacter(text, " ", indentationAmount);

        /// <summary>Indents using a string input.</summary>
        /// <param name="text">The text you want to be indented.</param>
        /// <param name="indentationAmount">The amount of spaces or char you want the <paramref name="text"/> to be intented with.</param>
        /// <param name="indentationCharacter">The amount of spaces you want it to be intented.</param>
        /// <returns>Indented text.</returns>
        public static string IndentWithCharacter(string text, string indentationCharacter, int indentationAmount = 4) {
            Preconditions.NotNullOrEmpty(indentationCharacter, nameof(indentationCharacter));

            var sb = new StringBuilder(text);
            sb.Insert(0, indentationCharacter, indentationAmount);

            return sb.ToString();
        }
    }
}
