namespace ComfyLib;

using System;
using System.Collections.Generic;

public static class StringExtensions {
  public static int GetStableHashCode(this ReadOnlySpan<char> span) {
    int hash1 = 5381;
    int hash2 = hash1;
    int i = 0;

    while (i < span.Length && span[i] != '\0') {
      hash1 = ((hash1 << 5) + hash1) ^ span[i];

      if (i == span.Length - 1 || span[i + 1] == '\0') {
        break;
      }

      hash2 = ((hash2 << 5) + hash2) ^ span[i + 1];
      i += 2;
    }

    return hash1 + (hash2 * 1566083941);
  }

  public static List<int> GetStableHashCodes(this ReadOnlySpan<char> span, char separator = ',') {
    span = span.Trim();

    if (span.IsEmpty) {
      return [];
    }

    List<int> hashes = [];

    while (!span.IsEmpty) {
      int index = span.IndexOf(separator);
      ReadOnlySpan<char> token;

      if (index < 0) {
        token = span.Trim();
        span = [];
      } else {
        token = span[..index].Trim();
        span = span[(index + 1)..];
      }

      if (!token.IsEmpty) {
        hashes.Add(token.GetStableHashCode());
      }
    }

    return hashes;
  }
}
