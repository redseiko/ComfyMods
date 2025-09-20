namespace ComfyLib;

using System.Collections.Generic;

using TMPro;

using UnityEngine;

public static class UIFonts {
  public static readonly Dictionary<string, Font> FontCache = [];

  public static Font GetFont(string fontName) {
    if (!FontCache.TryGetValue(fontName, out Font font)) {
      font = Resources.FindObjectsOfTypeAll<Font>().FirstByNameOrThrow(fontName);
      FontCache[fontName] = font;
    }

    return font;
  }

  public const string ValheimAveriaSansLibre = "Valheim-AveriaSansLibre";
  public const string ValheimNorse = "Valheim-Norse";
  public const string ValheimNorsebold = "Valheim-Norsebold";
  public const string FallbackNotoSansNormal = "Fallback-NotoSansNormal";

  public static readonly Dictionary<string, TMP_FontAsset> FontAssetCache = [];

  public static TMP_FontAsset GetFontAsset(string fontName) {
    if (!FontAssetCache.TryGetValue(fontName, out TMP_FontAsset fontAsset)) {
      fontAsset = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstByNameOrDefault(fontName);

      if (!fontAsset) {
        Font font = GetFont(fontName);

        fontAsset = TMP_FontAsset.CreateFontAsset(font);
        fontAsset.name = fontName;
      }

      FontAssetCache[fontName] = fontAsset;
    }

    return fontAsset;
  }
}
