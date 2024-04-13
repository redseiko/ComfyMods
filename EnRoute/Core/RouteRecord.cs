namespace EnRoute;

using System.Collections.Generic;

public class RouteRecord {
  public readonly ZNetPeer NetPeer;
  public readonly long UserId;
  public readonly HashSet<long> NearbyUserIds = new();
  public Vector2i Sector = Vector2i.zero;

  public RouteRecord(ZNetPeer netPeer) {
    NetPeer = netPeer;
    UserId = netPeer.m_uid;
  }
}
