using Hylasoft.Opc.Common;
using Hylasoft.Opc.Common.Nodes;

namespace Hylasoft.Opc.Ua
{
  /// <summary>
  /// Represents a node to be used specifically for OPC UA
  /// </summary>
  public class UaNode : Node
  {
    public string NodeId { get; private set; }

    public UaNode(IClient<UaNode> client, string name, string nodeId, Node parent = null)
      : base(client, name, parent)
    {
      NodeId = nodeId;
    }
  }
}
