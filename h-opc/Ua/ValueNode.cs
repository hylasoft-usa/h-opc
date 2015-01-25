using System;
using Opc.Ua;

namespace Hylasoft.Opc.Ua
{
  /// <summary>
  /// A node that holds a value
  /// </summary>
  /// <typeparam name="T">The type of value that the node contains</typeparam>
  public class ValueNode<T> : Node
  {
    public ValueNode(IClient client, string tag, NodeId id = null)
      : base(client, tag, id)
    {
    }

    /// <summary>
    /// Read the tag
    /// </summary>
    /// <returns>the value from the tag</returns>
    public virtual T Read()
    {
      return Client.Read<T>(Tag);
    }

    /// <summary>
    /// Write on the tag
    /// </summary>
    /// <param name="item">the value to write</param>
    public virtual void Write(T item)
    {
      Client.Write(Tag, item);
    }

    /// <summary>
    /// Monitor the tag for changes
    /// </summary>
    /// <param name="callback">the callback to execute when the value is changed.
    /// The callback gets executed every time the value gets changed</param>
    public virtual void Monitor(Action<T> callback)
    {
      Client.Monitor(Tag, callback);
    }

    public override NodeClass Class
    {
      get { return NodeClass.Object; }
    }
  }
}