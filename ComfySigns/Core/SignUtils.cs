namespace ComfySigns;

using System;
using System.Text.RegularExpressions;

using ComfyLib;

using TMPro;

using UnityEngine;

using static PluginConfig;

public static class SignUtils {
  public static readonly Regex SizeRegex =
      new Regex(
          @"</?size(?:\s*=[^>]*)?>",
          RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase,
          TimeSpan.FromMilliseconds(100));

  public static void AddFallbackFont(TMP_FontAsset font, TMP_FontAsset fallbackFont) {
    if (!font || !fallbackFont || fallbackFont == font) {
      return;
    }

    if (font.fallbackFontAssets == default) {
      font.fallbackFontAssets = [fallbackFont];
    } else if (!font.fallbackFontAssets.Contains(fallbackFont)) {
      font.fallbackFontAssets.Add(fallbackFont);
    }
  }

  public static void AddFallbackFonts(TMP_FontAsset font) {
    AddFallbackFont(font, UIFonts.GetFontAsset(UIFonts.ValheimNorse));
    AddFallbackFont(font, UIFonts.GetFontAsset(UIFonts.ValheimNorsebold));
    AddFallbackFont(font, UIFonts.GetFontAsset(UIFonts.FallbackNotoSansNormal));
  }

  public static bool HasSignEffect(TextMeshProUGUI textComponent, string effectId) {
    if (textComponent.text.Length <= 0 || !textComponent.text.StartsWith("<link", StringComparison.Ordinal)) {
      return false;
    }

    foreach (TMP_LinkInfo linkInfo in textComponent.textInfo.linkInfo) {
      if (linkInfo.linkTextfirstCharacterIndex == 0
          && linkInfo.linkTextLength == textComponent.textInfo.characterCount
          && linkInfo.GetLinkID() == effectId) {
        return true;
      }
    }

    return false;
  }

  public static void OnSignConfigChanged() {
    SetupSignPrefabs(ZNetScene.s_instance);
  }

  public static void OnSignEffectConfigChanged() {
    foreach (Sign sign in Resources.FindObjectsOfTypeAll<Sign>()) {
      if (sign && sign.m_nview && sign.m_nview.IsValid() && sign.m_textWidget) {
        ProcessSignEffect(sign);
      }
    }
  }

  public static void OnSignTextTagsConfigChanged() {
    foreach (Sign sign in Resources.FindObjectsOfTypeAll<Sign>()) {
      if (sign && sign.m_nview && sign.m_nview.IsValid() && sign.m_textWidget) {
        if (SignTextIgnoreSizeTags.Value) {
          ProcessSignText(sign);
        } else if (sign.m_isViewable) {
          sign.m_textWidget.text = sign.m_currentText;
        }
      }
    }
  }

  public static void ProcessSignEffect(Sign sign) {
    TextMeshProUGUI textWidget = sign.m_textWidget;
    bool shouldRender = textWidget.enabled && HasSignEffect(textWidget, "party") && ShouldRenderSignEffect(sign);

    if (textWidget.gameObject.TryGetComponent(out VertexColorCycler cycler)) {
      if (cycler.enabled != shouldRender) {
        cycler.enabled = shouldRender;
      }
    } else if (shouldRender) {
      textWidget.gameObject.AddComponent<VertexColorCycler>();
    }
  }

  public static void ProcessSignText(Sign sign) {
    if (!SignTextIgnoreSizeTags.Value) {
      return;
    }

    string originalText = sign.m_textWidget.text;
    string sanitizedText = SizeRegex.Replace(originalText, string.Empty);

    if (!ReferenceEquals(originalText, sanitizedText)) {
      sign.m_textWidget.text = sanitizedText;
    }
  }

  public static void SetupSignFont(Sign sign, TMP_FontAsset fontAsset, Color color) {
    TextMeshProUGUI textWidget = sign.m_textWidget;

    if (textWidget.font != fontAsset) {
      textWidget.font = fontAsset;
    }

    if (textWidget.fontSharedMaterial != fontAsset.material) {
      textWidget.fontSharedMaterial = fontAsset.material;
    }

    textWidget.color = color;
  }

  public static void SetupSignPrefabs(ZNetScene netScene) {
    if (!netScene) {
      return;
    }

    TMP_FontAsset fontAsset = UIFonts.GetFontAsset(SignDefaultTextFontAsset.Value);
    Color fontColor = SignDefaultTextFontColor.Value;

    if (UseFallbackFonts.Value) {
      ComfySigns.LogInfo($"Adding fallback fonts to font: {fontAsset.name}");
      AddFallbackFonts(fontAsset);
    }

    foreach (GameObject prefab in netScene.m_namedPrefabs.Values) {
      if (prefab.TryGetComponent(out Sign sign) && sign.m_textWidget) {
        SetupSignFont(sign, fontAsset, fontColor);
      }
    }

    foreach (ZNetView netView in netScene.m_instances.Values) {
      if (netView.TryGetComponent(out Sign sign) && sign.m_textWidget) {
        SetupSignFont(sign, fontAsset, fontColor);
      }
    }
  }

  public static bool ShouldRenderSignText(Sign sign) {
    return
        Player.m_localPlayer
        && Vector3.Distance(sign.transform.position, Player.m_localPlayer.transform.position)
            <= SignTextMaximumRenderDistance.Value;
  }

  public static bool ShouldRenderSignEffect(Sign sign) {
    return
        SignEffectEnablePartyEffect.Value
        && Player.m_localPlayer
        && Vector3.Distance(Player.m_localPlayer.transform.position, sign.transform.position)
            <= SignEffectMaximumRenderDistance.Value;
  }
}
