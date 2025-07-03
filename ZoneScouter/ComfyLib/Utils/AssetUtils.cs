namespace ComfyLib;

using System.Collections.Generic;

using SoftReferenceableAssets;

using UnityEngine;

public static class AssetUtils {
  public static readonly AssetCache<Shader> ShaderCache = new();

  public static Shader StandardShader => ShaderCache.GetAsset("7e6bbee7a32b746cb9396cd890ce7189");
  public static Shader DistortionShader => ShaderCache.GetAsset("31ecdd7109334474ab03326b6095f40b");
}

public sealed class AssetCache<T> where T : Object {
  readonly Dictionary<string, T> _cache = [];

  public T GetAsset(string assetId) {
    if (!_cache.TryGetValue(assetId, out T asset) && AssetID.TryParse(assetId, out AssetID assetIdStruct)) {
      SoftReference<T> reference = new(assetIdStruct);
      reference.Load();
      asset = reference.Asset;
      _cache[assetId] = asset;
    }

    return asset;
  }
}
