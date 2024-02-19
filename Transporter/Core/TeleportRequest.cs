namespace Transporter;

using System;

using UnityEngine;

public sealed class TeleportRequest {
  public long PlayerId { get; }
  public Vector3 Destination { get; }
  public long EpochTimestamp { get; }

  public TeleportRequest(long playerId, Vector3 destination, long epochTimestamp) {
    PlayerId = playerId;
    Destination = destination;
    EpochTimestamp = epochTimestamp;
  }

  public TeleportRequest(long playerId, Vector3 destination)
      : this(playerId, destination, DateTimeOffset.UtcNow.ToUnixTimeSeconds()) {
  }

  public override string ToString() {
    return $"{PlayerId},{Destination.x:F0},{Destination.y:F0},{Destination.z:F0},{EpochTimestamp:F0}";
  }

  public static char[] CommaDelimiter = new char[] { ',' };

  public static bool TryParse(string text, out TeleportRequest request) {
    string[] parts = text.Split(CommaDelimiter, count: 5, StringSplitOptions.RemoveEmptyEntries);

    if (parts.Length >= 5
        && long.TryParse(parts[0], out long playerId)
        && float.TryParse(parts[1], out float x)
        && float.TryParse(parts[2], out float y)
        && float.TryParse(parts[3], out float z)
        && long.TryParse(parts[4], out long epochTimestamp)) {
      request = new(playerId, new(x, y, z), epochTimestamp);
      return true;
    }

    request = default;
    return false;
  }
}
