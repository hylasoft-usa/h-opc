using Hylasoft.Opc.Common.Nodes;
using OpcF = Opc.Ua;

namespace Hylasoft.Opc.Ua
{
  public static class NodeExtensions
  {
    public static OpcNode ToHylaNode(this OpcF.ReferenceDescription node, UaClient client, Node parent = null)
    {
      var name = node.DisplayName.ToString();
      var nodeId = node.NodeId.ToString();
      return new OpcNode(client, name, nodeId, parent);
    }
  }
}