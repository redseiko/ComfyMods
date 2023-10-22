using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace ComfyLib {
  public class UIResources {
    static readonly Dictionary<string, Sprite> _spriteCache = new();

    public static Sprite GetSprite(string spriteName) {
      if (!_spriteCache.TryGetValue(spriteName, out Sprite sprite)) {
        sprite = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(sprite => sprite.name == spriteName);
        _spriteCache[spriteName] = sprite;
      }

      return sprite;
    }
  }
}
