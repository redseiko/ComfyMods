namespace PotteryBarn;

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
      humanoid.m_defaultItems = [];
      humanoid.m_randomWeapon = [];
      humanoid.m_randomArmor = [];
      humanoid.m_randomShield = [];
      humanoid.m_randomSets = [];
    }
  }
}
