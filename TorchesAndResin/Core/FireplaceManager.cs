namespace TorchesAndResin;

using System.Collections.Generic;

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

    if (netView.IsOwner()
        && (!zdo.GetFloat(ZDOVars.s_fuel, out float currentFuel) || currentFuel < startingFuel)) {
      zdo.Set(ZDOVars.s_fuel, startingFuel);
    }
  }
}
