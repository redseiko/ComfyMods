namespace ReportCard;

using System;
using System.Collections.Generic;

public sealed class ExploredStats {
  public readonly Dictionary<Heightmap.Biome, int> BiomeTotalCount = [];
  public readonly Dictionary<Heightmap.Biome, int> BiomeExploredCount = [];

  public ExploredStats() {
    Reset();
  }

  public void Reset() {
    BiomeTotalCount.Clear();
    BiomeExploredCount.Clear();

    foreach (Heightmap.Biome biome in GetHeightmapBiomes()) {
      BiomeTotalCount[biome] = 0;
      BiomeExploredCount[biome] = 0;
    }
  }



  public static Heightmap.Biome[] GetHeightmapBiomes() {
    return (Heightmap.Biome[]) Enum.GetValues(typeof(Heightmap.Biome));
  }
}
