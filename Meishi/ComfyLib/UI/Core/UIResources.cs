namespace ComfyLib;

using UnityEngine;

public static class UIResources {
  public static readonly ResourceCache<Sprite> SpriteCache = new();
  public static readonly ResourceCache<Material> MaterialCache = new();

  public static Sprite GetSprite(string spriteName) => SpriteCache.GetResource(spriteName);
  public static Material GetMaterial(string materialName) => MaterialCache.GetResource(materialName);
}
