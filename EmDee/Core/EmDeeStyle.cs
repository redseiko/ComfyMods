namespace EmDee;

using UnityEngine;

public sealed class EmDeeStyle {
  public Color TextColor { get; set; } = Color.white;
  
  public float H1Size { get; set; } = 32f;
  public float H2Size { get; set; } = 28f;
  public float H3Size { get; set; } = 24f;
  public float BodySize { get; set; } = 18f;
  public float CodeFontSize { get; set; } = 16f;

  public Color CodeTextColor { get; set; } = new Color(0.8f, 0.8f, 0.8f);
  public Color CodeBlockBackgroundColor { get; set; } = new Color(0.1f, 0.1f, 0.1f, 0.5f);
}
