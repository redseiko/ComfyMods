using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

namespace CarbonCopy {
  static class UIResources {
    static readonly Dictionary<string, Sprite> _spriteByNameCache = new();
    static DefaultControls.Resources _resources = new();

    static Sprite GetSprite(string spriteName) {
      if (!_spriteByNameCache.TryGetValue(spriteName, out Sprite sprite)) {
        sprite = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(sprite => sprite.name == spriteName);
        _spriteByNameCache[spriteName] = sprite;
      }

      return sprite;
    }

    public static DefaultControls.Resources CreateResources() {
      _resources.standard ??= GetSprite("UISprite");
      _resources.background ??= GetSprite("Background");
      _resources.inputField ??= GetSprite("InputFieldBackground");
      _resources.knob ??= GetSprite("Knob");
      _resources.checkmark ??= GetSprite("Checkmark");
      _resources.dropdown ??= GetSprite("DropdownArrow");
      _resources.mask ??= GetSprite("UIMask");

      return _resources;
    }
  }
}
