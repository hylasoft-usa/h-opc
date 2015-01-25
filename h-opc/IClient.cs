using System;

namespace Hylasoft.Opc
{
  /// <summary>
  /// Client interface to perform basic Opc tasks, like discovery, monitoring, reading/writing tags,
  /// </summary>
  public interface IClient
  {
    void Connect();

    OpcStatus Status { get; }

    /// <summary>
    /// Read a tag
    /// </summary>
    /// <typeparam name="T">The type of tag to read</typeparam>
    /// <param name="tag">the identifier of the tag. You can specify a subfolder by using a comma delimited name.
    /// E.g: the tag `foo.bar` reads the tag `bar` on the folder `foo`</param>
    /// <returns>The value retrieved from the OPC</returns>
    T Read<T>(string tag);

    /// <summary>
    /// Write a value on the specified opc tag
    /// </summary>
    /// <typeparam name="T">The type of tag to write on</typeparam>
    /// <param name="tag">the identifier of the tag. You can specify a subfolder by using a comma delimited name.
    /// E.g: the tag `foo.bar` writes on the tag `bar` on the folder `foo`</param>
    /// <param name="item"></param>
    void Write<T>(string tag, T item);

    /// <summary>
    /// Monitor the specified tag for changes
    /// </summary>
    /// <typeparam name="T">the type of tag to monitor</typeparam>
    /// <param name="tag">the identifier of the tag. You can specify a subfolder by using a comma delimited name.
    /// E.g: the tag `foo.bar` monitors the tag `bar` on the folder `foo`</param>
    /// <param name="callback">the callback to execute when the value is changed.
    /// The callback gets executed every time the value gets changed</param>
    void Monitor<T>(string tag, Action<T> callback);
  }
}