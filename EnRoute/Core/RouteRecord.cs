namespace EnRoute;

using System.Collections.Generic;

public sealed class RouteRecord {
  public readonly ZNetPeer NetPeer;
  public readonly long UserId;
  public readonly HashSet<long> NearbyUserIds = [];
  public Vector2s Sector = Vector2s.zero;

  public RouteRecord(ZNetPeer netPeer) {
    NetPeer = netPeer;
    UserId = netPeer.m_uid;
  }
}
