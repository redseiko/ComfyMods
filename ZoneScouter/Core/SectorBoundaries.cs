namespace ZoneScouter;

using System.Collections;
using System.Collections.Generic;

using ComfyLib;

using UnityEngine;

using static PluginConfig;

public static class SectorBoundaries {
  static Coroutine _updateBoundaryCubeCoroutine;

  static GameObject _boundaryCube;
  static readonly List<MeshRenderer> _boundaryWallRendererCache = [];

  public static void ToggleSectorBoundaries() {
    TearDown();
    StartUp();
  }

  public static void SetBoundaryColor(Color targetColor) {
    if (IsModEnabled.Value && _boundaryCube) {
      foreach (MeshRenderer renderer in _boundaryWallRendererCache) {
        renderer.material.color = targetColor;
      }
    }
  }

  static void TearDown() {
    if (_updateBoundaryCubeCoroutine != null && Hud.m_instance) {
      Hud.m_instance.StopCoroutine(_updateBoundaryCubeCoroutine);
    }

    _updateBoundaryCubeCoroutine = null;

    if (_boundaryCube) {
      UnityEngine.Object.Destroy(_boundaryCube);
    }

    _boundaryCube = null;
    _boundaryWallRendererCache.Clear();
  }

  static void StartUp() {
    if (IsModEnabled.Value && ShowSectorBoundaries.Value && Hud.m_instance) {
      _boundaryCube = CreateBoundaryCube();
      _boundaryWallRendererCache.AddRange(_boundaryCube.GetComponentsInChildren<MeshRenderer>());
      _updateBoundaryCubeCoroutine = Hud.m_instance.StartCoroutine(UpdateBoundaryCubeCoroutine());
    }
  }

  static IEnumerator UpdateBoundaryCubeCoroutine() {
    WaitForSeconds waitInterval = new(seconds: 1f);

    while (true) {
      yield return waitInterval;

      if (!ZoneSystem.s_instance || !Player.m_localPlayer) {
        continue;
      }

      Vector3 position = Player.m_localPlayer.transform.position;
      Vector2s sector = ZoneSystem.GetZone(position);

      Vector3 cubePosition = ZoneSystem.GetZonePos(sector);
      cubePosition.y = position.y;

      _boundaryCube.transform.position = cubePosition;
    }
  }

  static GameObject CreateBoundaryCube() {
    GameObject cube = new("BoundaryCube");
    cube.transform.position = Vector3.zero;

    CreateBoundaryCubeWall(cube, new(32f, -256f, 0f), new(0.1f, 1024f, 64f));
    CreateBoundaryCubeWall(cube, new(-32f, -256f, 0f), new(0.1f, 1024f, 64f));
    CreateBoundaryCubeWall(cube, new(0f, -256f, 32f), new(64f, 1024f, 0.1f));
    CreateBoundaryCubeWall(cube, new(0f, -256f, -32f), new(64f, 1024f, 0.1f));

    return cube;
  }

  static GameObject CreateBoundaryCubeWall(GameObject cube, Vector3 position, Vector3 scale) {
    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
    wall.name = "BoundaryCube.Wall";

    wall.transform.SetParent(cube.transform, worldPositionStays: false);
    wall.transform.localPosition = position;
    wall.transform.localScale = scale;

    MeshRenderer renderer = wall.GetComponent<MeshRenderer>();
    renderer.material.shader = AssetUtils.DistortionShader;
    renderer.material.color = SectorBoundaryColor.Value;

    UnityEngine.Object.Destroy(wall.GetComponentInChildren<Collider>());

    return wall;
  }
}
