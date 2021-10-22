using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace ColorfulWards {
  public class PrivateAreaData {
    public List<Light> PointLight { get; } = new List<Light>();
    public List<Material> GlowMaterial { get; } = new List<Material>();
    public List<ParticleSystem> SparcsSystem { get; } = new List<ParticleSystem>();
    public List<ParticleSystemRenderer> SparcsRenderer { get; } = new List<ParticleSystemRenderer>();
    public List<ParticleSystem> FlareSystem { get; } = new List<ParticleSystem>();

    public Color TargetColor { get; set; } = Color.clear;

    public PrivateAreaData(PrivateArea privateArea) {
      PointLight.AddRange(FindChildren(privateArea, "Point light").Select(go => go.GetComponent<Light>()));
      GlowMaterial.AddRange(FindChildren(privateArea, "default").Select(go => go.GetComponent<Renderer>().material));

      foreach (GameObject sparcsObject in FindChildren(privateArea, "sparcs")) {
        SparcsSystem.Add(sparcsObject.GetComponent<ParticleSystem>());
        SparcsRenderer.Add(sparcsObject.GetComponent<ParticleSystemRenderer>());
      }

      FlareSystem.AddRange(FindChildren(privateArea, "flare").Select(go => go.GetComponent<ParticleSystem>()));
    }

    private IEnumerable<GameObject> FindChildren(PrivateArea privateArea, string name) {
      return privateArea
          .GetComponentsInChildren<Transform>()
          .Where(t => t.name == name && t.gameObject != null)
          .Select(t => t.gameObject);
    }
  }
}
