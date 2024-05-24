namespace Pinnacle;

using System;
using System.Collections.Generic;
using System.Text;

using ComfyLib;

using UnityEngine;

public static class ExploredStatsCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "explored-stats",
        "(Pinnacle) explored-stats",
        action: Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    Minimap minimap = Minimap.m_instance;
    float pixelSize = minimap.m_pixelSize;
    float pixelHalfSize = pixelSize / 2f;
    int textureSize = minimap.m_textureSize;
    int textureHalfSize = textureSize / 2;
    int radius = Mathf.CeilToInt(10500f / pixelSize);
    int radiusSquared = radius * radius;
    minimap.WorldToPixel(Vector3.zero, out int px, out int py);

    WorldGenerator worldGenerator = WorldGenerator.m_instance;
    bool[] explored = minimap.m_explored;

    int validCount = 0;
    int exploredCount = 0;
    ExploredStats exploredStats = new();

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

        exploredStats.BiomeCount[biome]++;
        validCount++;

        if (explored[y * textureSize + x]) {
          exploredCount++;
          exploredStats.BiomeExploredCount[biome]++;
        }
      }
    }

    StringBuilder output = new();
    output.AppendLine($"Explored: {exploredCount / validCount * 100:F2}% ({exploredCount}/{validCount})");

    foreach (Heightmap.Biome biome in (Heightmap.Biome[]) Enum.GetValues(typeof(Heightmap.Biome))) {
      int biomeCount = exploredStats.BiomeCount[biome];
      int biomeExploredCount = exploredStats.BiomeExploredCount[biome];

      if (biomeCount <= 0) {
        continue;
      }

      output.AppendLine(
          $"{Enum.GetName(typeof(Heightmap.Biome), biome)}: "
              + $"{biomeExploredCount / biomeCount * 100}% ({biomeExploredCount}/{biomeCount})");
    }

    Pinnacle.LogInfo(output.ToString());

    return true;
  }
}

public sealed class ExploredStats {
  public Dictionary<Heightmap.Biome, int> BiomeCount = [];
  public Dictionary<Heightmap.Biome, int> BiomeExploredCount = [];

  public ExploredStats() {
    foreach (Heightmap.Biome biome in (Heightmap.Biome[]) Enum.GetValues(typeof(Heightmap.Biome))) {
      BiomeCount[biome] = 0;
      BiomeExploredCount[biome] = 0;
    }
  }
}
