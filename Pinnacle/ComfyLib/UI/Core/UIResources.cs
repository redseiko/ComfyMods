namespace ComfyLib;

using System.Collections.Generic;

using TMPro;

using UnityEngine;

public static class UIResources {
  public static readonly ResourceCache<Sprite> SpriteCache = new();

  public static Sprite GetSprite(string spriteName) {
    return SpriteCache.GetResource(spriteName);
  }

  public static Dictionary<string, TMP_FontAsset> FontAssetCache { get; private set; } = [];

  public static string ValheimNorseFont = "Valheim-Norse";
  public static string ValheimAveriaSansLibre = "Valheim-AveriaSansLibre";

  public static TMP_FontAsset ValheimNorseFontAsset {
    get => Minimap.m_instance.m_pinNamePrefab.GetComponentInChildren<TextMeshProUGUI>().font;
  }

  public static TMP_FontAsset ValheimAveriaSansLibreFontAsset {
    get => UnifiedPopup.instance.bodyText.font;
  }

  public static TMP_FontAsset GetFontAssetByName(string fontAssetName) {
    if (FontAssetCache.TryGetValue(fontAssetName, out TMP_FontAsset fontAsset)) {
      return fontAsset;
    }

    if (fontAssetName == ValheimNorseFont) {
      fontAsset = ValheimNorseFontAsset;
      FontAssetCache[fontAssetName] = fontAsset;

      return fontAsset;
    } else if (fontAssetName == ValheimAveriaSansLibre) {
      fontAsset = ValheimAveriaSansLibreFontAsset;
      FontAssetCache[fontAssetName] = fontAsset;

      return fontAsset;
    }

    foreach (TMP_FontAsset existingFontAsset in Resources.FindObjectsOfTypeAll<TMP_FontAsset>()) {
      FontAssetCache[existingFontAsset.name] = existingFontAsset;
    }

    return FontAssetCache[fontAssetName];
  }
}
