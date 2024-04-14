namespace Pinnacle;

using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;

public static class UIResources {
  static readonly Dictionary<string, Sprite> SpriteCache = new();

  public static Sprite GetSprite(string spriteName) {
    if (!SpriteCache.TryGetValue(spriteName, out Sprite sprite)) {
      sprite = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(sprite => sprite.name == spriteName);
      SpriteCache[spriteName] = sprite;
    }

    return sprite;
  }

  public static Dictionary<string, TMP_FontAsset> FontAssetCache { get; private set; } = new();

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
