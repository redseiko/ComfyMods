namespace Meishi;

using System.Collections.Generic;
using UnityEngine;

public static class MeishiMock {
  public static MeishiCardData GenerateRandom() {
    MeishiCardData data = new() {
      PlayerName = "Thor's Chosen",
      Title = "Viking Legend",
      BackgroundId = "woodpanel_info",
      BadgeIds = ["boss_eikthyr", "boss_elder", "boss_bonemass"]
    };

    // Randomize for testing
    if (Random.value > 0.5f) {
      data.PlayerName = "Odin's Warrior";
      data.Title = "The Wanderer";
      data.BadgeIds.Add("boss_moder");
    }

    return data;
  }
}
