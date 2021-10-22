using System.Collections.Generic;

using UnityEngine;

namespace ColorfulLights {
  public class FireplaceData {
    public List<Light> Lights { get; } = new List<Light>();
    public List<ParticleSystem> Systems { get; } = new List<ParticleSystem>();
    public List<ParticleSystemRenderer> Renderers { get; } = new List<ParticleSystemRenderer>();
    public Color TargetColor { get; set; } = Color.clear;

    public FireplaceData(Fireplace fireplace) {
      ExtractFireplaceData(fireplace.m_enabledObject);
      ExtractFireplaceData(fireplace.m_enabledObjectHigh);
      ExtractFireplaceData(fireplace.m_enabledObjectLow);
      ExtractFireplaceData(fireplace.m_fireworks);
    }

    void ExtractFireplaceData(GameObject targetObject) {
      if (targetObject) {
        Lights.AddRange(targetObject.GetComponentsInChildren<Light>(includeInactive: true));
        Systems.AddRange(targetObject.GetComponentsInChildren<ParticleSystem>(includeInactive: true));
        Renderers.AddRange(targetObject.GetComponentsInChildren<ParticleSystemRenderer>(includeInactive: true));
      }
    }
  }
}
