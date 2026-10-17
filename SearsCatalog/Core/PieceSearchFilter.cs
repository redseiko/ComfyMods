namespace SearsCatalog;

using System;
using System.Collections.Generic;
using System.Text;

using static PluginConfig;

public static class PieceSearchFilter {
  public static List<Piece> CurrentPieces { get; } = [];
  public static List<Piece> FilteredPieces { get; } = [];

  static readonly Dictionary<string, string> _cachedSearchStrings = [];
  static readonly List<Piece> _tempSearchMatches = [];
  static string _previousSearchTerm = string.Empty;

  public static void Initialize() {
    Localization.OnLanguageChange = (Action) Delegate.Remove(Localization.OnLanguageChange, new Action(ResetSearch));
    Localization.OnLanguageChange = (Action) Delegate.Combine(Localization.OnLanguageChange, new Action(ResetSearch));
  }

  public static void Destroy() {
    Localization.OnLanguageChange = (Action) Delegate.Remove(Localization.OnLanguageChange, new Action(ResetSearch));

    CurrentPieces.Clear();
    FilteredPieces.Clear();

    _tempSearchMatches.Clear();
    _cachedSearchStrings.Clear();
    _previousSearchTerm = string.Empty;
  }

  public static void ResetSearch() {
    _cachedSearchStrings.Clear();
    _previousSearchTerm = string.Empty;
  }

  public static void ResetSearchTerm() {
    _previousSearchTerm = string.Empty;
  }

  public static void ApplySearch(string term) {
    bool isVanillaMode = BuildUiSearchFilterMode.Value == SearchFilterMode.Vanilla;
    string normalizedTerm;
    Span<Range> tokenRanges = stackalloc Range[16];
    int tokenCount = 0;

    if (isVanillaMode) {
      normalizedTerm = term.Replace(" ", string.Empty).ToLowerInvariant();
    } else {
      normalizedTerm = term.Trim().ToLowerInvariant();
      tokenCount = ParseQueryTokens(normalizedTerm.AsSpan(), tokenRanges);
    }

    if (FilteredPieces.Capacity < CurrentPieces.Count) {
      FilteredPieces.Capacity = CurrentPieces.Count;
    }

    if (string.IsNullOrEmpty(normalizedTerm) || (!isVanillaMode && tokenCount == 0)) {
      FilteredPieces.Clear();
      FilteredPieces.AddRange(CurrentPieces);

      _previousSearchTerm = string.Empty;

      return;
    }

    List<Piece> sourcePieces =
        (!string.IsNullOrEmpty(_previousSearchTerm)
                && normalizedTerm.StartsWith(_previousSearchTerm, StringComparison.Ordinal))
            ? FilteredPieces
            : CurrentPieces;

    _tempSearchMatches.Clear();

    if (_tempSearchMatches.Capacity < sourcePieces.Count) {
      _tempSearchMatches.Capacity = sourcePieces.Count;
    }

    ReadOnlySpan<char> termSpan = normalizedTerm.AsSpan();
    ReadOnlySpan<Range> activeTokens = tokenRanges[..tokenCount];

    for (int i = 0, count = sourcePieces.Count; i < count; i++) {
      Piece piece = sourcePieces[i];

      if (MatchesSearch(piece, termSpan, activeTokens, isVanillaMode)) {
        _tempSearchMatches.Add(piece);
      }
    }

    FilteredPieces.Clear();
    FilteredPieces.AddRange(_tempSearchMatches);
    _tempSearchMatches.Clear();
    _previousSearchTerm = normalizedTerm;
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

  static bool MatchesSearch(
      Piece piece, ReadOnlySpan<char> searchTerm, ReadOnlySpan<Range> tokens, bool isVanillaMode) {
    string searchString = GetOrCreateSearchString(piece, isVanillaMode);
    ReadOnlySpan<char> searchSpan = searchString.AsSpan();

    if (isVanillaMode) {
      return searchSpan.Contains(searchTerm, StringComparison.Ordinal);
    }

    for (int i = 0, count = tokens.Length; i < count; i++) {
      ReadOnlySpan<char> token = searchTerm[tokens[i]];

      if (!searchSpan.Contains(token, StringComparison.Ordinal)) {
        return false;
      }
    }

    return true;
  }

  static string GetOrCreateSearchString(Piece piece, bool isVanillaMode) {
    if (!_cachedSearchStrings.TryGetValue(piece.gameObject.name, out string searchString)) {
      searchString = CreateSearchString(piece, ItemDrop.ItemData.m_stringBuilder, isVanillaMode);
      _cachedSearchStrings[piece.gameObject.name] = searchString;
    }

    return searchString;
  }

  static string CreateSearchString(Piece piece, StringBuilder searchStringBuilder, bool stripSpaces) {
    searchStringBuilder
        .Clear()
        .AppendLocalized(piece.m_name, stripSpaces);

    if (piece.m_resources == null) {
      return searchStringBuilder.ToString();
    }

    for (int i = 0, count = piece.m_resources.Length; i < count; i++) {
      Piece.Requirement requirement = piece.m_resources[i];

      if (requirement == null || !requirement.m_resItem) {
        continue;
      }

      searchStringBuilder
          .Append('\0')
          .AppendLocalized(requirement.m_resItem.m_itemData.m_shared.m_name, stripSpaces);
    }

    return searchStringBuilder.ToString();
  }

  static void AppendLocalized(this StringBuilder builder, string text, bool stripSpaces) {
    string localized = Localization.m_instance.Localize(text).ToLowerInvariant();

    if (!stripSpaces) {
      builder.Append(localized);
      return;
    }

    for (int i = 0, length = localized.Length; i < length; i++) {
      char c = localized[i];

      if (c != ' ') {
        builder.Append(c);
      }
    }
  }
}
