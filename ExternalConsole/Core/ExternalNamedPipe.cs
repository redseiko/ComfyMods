using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

using ComfyLib;

namespace ExternalConsole {
  public static class ExternalNamedPipe {
    public static NamedPipeClientStream PipeClient { get; private set; }
    public static StreamWriter PipeWriter { get; private set; }
    public static StreamReader PipeReader { get; private set; }

    public static async Task Connect(string pipeName) {
      PipeClient = new(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
      ExternalConsole.LogInfo($"Connecting to named pipe: {pipeName}");
      await PipeClient.ConnectAsync();
      ExternalConsole.LogInfo($"Connected!");
    }
    
    public static IEnumerator StartExternalNamedPipe(string pipeName) {
      Console console = Console.m_instance;
      PipeClient = new(".", pipeName, PipeDirection.InOut, PipeOptions.WriteThrough | PipeOptions.Asynchronous);
      ExternalConsole.LogInfo($"Connecting to named pipe: {pipeName}");
      yield return Task.Run(() => PipeClient.ConnectAsync()).AsCoroutine();

      //while (!connectTask.IsCompleted) {
      //  yield return null;
      //}

      PipeWriter = new(PipeClient) {
        AutoFlush = true
      };

      PipeReader = new(PipeClient);

      _pipeOutputQueue = new();

      ExternalConsole.LogInfo($"Connected to pipe!");
      Task readPipeInputTask = Task.Run(() => ReadPipeInput());
      Task writePipeOutputTask = Task.Run(() => WritePipeOutput());
      Task task = Task.WhenAny(readPipeInputTask, writePipeOutputTask);

      while (PipeClient.IsConnected && !task.IsCompleted && console) {
        yield return null;
      }

      if (task.IsFaulted) {
        ExternalConsole.LogError($"Failed ReadPipeInput task: {readPipeInputTask.Exception}");
      }

      ExternalConsole.LogInfo($"Disconnected from pipe.");
    }

    static async Task ReadPipeInput() {
      ExternalConsole.LogInfo($"Starting ReadPipeInput().");

      while (PipeClient.IsConnected) {
        string pipeInput = await PipeReader.ReadLineAsync();

        if (pipeInput == null) {
          ExternalConsole.LogError($"PipeInput is null.");
          break;
        } else if (pipeInput.Length == 0) {
          ExternalConsole.LogError($"PipeInput is empty string.");
        } else {
          ExternalConsole.LogInfo($"PipeInput: {pipeInput}");
        }
      }

      ExternalConsole.LogInfo($"Stopping ReadPipeInput().");
    }

    static BlockingCollection<string> _pipeOutputQueue;

    static async Task WritePipeOutput() {
      ExternalConsole.LogInfo($"Starting WritePipeOutput().");

      while (PipeClient.IsConnected) {
        string pipeOutput = _pipeOutputQueue.Take();

        if (string.IsNullOrWhiteSpace(pipeOutput)) {
          ExternalConsole.LogError($"PipeOutput is null/empty.");
        } else {
          ExternalConsole.LogInfo($"PipeOutput: {pipeOutput}");
          await PipeWriter.WriteLineAsync(pipeOutput);
          //PipeWriter.WriteLine(pipeOutput);
          //PipeClient.WaitForPipeDrain();
        }
      }

      ExternalConsole.LogInfo($"Stopping WritePipeOutput().");
    }

    public static bool SendPipeOutput(string pipeOutput) {
      if (string.IsNullOrWhiteSpace(pipeOutput)) {
        ExternalConsole.LogError($"Ignoring empty pipe output.");
        return false;
      }

      ExternalConsole.LogInfo($"Queue PipeOutput: {pipeOutput}");
      _pipeOutputQueue.Add(pipeOutput);

      return true;
    }
  }
}
