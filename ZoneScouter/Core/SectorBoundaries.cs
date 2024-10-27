namespace ZoneScouter;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ComfyLib;

using UnityEngine;

using static PluginConfig;

public static class SectorBoundaries {
  static readonly Lazy<Shader> DistortionShader = new(() => Shader.Find("Custom/Distortion"));
  static readonly Vector2i UnsetSector = new(int.MaxValue, int.MaxValue);

  static Coroutine _updateBoundaryCubeCoroutine;
  static Vector2i _lastBoundarySector = UnsetSector;

  static GameObject _boundaryCube;
  static readonly List<MeshRenderer> _boundaryWallRendererCache = [];

  public static void ToggleSectorBoundaries() {
    TearDown();
    StartUp();
  }

  public static void SetBoundaryColor(Color targetColor) {
    if (IsModEnabled.Value && _boundaryCube) {
      foreach (MeshRenderer renderer in _boundaryWallRendererCache) {
        renderer.material.SetColor("_Color", targetColor);
      }
    }
  }

  static void TearDown() {
    if (_updateBoundaryCubeCoroutine != null && Hud.m_instance) {
      Hud.m_instance.StopCoroutine(_updateBoundaryCubeCoroutine);
    }

    _updateBoundaryCubeCoroutine = null;
    _lastBoundarySector = UnsetSector;

    if (_boundaryCube) {
      UnityEngine.Object.Destroy(_boundaryCube);
    }

    _boundaryCube = null;
    _boundaryWallRendererCache.Clear();
  }

  static void StartUp() {
    if (IsModEnabled.Value && ShowSectorBoundaries.Value && Hud.m_instance) {
      _boundaryCube = CreateBoundaryCube();

      _boundaryWallRendererCache.AddRange(
          _boundaryCube.Children().Select(child => child.GetComponent<MeshRenderer>()));

      _updateBoundaryCubeCoroutine = Hud.m_instance.StartCoroutine(UpdateBoundaryCubeCoroutine());
    }
  }

  static IEnumerator UpdateBoundaryCubeCoroutine() {
    WaitForSeconds waitInterval = new(seconds: 1f);

    while (true) {
      if (!ZoneSystem.m_instance || !Player.m_localPlayer) {
        _lastBoundarySector = UnsetSector;
        yield return waitInterval;
        continue;
      }

      Vector2i sector = ZoneSystemUtils.GetZone(Player.m_localPlayer.transform.position);

      if (sector != _lastBoundarySector) {
        _boundaryCube.transform.position = ZoneSystemUtils.GetZonePos(sector);
        _lastBoundarySector = sector;
      }

      yield return waitInterval;
    }
  }

  static GameObject CreateBoundaryCube() {
    GameObject cube = new("BoundaryCube");
    cube.transform.position = Vector3.zero;

    CreateBoundaryCubeWall(cube, new(32f, 256f, 0f), new(0.1f, 512f, 64f));
    CreateBoundaryCubeWall(cube, new(-32f, 256f, 0f), new(0.1f, 512f, 64f));
    CreateBoundaryCubeWall(cube, new(0f, 256f, 32f), new(64f, 512f, 0.1f));
    CreateBoundaryCubeWall(cube, new(0f, 256f, -32f), new(64f, 512f, 0.1f));
    
    CreateBoundaryCubeWall(cube, new(32f, 5000f, 0f), new(0.1f, 512f, 64f));
    CreateBoundaryCubeWall(cube, new(-32f, 5000f, 0f), new(0.1f, 512f, 64f));
    CreateBoundaryCubeWall(cube, new(0f, 5000f, 32f), new(64f, 512f, 0.1f));
    CreateBoundaryCubeWall(cube, new(0f, 5000f, -32f), new(64f, 512f, 0.1f));

    return cube;
  }

  static GameObject CreateBoundaryCubeWall(GameObject cube, Vector3 position, Vector3 scale) {
    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
    wall.name = "BoundaryCube.Wall";

    wall.transform.SetParent(cube.transform, worldPositionStays: false);
    wall.transform.localPosition = position;
    wall.transform.localScale = scale;

    MeshRenderer renderer = wall.GetComponent<MeshRenderer>();
    renderer.material.SetColor("_Color", SectorBoundaryColor.Value);
    renderer.material.shader = DistortionShader.Value;

    UnityEngine.Object.Destroy(wall.GetComponentInChildren<Collider>());

    return wall;
  }
}
