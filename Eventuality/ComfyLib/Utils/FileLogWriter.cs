namespace ComfyLib;

using System;
using System.IO;
using System.Text;

public sealed class FileLogWriter : IDisposable {
  readonly string _filename;
  readonly StreamWriter _writer;

  public FileLogWriter(string filename, string header = default, bool autoFlush = true) {
    _filename = filename;

    _writer = new StreamWriter(_filename, append: true, Encoding.UTF8) {
      AutoFlush = autoFlush,
    };

    if (_writer.BaseStream.Length == 0 && !string.IsNullOrEmpty(header)) {
      _writer.WriteLine(header);
    }
  }

  public void WriteLine(string line) {
    _writer.WriteLine(line);
  }

  public void Dispose() {
    _writer?.Dispose();
  }
}
