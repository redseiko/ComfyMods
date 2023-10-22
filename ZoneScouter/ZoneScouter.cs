using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;

using ComfyLib;

using HarmonyLib;

using UnityEngine;

using static ZoneScouter.PluginConfig;

namespace ZoneScouter {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class ZoneScouter : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.zonescouter";
    public const string PluginName = "ZoneScouter";
    public const string PluginVersion = "1.3.0";

    Harmony _harmony;

    void Awake() {
      BindConfig(Config);

      SectorInfoPanelBackgroundColor.OnSettingChanged(color => SectorInfoPanel?.Panel.Ref()?.Image().SetColor(color));
      SectorInfoPanelPosition.OnSettingChanged(
          position => SectorInfoPanel?.Panel.Ref()?.RectTransform().SetPosition(position));

      SectorInfoPanelFontSize.OnSettingChanged(() => SectorInfoPanel?.SetPanelStyle());
      PositionValueXTextColor.OnSettingChanged(() => SectorInfoPanel?.SetPanelStyle());
      PositionValueYTextColor.OnSettingChanged(() => SectorInfoPanel?.SetPanelStyle());
      PositionValueZTextColor.OnSettingChanged(() => SectorInfoPanel?.SetPanelStyle());

      ShowSectorZdoCountGrid.OnSettingChanged(ToggleSectorZdoCountGrid);
      SectorZdoCountGridSize.OnSettingChanged(ToggleSectorZdoCountGrid);

      CellZdoCountBackgroundImageColor.OnSettingChanged(() => SectorZdoCountGrid?.SetCellStyle());
      CellZdoCountTextColor.OnSettingChanged(() => SectorZdoCountGrid?.SetCellStyle());
      CellZdoCountTextFontSize.OnSettingChanged(() => SectorZdoCountGrid?.SetCellStyle());

      CellSectorBackgroundImageColor.OnSettingChanged(() => SectorZdoCountGrid?.SetCellStyle());
      CellSectorTextColor.OnSettingChanged(() => SectorZdoCountGrid?.SetCellStyle());
      CellSectorTextFontSize.OnSettingChanged(() => SectorZdoCountGrid?.SetCellStyle());

      ShowSectorBoundaries.OnSettingChanged(SectorBoundaries.ToggleSectorBoundaries);
      SectorBoundaryColor.OnSettingChanged(SectorBoundaries.SetBoundaryColor);

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
    }

    void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    static readonly Dictionary<Vector2i, int> SectorToIndexCache = new();

    static long GetSectorZdoCount(Vector2i sector) {
      if (!SectorToIndexCache.TryGetValue(sector, out int index)) {
        index = ZDOMan.s_instance.SectorToIndex(sector);
        SectorToIndexCache[sector] = index;
      }

      return index >= 0
          ? ZDOMan.s_instance.m_objectsBySector[index]?.Count ?? 0L
          : 0L;
    }

    public static SectorInfoPanel SectorInfoPanel { get; private set; }
    static Coroutine _updateSectorInfoPanelCoroutine;

    public static void ToggleSectorInfoPanel() {
      if (_updateSectorInfoPanelCoroutine != null) {
        Hud.m_instance.Ref().StopCoroutine(_updateSectorInfoPanelCoroutine);
        _updateSectorInfoPanelCoroutine = null;
      }

      if (SectorInfoPanel?.Panel) {
        Destroy(SectorInfoPanel.Panel);
        SectorInfoPanel = null;
      }

      if (IsModEnabled.Value && ShowSectorInfoPanel.Value && Hud.m_instance) {
        SectorInfoPanel = new(Hud.m_instance.transform);

        SectorInfoPanel.Panel.RectTransform()
            .SetAnchorMin(new(0.5f, 1f))
            .SetAnchorMax(new(0.5f, 1f))
            .SetPivot(new(0.5f, 1f))
            .SetPosition(SectorInfoPanelPosition.Value)
            .SetSizeDelta(new(200f, 200f))
            .SetAsFirstSibling();

        SectorInfoPanel.PanelDragger.OnEndDragAction = position => SectorInfoPanelPosition.Value = position;

        SectorInfoPanel.Panel.SetActive(true);
        _updateSectorInfoPanelCoroutine = Hud.m_instance.StartCoroutine(UpdateSectorInfoPanelCoroutine());
      }

      ToggleSectorZdoCountGrid();
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

        Vector2i sector = ZoneSystem.m_instance.GetZone(position);
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

    public static SectorZdoCountGrid SectorZdoCountGrid { get; private set; }
    static Coroutine _updateSectorZdoCountGridCoroutine;

    public static void ToggleSectorZdoCountGrid() {
      if (_updateSectorZdoCountGridCoroutine != null && Hud.m_instance) {
        Hud.m_instance.StopCoroutine(_updateSectorZdoCountGridCoroutine);
        _updateSectorZdoCountGridCoroutine = null;
      }

      if (SectorZdoCountGrid?.Grid) {
        Destroy(SectorZdoCountGrid.Grid);
        SectorZdoCountGrid = null;
      }

      if (IsModEnabled.Value && ShowSectorInfoPanel.Value && ShowSectorZdoCountGrid.Value && SectorInfoPanel?.Panel) {
        SectorZdoCountGrid = new(SectorInfoPanel.Panel.transform, SectorZdoCountGridSize.Value);
        SectorZdoCountGrid.Grid.SetActive(true);

        _updateSectorZdoCountGridCoroutine = Hud.m_instance.StartCoroutine(UpdateSectorZdoCountGrid());
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

        Vector2i sector = ZoneSystem.m_instance.GetZone(Player.m_localPlayer.transform.position);

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
  }
}
