namespace ComfyLib;

using System.Collections.Generic;

using TMPro;

using UnityEngine;

public static class FontCache {
  public static readonly string ValheimAveriaSansLibreFont = "Valheim-AveriaSansLibre";

  public static TMP_FontAsset ValheimAveriaSansLibreFontAsset {
    get => UnifiedPopup.instance.bodyText.font;
  }

  public static readonly Dictionary<string, TMP_FontAsset> FontAssetCache = [];

  public static TMP_FontAsset GetFontAssetByName(string fontAssetName) {
    if (!FontAssetCache.TryGetValue(fontAssetName, out TMP_FontAsset fontAsset)) {
      fontAsset =
          fontAssetName == ValheimAveriaSansLibreFont
              ? ValheimAveriaSansLibreFontAsset
              :  Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstByNameOrThrow(fontAssetName);

      FontAssetCache[fontAssetName] = fontAsset;
    }

    return fontAsset;
  }
}
