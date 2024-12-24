namespace ColorfulPalettes;

using ComfyLib;

using UnityEngine;

public sealed class ConfigSelectController {
  static ConfigSelectController _instance;

  public static ConfigSelectController Instance {
    get {
      if (!_instance?.ConfigSelect?.Panel) {
        ColorfulPalettes.LogInfo($"Creating new ConfigSelect.");
        _instance = new(UnifiedPopup.instance.gameObject.transform.parent);
        _instance.HideConfigSelect();
      }

      return _instance;
    }
  }

  public ConfigSelectPanel ConfigSelect { get; }
  public bool IsVisible { get; private set; }

  public ConfigSelectController(Transform parentTransform) {
    ConfigSelect = CreateConfigSelect(parentTransform);
  }

  ConfigSelectPanel CreateConfigSelect(Transform parentTransform) {
    ConfigSelectPanel configSelect = new(parentTransform);

    configSelect.RectTransform
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(400f, 500f));

    configSelect.CloseButton.Button.onClick.AddListener(HideConfigSelect);

    return configSelect;
  }

  public void ShowConfigSelect() {
    ConfigSelect.Panel.SetActive(true);
    IsVisible = true;
  }

  public void HideConfigSelect() {
    ConfigSelect.Panel.SetActive(false);
    IsVisible = false;
  }
}
