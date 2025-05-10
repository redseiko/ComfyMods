namespace Pinnacle;

using System.Text.RegularExpressions;

using UnityEngine;

public static class PinManager {
  public static Regex PinIconColorRegex =
      new Regex(
          @".*\[(#[0-9a-fA-F]{6})\]",
          RegexOptions.Compiled | RegexOptions.CultureInvariant,
          System.TimeSpan.FromMilliseconds(50));

  public static void SetPinIconColor(Minimap.PinData pinData) {
    if (TryGetPinIconColor(pinData.m_name, out Color iconColor)) {
      pinData.m_iconElement.color = iconColor;
    }
  }

  public static void RemovePinIconColorText(Minimap.PinNameData pinNameData) {
    string nameText = pinNameData.PinNameText.text;

    if (IsValidPinIconColorName(nameText)) {
      Match match = PinIconColorRegex.Match(nameText);

      if (match.Success) {
        pinNameData.PinNameText.text = nameText.Remove(match.Groups[1].Index - 1, match.Groups[1].Length + 2);
      }
    }
  }

  public static bool TryGetPinIconColor(string pinName, out Color iconColor) {
    if (IsValidPinIconColorName(pinName)) {
      Match match = PinIconColorRegex.Match(pinName);

      if (match.Success && ColorUtility.TryParseHtmlString(match.Groups[1].Value, out iconColor)) {
        return true;
      }
    }

    iconColor = default;
    return false;
  }

  static bool IsValidPinIconColorName(string pinName) {
    if (string.IsNullOrEmpty(pinName)) {
      return false;
    }

    int nameLength = pinName.Length;

    if (nameLength < 9 || pinName[nameLength - 1] != ']') {
      return false;
    }

    return true;
  }
}
