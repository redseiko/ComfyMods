namespace ComfySigns;

using System.Text.RegularExpressions;

using ComfyLib;

using TMPro;

using UnityEngine;

using static PluginConfig;

public static class SignUtils {
  public static readonly Regex SizeRegex = new(@"<size=[^>]*>");

  public static void AddFallbackFont(TMP_FontAsset font, TMP_FontAsset fallbackFont) {
    if (!font || !fallbackFont || fallbackFont == font) {
      return;
    }

    if (font.fallbackFontAssetTable == null) {
      font.fallbackFontAssetTable = [fallbackFont];
    } else if (!font.fallbackFontAssetTable.Contains(fallbackFont)) {
      font.fallbackFontAssetTable.Add(fallbackFont);
    }
  }

  public static void AddFallbackFonts(TMP_FontAsset font) {
    AddFallbackFont(font, UIFonts.GetFontAsset(UIFonts.ValheimNorse));
    AddFallbackFont(font, UIFonts.GetFontAsset(UIFonts.ValheimNorsebold));
    AddFallbackFont(font, UIFonts.GetFontAsset(UIFonts.FallbackNotoSansNormal));
  }

  public static bool HasSignEffect(TMP_Text textComponent, string effectId) {
    if (textComponent.text.Length <= 0 || !textComponent.text.StartsWith("<link", System.StringComparison.Ordinal)) {
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
        } else {
          sign.m_textWidget.text = sign.m_currentText;
        }
      }
    }
  }

  public static void ProcessSignEffect(Sign sign) {
    if (sign.m_textWidget.enabled && HasSignEffect(sign.m_textWidget, "party") && ShouldRenderSignEffect(sign)) {
      if (!sign.m_textWidget.gameObject.TryGetComponent(out VertexColorCycler _)) {
        sign.m_textWidget.gameObject.AddComponent<VertexColorCycler>();
      }
    } else if (sign.m_textWidget.gameObject.TryGetComponent(out VertexColorCycler colorCycler)) {
      UnityEngine.Object.Destroy(colorCycler);
      sign.m_textWidget.ForceMeshUpdate(ignoreActiveState: true);
    }
  }

  public static void ProcessSignText(Sign sign) {
    if (SignTextIgnoreSizeTags.Value && SizeRegex.IsMatch(sign.m_textWidget.text)) {
      sign.m_textWidget.text = SizeRegex.Replace(sign.m_textWidget.text, string.Empty);
    }
  }

  public static void SetupSignFont(Sign sign, TMP_FontAsset fontAsset, Color color) {
    if (!sign.m_textWidget.font == fontAsset) {
      sign.m_textWidget.font = fontAsset;
    }

    if (sign.m_textWidget.fontSharedMaterial != fontAsset.material) {
      sign.m_textWidget.fontSharedMaterial = fontAsset.material;
    }

    sign.m_textWidget.color = color;
  }

  public static void SetupSignPrefabs(ZNetScene netScene) {
    if (!netScene) {
      return;
    }

    TMP_FontAsset fontAsset = UIFonts.GetFontAsset(SignDefaultTextFontAsset.Value);
    Color fontColor = SignDefaultTextFontColor.Value;

    if (UseFallbackFonts.Value) {
      AddFallbackFonts(fontAsset);
    }

    foreach (GameObject prefab in netScene.m_namedPrefabs.Values) {
      if (prefab.TryGetComponent(out Sign sign)) {
        SetupSignFont(sign, fontAsset, fontColor);
      }
    }

    foreach (ZNetView netView in netScene.m_instances.Values) {
      if (netView.TryGetComponent(out Sign sign)) {
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
