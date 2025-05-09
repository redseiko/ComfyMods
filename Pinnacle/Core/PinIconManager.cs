namespace Pinnacle;

using System.Text.RegularExpressions;

using ComfyLib;

using UnityEngine;

using static PluginConfig;

public static class PinIconManager {
  public static void ProcessIconTagsCreated(Minimap.PinData pinData) {
    bool hasIconTag = false;

    hasIconTag |= ProcessPinIconColorTags.Value && TryProcessIconColorTag(pinData);
    hasIconTag |= ProcessPinIconSpriteTags.Value && TryProcessIconSpriteTag(pinData);
    hasIconTag |= ProcessPinIconScaleTags.Value && TryProcessIconScaleTag(pinData);

    if (hasIconTag) {
      pinData.m_worldSize = uint.MinValue;
    }
  }

  public static void ProcessIconTagsModified(Minimap.PinData pinData) {
    bool hasIconTagFlag = HasIconTagFlag(pinData);
    bool hasIconTag = false;

    if (ProcessPinIconColorTags.Value && TryProcessIconColorTag(pinData)) {
      hasIconTag = true;
    } else if (hasIconTagFlag) {
      ResetIconColor(pinData);
    }

    if (ProcessPinIconSpriteTags.Value && TryProcessIconSpriteTag(pinData)) {
      hasIconTag = true;
    } else if (hasIconTagFlag) {
      ResetIconSprite(pinData);
    }

    if (ProcessPinIconScaleTags.Value && TryProcessIconScaleTag(pinData)) {
      hasIconTag = true;
    } else if (hasIconTagFlag) {
      ResetIconScale(pinData);
    }

    if (hasIconTag) {
      RemoveIconTagText(pinData.m_NamePinData);
      pinData.m_worldSize = uint.MinValue;
    } else if (hasIconTagFlag) {
      pinData.m_worldSize = default;
    }
  }

  public static bool IsValidIconTagName(string pinName) {
    return pinName != default && pinName.Length > 0 && pinName[pinName.Length - 1] == ']';
  }

  public static bool HasIconTagFlag(Minimap.PinData pinData) {
    return pinData.m_worldSize == uint.MinValue;
  }

  public static void RemoveIconTagText(Minimap.PinNameData pinNameData) {
    string nameText = pinNameData.PinNameText.text;

    if (!IsValidIconTagName(nameText)) {
      return;
    }

    if (StripPinIconColorTagText.Value) {
      nameText = RemoveIconColorTagText(nameText);
    }

    if (StripPinIconSpriteTagText.Value) {
      nameText = RemoveIconSpriteTagText(nameText);
    }

    if (StripPinIconScaleTagText.Value) {
      nameText = RemoveIconScaleTagText(nameText);
    }

    pinNameData.PinNameText.text = nameText;
  }

  public static Regex IconColorTagRegex =
      new Regex(
          @".*\[(#[0-9a-fA-F]{6})\].*",
          RegexOptions.Compiled | RegexOptions.CultureInvariant,
          System.TimeSpan.FromMilliseconds(100));

  public static bool TryProcessIconColorTag(Minimap.PinData pinData) {
    string pinName = pinData.m_name;

    if (pinName.Length >= 9) {
      Match match = IconColorTagRegex.Match(pinName);

      if (match.Success && ColorUtility.TryParseHtmlString(match.Groups[1].Value, out Color iconColor)) {
        pinData.m_iconElement.color = iconColor;
        return true;
      }
    }

    return false;
  }

  public static void ResetIconColor(Minimap.PinData pinData) {
    pinData.m_iconElement.color =
        pinData.m_ownerID == 0L
            ? Color.white
            : GetIconFadeColor(Minimap.m_instance);
  }

  public static Color GetIconFadeColor(Minimap minimap) {
    return new Color(0.7f, 0.7f, 0.7f, 0.8f * minimap.m_sharedMapDataFade);
  }

  public static string RemoveIconColorTagText(string nameText) {
    if (nameText.Length >= 9) {
      Match match = IconColorTagRegex.Match(nameText);

      if (match.Success) {
        Group group = match.Groups[1];
        return nameText.Remove(group.Index - 1, group.Length + 2);
      }
    }

    return nameText;
  }

  public static Regex IconSpriteTagRegex =
      new Regex(
          @".*\[:(\w{3,})\].*",
          RegexOptions.Compiled | RegexOptions.CultureInvariant,
          System.TimeSpan.FromMilliseconds(100));

  public static bool TryProcessIconSpriteTag(Minimap.PinData pinData) {
    string pinName = pinData.m_name;

    if (pinName.Length >= 6) {
      Match match = IconSpriteTagRegex.Match(pinName);

      if (match.Success && UIResources.SpriteCache.TryGetResource(match.Groups[1].Value, out Sprite iconSprite)) {
        pinData.m_iconElement.sprite = iconSprite;
        return true;
      }
    }

    return false;
  }

  public static void ResetIconSprite(Minimap.PinData pinData) {
    pinData.m_iconElement.sprite = pinData.m_icon;
  }

  public static string RemoveIconSpriteTagText(string nameText) {
    if (nameText.Length >= 6) {
      Match match = IconSpriteTagRegex.Match(nameText);

      if (match.Success) {
        Group group = match.Groups[1];
        return nameText.Remove(group.Index - 2, group.Length + 3);
      }
    }

    return nameText;
  }

  public static Regex IconScaleTagRegex =
      new Regex(
          @".*\[(\d{2,3})%\]",
          RegexOptions.Compiled | RegexOptions.CultureInvariant,
          System.TimeSpan.FromMilliseconds(100));

  public static bool TryProcessIconScaleTag(Minimap.PinData pinData) {
    string pinName = pinData.m_name;

    if (pinName.Length >= 4) {
      Match match = IconScaleTagRegex.Match(pinName);

      if (match.Success && int.TryParse(match.Groups[1].Value, out int iconScale)) {
        float iconSize = Mathf.Clamp(iconScale, 50, 200) * 0.01f * GetIconDefaultSize(Minimap.m_instance);
        pinData.m_uiElement.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);
        pinData.m_uiElement.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);

        return true;
      }
    }

    return false;
  }

  public static void ResetIconScale(Minimap.PinData pinData) {
    RectTransform uiElement = pinData.m_uiElement;
    float defaultSize = GetIconDefaultSize(Minimap.m_instance);

    uiElement.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, defaultSize);
    uiElement.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, defaultSize);
  }

  public static string RemoveIconScaleTagText(string nameText) {
    if (nameText.Length >= 4) {
      Match match = IconScaleTagRegex.Match(nameText);

      if (match.Success) {
        Group group = match.Groups[1];
        return nameText.Remove(group.Index - 1, group.Length + 3);
      }
    }

    return nameText;
  }

  public static float GetIconDefaultSize(Minimap minimap) {
    return minimap.m_mode == Minimap.MapMode.Large ? minimap.m_pinSizeLarge : minimap.m_pinSizeSmall;
  }
}
