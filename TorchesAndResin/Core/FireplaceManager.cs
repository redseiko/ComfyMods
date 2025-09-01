namespace TorchesAndResin;

using System.Collections.Generic;

using static PluginConfig;

public static class FireplaceManager {
  public static readonly HashSet<int> EligibleFireplaceHashCodes = [
    "Candle_resin".GetStableHashCode(),
    "fire_pit_iron".GetStableHashCode(),
    "piece_groundtorch".GetStableHashCode(),
    "piece_groundtorch_wood".GetStableHashCode(),
    "piece_walltorch".GetStableHashCode(),
    "piece_brazierceiling01".GetStableHashCode(),
    "piece_brazierfloor01".GetStableHashCode(),
  ];

  public static void SetEligibleFireplaceFuel(Fireplace fireplace, float startingFuel) {
    ZNetView netView = fireplace.m_nview;
    ZDO zdo = netView.m_zdo;

    if (!EligibleFireplaceHashCodes.Contains(zdo.m_prefab)) {
      return;
    }

    fireplace.m_startFuel = startingFuel;

    if (!netView.IsOwner()) {
      return;
    }

    if (!zdo.GetFloat(ZDOVars.s_fuel, out float currentFuel) || currentFuel < startingFuel) {
      zdo.Set(ZDOVars.s_fuel, startingFuel);
    }

    if (fireplace.m_canTurnOff
        && CandleAlwaysToggleOn.Value
        && zdo.GetInt(ZDOVars.s_state, out int fireplaceState)
        && fireplaceState != 1) {
      zdo.Set(ZDOVars.s_state, 1);
    }
  }
}
