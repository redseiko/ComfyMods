namespace ComfyLib;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using TMPro;

using UnityEngine;

public static class UIFonts {
  public static readonly Lazy<Dictionary<string, string>> OsFontMap =
      new(() => {
        Dictionary<string, string> map = [];
        foreach (string osFontPath in Font.GetPathsToOSFonts()) {
          map[Path.GetFileNameWithoutExtension(osFontPath)] = osFontPath;
        }

        return map;
      });

  public static readonly string ValheimAveriaSansLibreFont = "Valheim-AveriaSansLibre";
  public static readonly string ValheimNorseFont = "Valheim-Norse";
  public static readonly string ValheimNorseboldFont = "Valheim-Norsebold";
  public static readonly string FallbackNotoSansNormal = "Fallback-NotoSansNormal";

  public static TMP_FontAsset ValheimAveriaSansLibreFontAsset {
    get => UnifiedPopup.instance.bodyText.font;
  }

  static readonly Dictionary<string, TMP_FontAsset> _fontAssetCache = [];

  public static TMP_FontAsset GetFontAssetByName(string fontAssetName) {
    if (!_fontAssetCache.TryGetValue(fontAssetName, out TMP_FontAsset fontAsset)) {
      fontAsset = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstOrDefault(f => f.name == fontAssetName);

      // TODO: do this less hacky.
      if (fontAssetName != ValheimNorseFont && fontAssetName != ValheimNorseboldFont) {
        fontAsset.material.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.175f);
        fontAsset.material.SetFloat(ShaderUtilities.ID_FaceDilate, 0.175f);
      }

      _fontAssetCache[fontAssetName] = fontAsset;
    }

    return fontAsset;
  }
}
