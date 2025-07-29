namespace ZoneScouter;

using System.Collections;
using System.Collections.Generic;

using ComfyLib;

using UnityEngine;

using static PluginConfig;

public static class SectorInfoPanelController {
  static readonly Dictionary<Vector2i, int> SectorToIndexCache = [];

  static Coroutine _updateSectorInfoPanelCoroutine;
  static Coroutine _updateSectorZdoCountGridCoroutine;

  public static SectorInfoPanel SectorInfoPanel { get; private set; }

  public static SectorZdoCountGrid SectorZdoCountGrid { get; private set; }

  public static void ToggleSectorInfoPanel() {
    if (_updateSectorInfoPanelCoroutine != null) {
      Hud.m_instance.Ref().StopCoroutine(_updateSectorInfoPanelCoroutine);
      _updateSectorInfoPanelCoroutine = null;
    }

    if (SectorInfoPanel?.Panel) {
      UnityEngine.Object.Destroy(SectorInfoPanel.Panel);
      SectorInfoPanel = null;
    }

    if (IsModEnabled.Value && ShowSectorInfoPanel.Value && Hud.m_instance) {
      SectorInfoPanel = new(Hud.m_instance.transform);

      SectorInfoPanel.RectTransform
          .SetAnchorMin(new(0.5f, 1f))
          .SetAnchorMax(new(0.5f, 1f))
          .SetPivot(new(0.5f, 1f))
          .SetPosition(SectorInfoPanelPosition.Value)
          .SetSizeDelta(new(200f, 200f))
          .SetAsFirstSibling();

      SectorInfoPanel.PanelDragger.OnPanelEndDrag += OnSectorInfoPanelEndDrag;
      SectorInfoPanel.CopyPositionButton.Button.onClick.AddListener(CopyPlayerPositionToClipboard);

      SectorInfoPanel.Panel.SetActive(true);
      _updateSectorInfoPanelCoroutine = Hud.m_instance.StartCoroutine(UpdateSectorInfoPanelCoroutine());
    }

    ToggleSectorZdoCountGrid();
  }

  public static void OnSectorInfoPanelEndDrag(object sender, Vector3 position) {
    SectorInfoPanelPosition.Value = position;
  }

  public static void ToggleSectorZdoCountGrid() {
    if (_updateSectorZdoCountGridCoroutine != null && Hud.m_instance) {
      Hud.m_instance.StopCoroutine(_updateSectorZdoCountGridCoroutine);
      _updateSectorZdoCountGridCoroutine = null;
    }

    if (SectorZdoCountGrid?.Grid) {
      UnityEngine.Object.Destroy(SectorZdoCountGrid.Grid);
      SectorZdoCountGrid = null;
    }

    if (IsModEnabled.Value && ShowSectorInfoPanel.Value && ShowSectorZdoCountGrid.Value && SectorInfoPanel?.Panel) {
      SectorZdoCountGrid = new(SectorInfoPanel.Panel.transform, SectorZdoCountGridSize.Value);
      SectorZdoCountGrid.Grid.SetActive(true);

      _updateSectorZdoCountGridCoroutine = Hud.m_instance.StartCoroutine(UpdateSectorZdoCountGrid());
    }
  }

  static long GetSectorZdoCount(Vector2i sector) {
    if (!SectorToIndexCache.TryGetValue(sector, out int index)) {
      index = ZDOMan.s_instance.SectorToIndex(sector);
      SectorToIndexCache[sector] = index;
    }

    return index >= 0
        ? ZDOMan.s_instance.m_objectsBySector[index]?.Count ?? 0L
        : 0L;
  }

  static IEnumerator UpdateSectorInfoPanelCoroutine() {
    WaitForSeconds waitInterval = new(seconds: 0.25f);
    Vector3 lastPosition = Vector3.positiveInfinity;
    Vector2i lastSector = new(int.MinValue, int.MaxValue);
    long lastZdoCount = long.MaxValue;
    uint lastNextUid = uint.MaxValue;

    while (true) {
      yield return waitInterval;

      if (!ZoneSystem.m_instance || !Player.m_localPlayer || !SectorInfoPanel?.Panel) {
        continue;
      }

      uint nextUid = ZDOMan.s_instance.m_nextUid;

      if (nextUid != lastNextUid) {
        SectorInfoPanel.ZdoManagerNextId.Value.SetText($"{nextUid:D}");
        lastNextUid = nextUid;
      }

      Vector3 position = Player.m_localPlayer.transform.position;

      if (position != lastPosition) {
        lastPosition = position;
        SectorInfoPanel.PositionX.Value.SetText($"{position.x:F0}");
        SectorInfoPanel.PositionY.Value.SetText($"{position.y:F0}");
        SectorInfoPanel.PositionZ.Value.SetText($"{position.z:F0}");
      }

      Vector2i sector = ZoneSystem.GetZone(position);
      long zdoCount = GetSectorZdoCount(sector);

      if (sector == lastSector && zdoCount == lastZdoCount) {
        continue;
      }

      lastSector = sector;
      lastZdoCount = zdoCount;

      SectorInfoPanel.SectorXY.Value.SetText($"{sector.x},{sector.y}");
      SectorInfoPanel.SectorZdoCount.Value.SetText($"{zdoCount}");
    }
  }

  static IEnumerator UpdateSectorZdoCountGrid() {
    if (!SectorZdoCountGrid?.Grid) {
      yield break;
    }

    WaitForSeconds waitInterval = new(seconds: 1f);

    int size = SectorZdoCountGrid.Size;
    int offset = Mathf.FloorToInt(size / 2f);

    while (SectorZdoCountGrid?.Grid) {
      if (!Player.m_localPlayer) {
        yield return waitInterval;
        continue;
      }

      Vector2i sector = ZoneSystem.GetZone(Player.m_localPlayer.transform.position);

      for (int i = 0; i < size; i++) {
        for (int j = 0; j < size; j++) {
          Vector2i cellSector = new(sector.x + i - offset, sector.y - j + offset);
          SectorZdoCountCell cell = SectorZdoCountGrid.Cells[j, i];

          cell.ZdoCount.SetText($"{GetSectorZdoCount(cellSector)}");
          cell.Sector.SetText($"{cellSector.x},{cellSector.y}");
        }
      }

      yield return waitInterval;
    }
  }

  public static void CopyPlayerPositionToClipboard() {
    if (Player.m_localPlayer) {
      CopyPositionToClipboard(Player.m_localPlayer.transform.position);
    }
  }

  public static void CopyPositionToClipboard(Vector3 targetPosition) {
    string prefix = CopyPositionValuePrefix.Value;

    string separator =
        CopyPositionValueSeparator.Value == PositionValueSeparator.Space
            ? " "
            : ",";

    string text =
        CopyPositionValueOrder.Value == PositionValueOrder.XYZ
            ? $"{prefix}{targetPosition.x:F0}{separator}{targetPosition.y:F0}{separator}{targetPosition.z:F0}"
            : $"{prefix}{targetPosition.x:F0}{separator}{targetPosition.z:F0}{separator}{targetPosition.y:F0}";

    GUIUtility.systemCopyBuffer = text;
    Chat.instance.AddMessage($"Copied to clipboard: {text}");
  }
}
