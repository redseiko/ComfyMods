namespace ReportCard;

using UnityEngine;

public sealed class MinimapFocus : MonoBehaviour {
  void Update() {
    Minimap.s_instance.m_wasFocused = true;
  }

  void OnDisable() {
    Minimap.s_instance.m_wasFocused = false;
  }

  void OnEnable() {
    Minimap.s_instance.m_wasFocused = true;
  }
}
