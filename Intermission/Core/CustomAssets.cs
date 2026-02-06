namespace Intermission;

using System.Collections.Generic;
using System.IO;
using System.Reflection;

using ComfyLib;

using HarmonyLib;

using UnityEngine;

public static class CustomAssets {
  public static List<string> LoadingTips { get; } = [];
  public static List<string> LoadingImageFiles { get; } = [];

  static readonly Dictionary<string, Sprite> _loadingImageCache = [];

  static readonly MethodInfo _loadImageMethod =
      AccessTools.Method(typeof(ImageConversion), nameof(ImageConversion.LoadImage), [typeof(Texture2D), typeof(byte[])]);

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

      return [];
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

    Texture2D texture = new(1, 1) {
      name = $"intermission.texture-{Path.GetFileName(imageFile)}"
    };

    _loadImageMethod.Invoke(obj: null, [texture, File.ReadAllBytes(imageFile)]);

    sprite = Sprite.Create(texture, new(0, 0, texture.width, texture.height), Vector2.zero, 1);
    sprite.name = $"intermission.sprite-{Path.GetFileName(imageFile)}";

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
