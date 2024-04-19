namespace PotteryBarn;

using System;

using UnityEngine;

public static class PlacementGhostUtils {
  public static bool HasActiveComponents(GameObject gameObject) {
    return gameObject.TryGetComponent(out MonsterAI _)
        || gameObject.TryGetComponent(out AnimalAI _)
        || gameObject.TryGetComponent(out Tameable _)
        || gameObject.TryGetComponent(out Ragdoll _)
        || gameObject.TryGetComponent(out Humanoid _);
  }

  public static void DestroyActiveComponents(GameObject gameObject) {
    if (gameObject.TryGetComponent(out MonsterAI monsterAi)) {
      UnityEngine.Object.DestroyImmediate(monsterAi);
    }

    if (gameObject.TryGetComponent(out AnimalAI animalAi)) {
      UnityEngine.Object.DestroyImmediate(animalAi);
    }

    if (gameObject.TryGetComponent(out Tameable tameable)) {
      UnityEngine.Object.DestroyImmediate(tameable);
    }

    if (gameObject.TryGetComponent(out Ragdoll ragdoll)) {
      UnityEngine.Object.DestroyImmediate(ragdoll);
    }

    if (gameObject.TryGetComponent(out Humanoid humanoid)) {
      humanoid.m_defaultItems ??= Array.Empty<GameObject>();
      humanoid.m_randomWeapon ??= Array.Empty<GameObject>();
      humanoid.m_randomArmor ??= Array.Empty<GameObject>();
      humanoid.m_randomShield ??= Array.Empty<GameObject>();
      humanoid.m_randomSets ??= Array.Empty<Humanoid.ItemSet>();
    }
  }
}
