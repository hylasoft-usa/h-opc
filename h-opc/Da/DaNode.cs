using System;
using Hylasoft.Opc.Common;

namespace Hylasoft.Opc.Da
{
  /// <summary>
  /// Represents a node to be used specifically for OPC DA
  /// </summary>
  public class DaNode : Node
  {
    /// <summary>
    /// Instantiates a DaNode class
    /// </summary>
    /// <param name="client">the client the node belongs to</param>
    /// <param name="name">the name of the node</param>
    /// <param name="tag"></param>
    /// <param name="parent">The parent node</param>
    public DaNode(IClient<Node> client, string name, string tag, Node parent = null)
      : base(client, name, parent)
    {
      Tag = tag;
    }
  }
}
