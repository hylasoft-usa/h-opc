using System;

namespace Hylasoft.Opc
{
  /// <summary>
  /// Client interface to perform basic Opc tasks, like discovery, monitoring, reading/writing tags,
  /// </summary>
  public interface IClient
  {
    void Connect();

    OpcStatus Status { get; set; }

    /// <summary>
    /// Read a tag
    /// </summary>
    /// <typeparam name="T">The type of tag to read</typeparam>
    /// <param name="tag">the identifier of the tag</param>
    /// <returns>The value retrieved from the OPC</returns>
    T Read<T>(string tag);

    void Write<T>(string tag, T item);

    void Monitor<T>(string tag, Action<T> callback);
  }
}