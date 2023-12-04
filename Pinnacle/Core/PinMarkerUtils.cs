using System.Collections.Generic;

using TMPro;

using static Pinnacle.PluginConfig;

namespace Pinnacle {
  public static class PinMarkerUtils {
    public static void SetupPinNamePrefab(Minimap minimap) {
      TMP_Text label = minimap.m_pinNamePrefab.GetComponentInChildren<TMP_Text>();
      label.enableAutoSizing = false;
      label.richText = true;

      SetPinNameFont();
      SetPinNameFontSize();
    }

    public static void SetPinNameFont() {
      if (Minimap.m_instance) {
        TMP_FontAsset font = UIResources.GetFontAssetByName(PinFont.Value);
        
        foreach (TMP_Text label in GetPinNameLabels(Minimap.m_instance)) {
          label.font = font;
        }
      }
    }

    public static void SetPinNameFontSize() {
      if (Minimap.m_instance) {
        float fontSize = PinFontSize.Value;

        foreach (TMP_Text label in GetPinNameLabels(Minimap.m_instance)) {
          label.fontSize = fontSize;
        }
      }
    }

    static IEnumerable<TMP_Text> GetPinNameLabels(Minimap minimap) {
      foreach (TMP_Text label in minimap.m_pinNamePrefab.GetComponentsInChildren<TMP_Text>(includeInactive: true)) {
        yield return label;
      }

      foreach (TMP_Text label in minimap.m_pinNameRootLarge.GetComponentsInChildren<TMP_Text>(includeInactive: true)) {
        yield return label;
      }
    }
  }
}
