namespace ComfyLib;

using System;
using System.Collections.Generic;

using UnityEngine;

public static class UIResources {
  public static readonly Dictionary<string, Sprite> SpriteCache = new();

  public static Sprite GetSprite(string spriteName) {
    if (!SpriteCache.TryGetValue(spriteName, out Sprite cachedSprite) || !cachedSprite) {
      cachedSprite = Resources.FindObjectsOfTypeAll<Sprite>().FirstByNameOrThrow(spriteName);
      SpriteCache[spriteName] = cachedSprite;
    }

    return cachedSprite;
  }

  public static readonly Dictionary<string, Material> MaterialCache = new();

  public static Material GetMaterial(string materialName) {
    if (!MaterialCache.TryGetValue(materialName, out Material cachedMaterial) || !cachedMaterial) {
      cachedMaterial = Resources.FindObjectsOfTypeAll<Material>().FirstByNameOrThrow(materialName);
      MaterialCache[materialName] = cachedMaterial;
    }

    return cachedMaterial;
  }
}
