namespace Meishi;

using BepInEx.Logging;

using ComfyLib;

using UnityEngine;

public static class MeishiController {
  static ManualLogSource _logger;

  static Transform _uiParent;

  public static void Initialize(ManualLogSource logger) {
    _logger = logger;
  }

  public static void RegisterParent(Transform parent) {
    _uiParent = parent;
  }

  public static void CreateTestCard() {
    _logger?.LogInfo("Meishi: Creating Test Card...");
    
    if (_uiParent == null) {
      _logger?.LogError("Meishi: UI Parent not registered!");
      return;
    }

    // Check if valid
    if (!_uiParent.gameObject.activeInHierarchy) {
       _logger?.LogWarning("Meishi: Parent is not active, card might not be visible.");
    }

    MeishiCardData data = MeishiMock.GenerateRandom();

    MeishiCard card = new(parentTransform: _uiParent);
    card.SetData(data);
    
    // Position randomly
    card.RectTransform.SetPosition(new Vector2(Random.Range(-200, 200), Random.Range(-100, 100)));
  }

  public static void CreateTestButton(Transform parent) {
    ComfyLib.LabelButton button = new ComfyLib.LabelButton(parent);
    
    button.RectTransform
        .SetAnchorMin(new Vector2(1, 1))
        .SetAnchorMax(new Vector2(1, 1))
        .SetPivot(new Vector2(1, 1))
        .SetPosition(new Vector2(-50, -50))
        .SetSizeDelta(new Vector2(150, 40));
        
    button.Label.text = "Test Meishi";
    button.AddOnClickListener(CreateTestCard);
  }
}
