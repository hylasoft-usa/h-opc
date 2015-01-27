using Hylasoft.Opc.Common.Nodes;
using OpcF = Opc.Ua;

namespace Hylasoft.Opc.Ua
{
  /// <summary>
  /// Class with extension methods for OPC UA
  /// </summary>
  public static class NodeExtensions
  {
    /// <summary>
    /// Converts an OPC Foundation node to an Hylasoft OPC UA Node
    /// </summary>
    /// <param name="node">The node to convert</param>
    /// <param name="client">the client the node belongs to</param>
    /// <param name="parent">the parent node (optional)</param>
    /// <returns></returns>
    public static UaNode ToHylaNode(this OpcF.ReferenceDescription node, UaClient client, Node parent = null)
    {
      var name = node.DisplayName.ToString();
      var nodeId = node.NodeId.ToString();
      return new UaNode(client, name, nodeId, parent);
    }
  }
}