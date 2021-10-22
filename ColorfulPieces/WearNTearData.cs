using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace ColorfulPieces {
  public class WearNTearData {
    public long LastDataRevision { get; set; } = -1L;
    public List<Material> Materials { get; } = new List<Material>();
    public Color TargetColor { get; set; } = Color.clear;
    public float TargetEmissionColorFactor { get; set; } = 0f;

    public WearNTearData(WearNTear wearNTear) {
      Materials.AddRange(wearNTear.GetComponentsInChildren<MeshRenderer>(true).SelectMany(r => r.materials));
      Materials.AddRange(wearNTear.GetComponentsInChildren<SkinnedMeshRenderer>(true).SelectMany(r => r.materials));

      foreach (Material material in Materials) {
        SaveMaterialColors(material);
      }
    }

    private static void SaveMaterialColors(Material material) {
      if (material.HasProperty("_Color")) {
        material.SetColor("_SavedColor", material.GetColor("_Color"));
      }

      if (material.HasProperty("_EmissionColor")) {
        material.SetColor("_SavedEmissionColor", material.GetColor("_EmissionColor"));
      }
    }
  }
}