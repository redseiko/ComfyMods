namespace BetterZeeRouter;

public abstract class RpcMethodHandler {
  public abstract bool Process(ZRoutedRpc.RoutedRPCData routedRpcData);
}
