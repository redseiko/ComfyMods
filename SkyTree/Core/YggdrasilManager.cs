namespace SkyTree;

using ComfyLib;

using UnityEngine;

public static class YggdrasilManager {
  public const int SkyboxLayer = 19;
  public const int StaticSolidLayer = 15;

  public static GameObject YggdrasilRoot { get; private set; }
  public static GameObject YggdrasilBranch { get; private set; }
  public static MeshCollider YggdrasilBranchCollider { get; private set; }

  public static void Reset() {
    YggdrasilRoot = default;
    YggdrasilBranch = default;
    YggdrasilBranchCollider = default;
  }

  public static void SetYggdrasilLayer(int layer) {
    YggdrasilRoot.layer = layer;
    YggdrasilBranch.layer = layer;
  }

  public static void ToggleYggdrasil(bool toggleOn) {
    if (toggleOn) {
      SetYggdrasilSolid(EnvMan.s_instance);
    } else {
      SetYggdrasilSkybox();
    }
  }

  public static void SetYggdrasilSolid(EnvMan environmentManager) {
    if (!environmentManager) {
      return;
    }

    YggdrasilRoot = environmentManager.transform.Find("YggdrasilBranch").gameObject;
    YggdrasilBranch = YggdrasilRoot.transform.Find("branch").gameObject;

    string targetLayerName = $"{StaticSolidLayer}:{LayerMask.LayerToName(StaticSolidLayer)}";

    int sourceLayer = YggdrasilRoot.layer;
    string sourceLayerName = $"{sourceLayer}:{LayerMask.LayerToName(sourceLayer)}";

    SkyTree.LogInfo($"Setting YggdrasilBranch layer from {sourceLayerName} to {targetLayerName}.");
    YggdrasilRoot.layer = StaticSolidLayer;

    sourceLayer = YggdrasilBranch.layer;
    sourceLayerName = $"{sourceLayer}:{LayerMask.LayerToName(sourceLayer)}";

    SkyTree.LogInfo($"Setting YggdrasilBranch/branch layer from {sourceLayerName} to {targetLayerName}.");
    YggdrasilBranch.layer = StaticSolidLayer;

    if (YggdrasilBranch.TryGetComponent(out MeshFilter filter)
        && !YggdrasilBranch.TryGetComponent(out MeshCollider _)) {
      SkyTree.LogInfo("Adding collider to YggdrasilBranch/branch.");

      YggdrasilBranchCollider = YggdrasilBranch.gameObject.AddComponent<MeshCollider>();

      YggdrasilBranchCollider.cookingOptions =
          MeshColliderCookingOptions.CookForFasterSimulation
          | MeshColliderCookingOptions.EnableMeshCleaning
          | MeshColliderCookingOptions.UseFastMidphase
          | MeshColliderCookingOptions.WeldColocatedVertices;

      YggdrasilBranchCollider.sharedMesh = filter.sharedMesh;
    }
  }

  public static void SetYggdrasilSkybox() {
    if (YggdrasilBranch) {
      YggdrasilBranch.layer = SkyboxLayer;

      if (YggdrasilBranch.TryGetComponent(out MeshCollider collider)) {
        UnityEngine.Object.Destroy(collider);
      }
    }

    if (YggdrasilRoot) {
      YggdrasilRoot.layer = SkyboxLayer;
    }
  }

  public static bool IsBlockedDelegate(int blockRayMask, Vector3 point) {
    point.y += 2000f;

    if (!Physics.Raycast(point, Vector3.down, out RaycastHit raycastHit, 10000f, blockRayMask)) {
      return false;
    }

    if (raycastHit.collider != YggdrasilBranchCollider) {
      return true;
    }

    point.y = Mathf.Min(raycastHit.point.y - 0.1f, 1000f);

    return Physics.Raycast(point, Vector3.down, 10000f, blockRayMask);
  }
}
