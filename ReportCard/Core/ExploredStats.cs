namespace ReportCard;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;

public sealed class ExploredStats {
  public readonly Dictionary<Heightmap.Biome, int> BiomeTotalCount = [];
  public readonly Dictionary<Heightmap.Biome, int> BiomeExploredCount = [];

  public static readonly float GenerateRadius = 10000f;

  public ExploredStats() {
    Reset();
  }

  public int TotalCount() => BiomeTotalCount.Values.Sum();
  public int ExploredCount() => BiomeExploredCount.Values.Sum();

  public int TotalCount(Heightmap.Biome biome) => BiomeTotalCount[biome];
  public int ExploredCount(Heightmap.Biome biome) => BiomeExploredCount[biome];

  void Reset() {
    BiomeTotalCount.Clear();
    BiomeExploredCount.Clear();

    foreach (Heightmap.Biome biome in GetHeightmapBiomes()) {
      BiomeTotalCount[biome] = 0;
      BiomeExploredCount[biome] = 0;
    }
  }

  Coroutine _generateCoroutine = default;

  public void Generate(Minimap minimap, TextMeshProUGUI label, Action<ExploredStats> onCompleteAction) {
    if (_generateCoroutine != null) {
      minimap.StopCoroutine(_generateCoroutine);
      _generateCoroutine = default;
    }

    Reset();

    _generateCoroutine = minimap.StartCoroutine(GenerateCoroutine(minimap, label, onCompleteAction));
  }

  IEnumerator GenerateCoroutine(Minimap minimap, TextMeshProUGUI label, Action<ExploredStats> onCompleteAction) {
    label.text = "Generating stats...";

    yield return null;

    float pixelSize = minimap.m_pixelSize;
    float pixelHalfSize = pixelSize / 2f;
    int textureSize = minimap.m_textureSize;
    int textureHalfSize = textureSize / 2;
    int radius = Mathf.CeilToInt(GenerateRadius / pixelSize);
    int radiusSquared = radius * radius;
    minimap.WorldToPixel(Vector3.zero, out int px, out int py);

    int counter = 0;

    WorldGenerator worldGenerator = WorldGenerator.m_instance;
    bool[] explored = minimap.m_explored;

    for (int y = py - radius; y <= py + radius; y++) {
      for (int x = px - radius; x <= px + radius; x++) {
        if (x < 0 || y < 0 || x >= textureSize || y >= textureSize) {
          continue;
        }

        int magnitudeSquared = ((x - px) * (x - px)) + ((y - py) * (y - py));

        if (magnitudeSquared >= radiusSquared) {
          continue;
        }

        float wx = (x - textureHalfSize) * pixelSize + pixelHalfSize;
        float wy = (y - textureHalfSize) * pixelSize + pixelHalfSize;
        Heightmap.Biome biome = worldGenerator.GetBiome(wx, wy);

        BiomeTotalCount[biome]++;

        if (explored[y * textureSize + x]) {
          BiomeExploredCount[biome]++;
        }

        counter++;

        if (counter % 15000 == 0) {
          if (label) {
            float percent = ((y * textureSize + x) * 1f / (explored.Length * 1f)) * 100f;
            label.text = $"Generating stats... ({percent:F2}%)";
          }

          yield return null;
        }
      }
    }

    onCompleteAction?.Invoke(this);
  }

  public static Heightmap.Biome[] GetHeightmapBiomes() {
    return (Heightmap.Biome[]) Enum.GetValues(typeof(Heightmap.Biome));
  }
}
