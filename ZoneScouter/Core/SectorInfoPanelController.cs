namespace ZoneScouter;

using System.Collections;

using ComfyLib;

using UnityEngine;

using static PluginConfig;

public static class SectorInfoPanelController {
  static Coroutine _updateSectorInfoPanelCoroutine;
  static Coroutine _updateSectorZDOCountGridCoroutine;

  public static SectorInfoPanel SectorInfoPanel { get; private set; }

  public static bool HasValidSectorInfoPanel() {
    return SectorInfoPanel?.Panel;
  }

  public static SectorZdoCountGrid SectorZDOCountGrid { get; private set; }

  public static bool HasValidSectorZDOCountGrid() {
    return SectorZDOCountGrid?.Grid;
  }

  public static void ToggleSectorInfoPanel() {
    if (_updateSectorInfoPanelCoroutine != null) {
      Hud.m_instance.Ref().StopCoroutine(_updateSectorInfoPanelCoroutine);
      _updateSectorInfoPanelCoroutine = null;
    }

    if (HasValidSectorInfoPanel()) {
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

    ToggleSectorZDOCountGrid();
  }

  public static void OnSectorInfoPanelEndDrag(object sender, Vector3 position) {
    SectorInfoPanelPosition.Value = position;
  }

  public static void ToggleSectorContent(bool toggleOn) {
    if (HasValidSectorInfoPanel()) {
      SectorInfoPanel.ToggleSectorContent(toggleOn);
    }
  }

  public static void ToggleZDOManagerContent(bool toggleOn) {
    if (HasValidSectorInfoPanel()) {
      SectorInfoPanel.ToggleZDOManagerContent(toggleOn);
    }
  }

  public static void ToggleMenuComponents(bool toggleOn) {
    if (HasValidSectorInfoPanel()) {
      SectorInfoPanel.ToggleMenuComponents(toggleOn);
    }
  }

  public static void ToggleSectorZDOCountGrid() {
    if (_updateSectorZDOCountGridCoroutine != null && Hud.m_instance) {
      Hud.m_instance.StopCoroutine(_updateSectorZDOCountGridCoroutine);
      _updateSectorZDOCountGridCoroutine = null;
    }

    if (HasValidSectorZDOCountGrid()) {
      UnityEngine.Object.Destroy(SectorZDOCountGrid.Grid);
      SectorZDOCountGrid = null;
    }

    if (IsModEnabled.Value && ShowSectorInfoPanel.Value && ShowSectorZDOCountGrid.Value && HasValidSectorInfoPanel()) {
      SectorZDOCountGrid = new(SectorInfoPanel.Panel.transform, SectorZDOCountGridSize.Value);
      SectorZDOCountGrid.Grid.SetActive(true);

      _updateSectorZDOCountGridCoroutine = Hud.m_instance.StartCoroutine(UpdateSectorZDOCountGrid());
    }
  }

  static long GetSectorZDOCount(Vector2s sector) {
    uint index = ZoneSystem.SectorToIndex(sector).Sector;

    return ZDOMan.s_instance.m_objectsBySector[index]?.Count ?? 0L;
  }

  static IEnumerator UpdateSectorInfoPanelCoroutine() {
    WaitForSeconds waitInterval = new(seconds: 0.25f);

    Vector3Int lastPosition = Vector3Int.zero;
    Vector2s lastSector = new(short.MinValue, short.MaxValue);

    long lastZDOCount = long.MaxValue;
    uint lastNextUid = uint.MaxValue;

    while (true) {
      yield return waitInterval;

      if (!ZoneSystem.s_instance || !Player.m_localPlayer || !HasValidSectorInfoPanel()) {
        continue;
      }

      uint nextUid = ZDOMan.s_instance.m_nextUid;

      if (nextUid != lastNextUid) {
        SectorInfoPanel.ZDOManagerNextId.Value.SetText($"{nextUid:D}");
        lastNextUid = nextUid;
      }

      Vector3Int position = Vector3Int.FloorToInt(Player.m_localPlayer.transform.position);

      if (position != lastPosition) {
        lastPosition = position;
        SectorInfoPanel.PositionX.Value.SetText($"{position.x}");
        SectorInfoPanel.PositionY.Value.SetText($"{position.y}");
        SectorInfoPanel.PositionZ.Value.SetText($"{position.z}");
      }

      Vector2s currentSector = ZoneSystem.GetZone(position);
      long currentZDOCount = GetSectorZDOCount(currentSector);

      if (currentSector == lastSector && currentZDOCount == lastZDOCount) {
        continue;
      }

      lastSector = currentSector;
      lastZDOCount = currentZDOCount;

      SectorInfoPanel.SectorXY.Value.SetText($"{currentSector.x},{currentSector.y}");
      SectorInfoPanel.SectorZDOCount.Value.SetText($"{currentZDOCount}");
    }
  }

  static IEnumerator UpdateSectorZDOCountGrid() {
    if (!HasValidSectorZDOCountGrid()) {
      yield break;
    }

    WaitForSeconds waitInterval = new(seconds: 2f);

    int size = SectorZDOCountGrid.Size;
    int offset = Mathf.FloorToInt(size / 2f);

    while (HasValidSectorZDOCountGrid()) {
      if (!Player.m_localPlayer) {
        yield return waitInterval;
        continue;
      }

      Vector2s sector = ZoneSystem.GetZone(Player.m_localPlayer.transform.position);

      for (int i = 0; i < size; i++) {
        for (int j = 0; j < size; j++) {
          Vector2s cellSector = new(sector.x + i - offset, sector.y - j + offset);
          SectorZdoCountCell cell = SectorZDOCountGrid.Cells[j, i];

          cell.ZdoCount.SetText($"{GetSectorZDOCount(cellSector)}");
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
