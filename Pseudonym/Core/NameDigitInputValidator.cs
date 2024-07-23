namespace Pseudonym;

using TMPro;

using UnityEngine;

// Copy of the existing `CharacterValidation.Name` logic in `TMP_InputField.cs` but modified to include digits.
public sealed class NameDigitInputValidator : TMP_InputValidator {
  public static NameDigitInputValidator Create() {
    return CreateInstance<NameDigitInputValidator>();
  }

  // CustomValidation requires doing all the text manipulation.
  public override char Validate(ref string text, ref int pos, char ch) {
    ch = ValidateInternal(text, pos, ch);

    if (ch != 0) {     
      text = text.Insert(pos, ch.ToString());
      pos++;
    }

    return ch;
  }

  static char ValidateInternal(string text, int pos, char ch) {
    char c = (text.Length > 0) ? text[Mathf.Clamp(pos - 1, 0, text.Length - 1)] : ' ';
    char c2 = (text.Length > 0) ? text[Mathf.Clamp(pos, 0, text.Length - 1)] : ' ';
    char c3 = (text.Length > 0) ? text[Mathf.Clamp(pos + 1, 0, text.Length - 1)] : '\n';

    if (ch >= '0' && ch <= '9') {
      return ch;
    }

    if (char.IsLetter(ch)) {
      if (char.IsLower(ch) && pos == 0) {
        return char.ToUpper(ch);
      }

      if (char.IsLower(ch) && (c == ' ' || c == '-')) {
        return char.ToUpper(ch);
      }

      if (char.IsUpper(ch) && pos > 0 && c != ' ' && c != '\'' && c != '-' && !char.IsLower(c)) {
        return char.ToLower(ch);
      }

      if (char.IsUpper(ch) && char.IsUpper(c2)) {
        return '\0';
      }

      return ch;
    }

    if (ch == '\'' && c2 != ' ' && c2 != '\'' && c3 != '\'' && !text.Contains("'")) {
      return ch;
    }

    if (char.IsLetter(c) && ch == '-' && c2 != '-') {
      return ch;
    }

    if ((ch == ' ' || ch == '-')
        && pos != 0
        && c != ' '
        && c != '\''
        && c != '-'
        && c2 != ' '
        && c2 != '\''
        && c2 != '-'
        && c3 != ' '
        && c3 != '\''
        && c3 != '-') {
      return ch;
    }

    return '\0';
  }
}
