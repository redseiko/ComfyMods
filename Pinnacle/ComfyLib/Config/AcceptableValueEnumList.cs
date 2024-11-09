namespace ComfyLib;

using System;
using System.Linq;

using BepInEx.Configuration;

public sealed class AcceptableValueEnumList<T> : AcceptableValueBase where T : Enum {
  public T[] AcceptableValues { get; }

  public AcceptableValueEnumList(params T[] acceptableValues) : base(typeof(T)) {
    if (acceptableValues == null) {
      throw new ArgumentNullException(nameof(acceptableValues));
    }

    if (acceptableValues.Length == 0) {
      throw new ArgumentException("At least one acceptable value is needed.", nameof(acceptableValues));
    }

    AcceptableValues = acceptableValues;
  }

  public override object Clamp(object value) {
    if (IsValid(value)) {
      return value;
    }

    return AcceptableValues[0];
  }

  public override bool IsValid(object value) {
    if (value is not T) {
      return false;
    }

    for (int i = 0; i < AcceptableValues.Length; i++) {
      if (AcceptableValues[i].Equals(value)) {
        return true;
      }
    }

    return false;
  }

  public override string ToDescriptionString() {
    return "# Acceptable values: " + string.Join(", ", AcceptableValues.Select(GetEnumName));
  }

  public string GetEnumName(T value) => Enum.GetName(typeof(T), value);
}
