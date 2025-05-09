namespace Pinnacle;

using System.Text.RegularExpressions;

using UnityEngine;

public static class PinManager {
  public static Regex PinIconColorRegex =
      new Regex(
          @".*\[(#[0-9a-fA-F]{6})\]",
          RegexOptions.Compiled | RegexOptions.CultureInvariant,
          System.TimeSpan.FromMilliseconds(50));

  public static Regex PinIconColorTagRegex =
      new Regex(
          @".*(\[#[0-9a-fA-F]{6}\])",
          RegexOptions.Compiled | RegexOptions.CultureInvariant,
          System.TimeSpan.FromMilliseconds(50));

  public static bool TryGetCustomPinIconColor(string pinName, out Color iconColor) {
    Match match = PinIconColorRegex.Match(pinName);

    if (match.Success && ColorUtility.TryParseHtmlString(match.Groups[1].Value, out iconColor)) {
      return true;
    }

    iconColor = default;
    return false;
  }
}
