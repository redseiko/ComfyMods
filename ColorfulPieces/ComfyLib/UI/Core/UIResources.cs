namespace ComfyLib;

using System.Collections.Generic;

using UnityEngine;

public static class UIResources {
  public static readonly ResourceCache<Sprite> SpriteCache = new();
  public static readonly ResourceCache<Material> MaterialCache = new();

  public static Sprite GetSprite(string spriteName) => SpriteCache.GetResource(spriteName);
  public static Material GetMaterial(string materialName) => MaterialCache.GetResource(materialName);
}

public sealed class ResourceCache<T> where T : Object {
  readonly Dictionary<string, T> _cache = [];

  public T GetResource(string resourceName) {
    if (!_cache.TryGetValue(resourceName, out T cachedResource)) {
      cachedResource = Resources.FindObjectsOfTypeAll<T>().FirstByNameOrThrow(resourceName);
      _cache[resourceName] = cachedResource;
    }

    return cachedResource;
  }
}
