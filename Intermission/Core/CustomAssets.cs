namespace Intermission;

using System;
using System.Collections.Generic;
using System.IO;

using ComfyLib;

using UnityEngine;

public static class CustomAssets {
  public static List<string> LoadingTips { get; } = new();
  public static List<string> LoadingImageFiles { get; } = new();

  static readonly Dictionary<string, Sprite> _loadingImageCache = new();

  public static void Initialize(string pluginDir) {
    LoadingTips.Clear();
    LoadingTips.AddRange(ReadLoadingTips(Path.Combine(pluginDir, "tips.txt")));

    LoadingImageFiles.Clear();
    LoadingImageFiles.AddRange(ReadLoadingImageFiles(pluginDir, ".png"));
    LoadingImageFiles.AddRange(ReadLoadingImageFiles(pluginDir, ".jpg"));
  }

  public static IEnumerable<string> ReadLoadingTips(string path) {
    if (File.Exists(path)) {
      string[] loadingTips = File.ReadAllLines(path);
      Intermission.LogInfo($"Found {loadingTips.Length} custom tips in file: {path}");

      return loadingTips;
    } else {
      Intermission.LogInfo($"Creating new empty custom tips file: {path}");
      Directory.CreateDirectory(Path.GetDirectoryName(path));
      File.Create(path);

      return Array.Empty<string>();
    }
  }

  public static IEnumerable<string> ReadLoadingImageFiles(string path, string extension) {
    Directory.CreateDirectory(Path.GetDirectoryName(path));

    string[] loadingImageFiles = Directory.GetFiles(path, $"*{extension}", SearchOption.TopDirectoryOnly);

    Intermission.LogInfo(
        $"Found {loadingImageFiles.Length} custom loading images ({extension}) in directory: {path}");

    return loadingImageFiles;
  }

  public static Sprite ReadLoadingImage(string imageFile) {
    if (_loadingImageCache.TryGetValue(imageFile, out Sprite sprite)) {
      return sprite;
    }

    if (!File.Exists(imageFile)) {
      Intermission.LogError($"Could not find custom loading image: {imageFile}");
      return null;
    }

    Texture2D texture = new(1, 1);
    texture.SetName($"intermission.texture-{Path.GetFileName(imageFile)}");
    texture.LoadImage(File.ReadAllBytes(imageFile));

    sprite = Sprite.Create(texture, new(0, 0, texture.width, texture.height), Vector2.zero, 1);
    sprite.SetName($"intermission.sprite-{Path.GetFileName(imageFile)}");

    _loadingImageCache[imageFile] = sprite;

    return sprite;
  }

  public static bool GetRandomLoadingTip(out string tipText) {
    if (LoadingTips.Count > 0) {
      tipText = LoadingTips[UnityEngine.Random.Range(0, LoadingTips.Count)];
      return true;
    }

    tipText = default;
    return false;
  }

  static int _loadingImageIndex = -1;

  public static bool GetRandomLoadingImage(out Sprite loadingImageSprite) {
    if (LoadingImageFiles.Count > 0) {
      if (_loadingImageIndex < 0 || _loadingImageIndex >= LoadingImageFiles.Count) {
        LoadingImageFiles.Sort(RandomStringComparer.Instance);
        _loadingImageIndex = 0;
      }

      loadingImageSprite = ReadLoadingImage(LoadingImageFiles[_loadingImageIndex]);
      _loadingImageIndex++;

      return loadingImageSprite;
    }

    loadingImageSprite = default;
    return false;
  }
}
