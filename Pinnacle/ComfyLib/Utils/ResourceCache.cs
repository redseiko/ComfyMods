namespace ComfyLib;

using System.Collections.Generic;

using UnityEngine;

public sealed class ResourceCache<T> where T : Object {
  readonly Dictionary<string, T> _cache = [];

  public T GetResource(string resourceName) {
    if (!_cache.TryGetValue(resourceName, out T cachedResource)) {
      cachedResource = Resources.FindObjectsOfTypeAll<T>().FirstByNameOrThrow(resourceName);
      _cache[resourceName] = cachedResource;
    }

    return cachedResource;
  }

  public bool TryGetResource(string resourceName, out T cachedResource) {
    if (!_cache.TryGetValue(resourceName, out cachedResource)) {
      cachedResource = Resources.FindObjectsOfTypeAll<T>().FirstByName(resourceName);
      _cache[resourceName] = cachedResource;
    }

    return cachedResource;
  }
}
