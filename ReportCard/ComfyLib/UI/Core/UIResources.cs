namespace ComfyLib;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public static class UIResources {
  public static readonly Dictionary<string, Sprite> SpriteCache = new();

  public static Sprite GetSprite(string spriteName) {
    if (!SpriteCache.TryGetValue(spriteName, out Sprite cachedSprite)) {
      cachedSprite = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(s => s.name == spriteName);
      SpriteCache[spriteName] = cachedSprite;
    }

    return cachedSprite;
  }
}
