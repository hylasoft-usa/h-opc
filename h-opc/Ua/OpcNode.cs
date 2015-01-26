using Hylasoft.Opc.Common;
using Hylasoft.Opc.Common.Nodes;

namespace Hylasoft.Opc.Ua
{
  public class OpcNode : Node
  {
    public string NodeId { get; private set; }

    public OpcNode(IClient<OpcNode> client, string name, string nodeId, Node parent = null)
      : base(client, name, parent)
    {
      NodeId = nodeId;
    }
  }
}
