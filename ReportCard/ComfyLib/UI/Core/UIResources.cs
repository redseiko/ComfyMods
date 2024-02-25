namespace ComfyLib;

using System;
using System.Collections.Generic;
using System.Linq;

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

  public static T FirstByNameOrThrow<T>(this T[] unityObjects, string name) where T : UnityEngine.Object {
    foreach (T unityObject in unityObjects) {
      if (unityObject.name == name) {
        return unityObject;
      }
    }

    throw new InvalidOperationException($"Could not find Unity object of type {typeof(T)} with name: {name}");
  }
}
