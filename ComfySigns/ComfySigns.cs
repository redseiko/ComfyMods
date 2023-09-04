using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using BepInEx;

using ComfyLib;

using HarmonyLib;

using TMPro;

using UnityEngine;

using static ComfySigns.PluginConfig;

namespace ComfySigns {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class ComfySigns : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.comfysigns";
    public const string PluginName = "ComfySigns";
    public const string PluginVersion = "1.4.0";

    Harmony _harmony;

    public void Awake() {
      BindConfig(Config);

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    static readonly Regex _sizeRegex = new(@"<size=[^>]*>");

    public static void ProcessSignText(Sign sign) {
      if (SignTextIgnoreSizeTags.Value && _sizeRegex.IsMatch(sign.m_textWidget.text)) {
        sign.m_textWidget.text = _sizeRegex.Replace(sign.m_textWidget.text, string.Empty);
      }
    }

    public static void ProcessSignEffect(Sign sign) {
      if (HasSignEffect(sign.m_textWidget, "party") && ShouldRenderSignEffect(sign)) {
        if (!sign.m_textWidget.gameObject.TryGetComponent(out VertexColorCycler _)) {
          sign.m_textWidget.gameObject.AddComponent<VertexColorCycler>();
        }
      } else {
        if (sign.m_textWidget.gameObject.TryGetComponent(out VertexColorCycler colorCycler)) {
          Destroy(colorCycler);
          sign.m_textWidget.ForceMeshUpdate(ignoreActiveState: true);
        }
      }
    }

    public static bool HasSignEffect(TMP_Text textComponent, string effectId) {
      if (textComponent.text.Length <= 0 || !textComponent.text.StartsWith("<link")) {
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

    public static bool ShouldRenderSignEffect(Sign sign) {
      return
          SignEffectEnablePartyEffect.Value
          && Player.m_localPlayer
          && Vector3.Distance(Player.m_localPlayer.transform.position, sign.transform.position)
              <= SignEffectMaximumRenderDistance.Value;
    }

    public static readonly EventHandler OnSignConfigChanged = (_, _) => {
      TMP_FontAsset font = UIFonts.GetFontAsset(SignDefaultTextFont.Value);

      if (UseFallbackFonts.Value && font) {
        AddFallbackFont(font);
      }

      Color color = SignDefaultTextColor.Value;

      foreach (Sign sign in Resources.FindObjectsOfTypeAll<Sign>()) {
        if (sign && sign.m_nview && sign.m_nview.IsValid() && sign.m_textWidget) {
          sign.m_textWidget
              .SetFont(font)
              .SetColor(color);
        }
      }
    };

    public static readonly EventHandler OnSignTextTagsConfigChanged = (_, _) => {
      foreach (Sign sign in Resources.FindObjectsOfTypeAll<Sign>()) {
        if (sign && sign.m_nview && sign.m_nview.IsValid() && sign.m_textWidget) {
          if (SignTextIgnoreSizeTags.Value) {
            ProcessSignText(sign);
          } else {
            sign.m_textWidget.text = sign.m_currentText;
          }
        }
      }
    };

    public static readonly EventHandler OnSignEffectConfigChanged = (_, _) => {
      foreach (Sign sign in Resources.FindObjectsOfTypeAll<Sign>()) {
        if (sign && sign.m_nview && sign.m_nview.IsValid() && sign.m_textWidget) {
          ProcessSignEffect(sign);
        }
      }
    };

    public static void AddFallbackFont(TMP_FontAsset font) {
      if (!font) {
        return;
      }

      TMP_FontAsset fallbackFont = UIFonts.GetFontAsset(UIFonts.ValheimNorse);
      
      if (!fallbackFont) {
        return;
      }

      if (font.fallbackFontAssetTable == null) {
        font.fallbackFontAssetTable = new() { fallbackFont };
      } else if (!font.fallbackFontAssetTable.Contains(fallbackFont)) {
        font.fallbackFontAssetTable.Add(fallbackFont);
      }
    }
  }
}