using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExternalConsole {
  public static class NamedPipeManager {
  }

  //public sealed class PipeClient {
  //  public string PipeName { get; }
  //  public NamedPipeClientStream ClientStream { get; }

  //  public PipeClient(string pipeName) {
  //    PipeName = pipeName;
  //    ClientStream = new(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
  //  }

  //  public async Task Connect() {

  //  }

  //  async Task StartReading(CancellationToken cancellationToken) {
  //    while (true) {
        
  //    }
  //  }
  //}
}
