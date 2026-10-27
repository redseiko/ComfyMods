namespace Pinnacle;

using System;
using System.Collections.Generic;

using UnityEngine;

public static class PinSearchFilter {
  public static List<Minimap.PinData> CurrentPins { get; } = [];
  public static List<Minimap.PinData> FilteredPins { get; } = [];

  static readonly List<Minimap.PinData> _searchMatches = [];
  static string _previousSearchTerm = string.Empty;

  public static void Reset() {
    CurrentPins.Clear();
    FilteredPins.Clear();

    _searchMatches.Clear();
    _previousSearchTerm = string.Empty;
  }

  public static void ResetSearch() {
    _previousSearchTerm = string.Empty;
  }

  public static void ResetFilteredPins() {
    if (FilteredPins.Capacity < CurrentPins.Count) {
      FilteredPins.Capacity = CurrentPins.Count;
    }

    FilteredPins.Clear();
    FilteredPins.AddRange(CurrentPins);

    _previousSearchTerm = string.Empty;
  }

  public static void SetSourcePins(IEnumerable<Minimap.PinData> pins) {
    CurrentPins.Clear();
    CurrentPins.AddRange(pins);
    CurrentPins.Sort(ComparePins);

    for (int i = 0, count = CurrentPins.Count; i < count; i++) {
      Minimap.PinData pin = CurrentPins[i];

      if (Mathf.Approximately(pin.m_pos.y, 0f)) {
        pin.m_pos.y = PinnacleUtils.GetHeight(pin.m_pos);
      }
    }

    ResetSearch();
    ResetFilteredPins();
  }

  public static int ComparePins(Minimap.PinData a, Minimap.PinData b) {
    int typeComparison = a.m_type.CompareTo(b.m_type);

    if (typeComparison != 0) {
      return typeComparison;
    }

    return string.Compare(a.m_name, b.m_name, StringComparison.InvariantCultureIgnoreCase);
  }

  public static bool TryFindFilteredPinIndex(Minimap.PinData pin, out int index) {
    if (pin == null) {
      index = -1;
      return false;
    }

    for (int i = 0, count = FilteredPins.Count; i < count; i++) {
      if (FilteredPins[i] == pin) {
        index = i;
        return true;
      }
    }

    index = -1;
    return false;
  }

  public static void ApplySearch(string term) {
    if (string.IsNullOrEmpty(term)) {
      ResetFilteredPins();
      return;
    }

    string normalizedTerm = term.Trim().ToLowerInvariant();

    if (string.IsNullOrEmpty(normalizedTerm)) {
      ResetFilteredPins();
      return;
    }

    if (normalizedTerm == _previousSearchTerm) {
      return;
    }

    Span<Range> tokenRanges = stackalloc Range[16];
    int tokenCount = ParseQueryTokens(normalizedTerm.AsSpan(), tokenRanges);

    if (tokenCount == 0) {
      ResetFilteredPins();
      return;
    }

    if (FilteredPins.Capacity < CurrentPins.Count) {
      FilteredPins.Capacity = CurrentPins.Count;
    }

    List<Minimap.PinData> sourcePins =
        (!string.IsNullOrEmpty(_previousSearchTerm)
                && normalizedTerm.StartsWith(_previousSearchTerm, StringComparison.Ordinal))
            ? FilteredPins
            : CurrentPins;

    FilterPins(sourcePins, normalizedTerm.AsSpan(), tokenRanges[..tokenCount]);
    _previousSearchTerm = normalizedTerm;
  }

  static void FilterPins(
      List<Minimap.PinData> sourcePins, ReadOnlySpan<char> searchTerm, ReadOnlySpan<Range> tokens) {
    _searchMatches.Clear();

    if (_searchMatches.Capacity < sourcePins.Count) {
      _searchMatches.Capacity = sourcePins.Count;
    }

    for (int i = 0, count = sourcePins.Count; i < count; i++) {
      Minimap.PinData pin = sourcePins[i];

      if (MatchesSearch(pin, searchTerm, tokens)) {
        _searchMatches.Add(pin);
      }
    }

    FilteredPins.Clear();
    FilteredPins.AddRange(_searchMatches);
    _searchMatches.Clear();
  }

  static int ParseQueryTokens(ReadOnlySpan<char> query, Span<Range> destination) {
    int tokenCount = 0;
    int maxTokens = destination.Length;
    int start = 0;
    int length = query.Length;

    while (start < length && tokenCount < maxTokens) {
      while (start < length && char.IsWhiteSpace(query[start])) {
        start++;
      }

      if (start >= length) {
        break;
      }

      int end = start;

      while (end < length && !char.IsWhiteSpace(query[end])) {
        end++;
      }

      destination[tokenCount++] = start..end;
      start = end;
    }

    return tokenCount;
  }

  static bool MatchesSearch(Minimap.PinData pin, ReadOnlySpan<char> searchTerm, ReadOnlySpan<Range> tokens) {
    string name = GetPinSearchName(pin);
    ReadOnlySpan<char> nameSpan = name.AsSpan();

    for (int i = 0, count = tokens.Length; i < count; i++) {
      ReadOnlySpan<char> token = searchTerm[tokens[i]];

      if (!nameSpan.Contains(token, StringComparison.OrdinalIgnoreCase)) {
        return false;
      }
    }

    return true;
  }

  static string GetPinSearchName(Minimap.PinData pin) {
    if (string.IsNullOrEmpty(pin.m_name)) {
      return PinNameUtils.GetFallbackPinName(pin);
    }

    if (pin.m_name[0] == '$' && Localization.m_instance != null) {
      return Localization.m_instance.Localize(pin.m_name);
    }

    return pin.m_name;
  }
}
