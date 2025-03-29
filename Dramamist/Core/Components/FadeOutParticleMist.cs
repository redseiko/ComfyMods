namespace Dramamist;

using ComfyLib;

using UnityEngine;

public sealed class FadeOutParticleMist : MonoBehaviour {
  Demister _demister;
  Collider _collider;

  void Awake() {
    _demister = GetComponentInChildren<Demister>();

    _collider =
        _demister.m_forceField.GetOrAddComponent<SphereCollider>()
            .SetRadius(_demister.m_forceField.endRange)
            .SetIsTrigger(true);

    ParticleMistManager.UpdateDemisterSettings(_demister);
  }

  void OnEnable() {
    _collider.SetEnabled(true);
    ParticleMist.m_instance.m_ps.trigger.AddCollider(_collider);
  }

  void OnDisable() {
    _collider.SetEnabled(false);
    ParticleMist.m_instance.m_ps.trigger.RemoveCollider(_collider);
  }
}
