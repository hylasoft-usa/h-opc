using System.Collections.Generic;

namespace Hylasoft.Opc.Common.Nodes
{
  /// <summary>
  /// Base class representing a node on the OPC server
  /// </summary>
  public class Node
  {
    private IEnumerable<Node> _subNodes;

    /// <summary>
    /// Gets the client that the node belongs to
    /// </summary>
    public IClient<Node> Client { get; private set; }

    /// <summary>
    /// Gets the displayed name of the node
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Gets the dot-separated fully qualified tag of the node
    /// </summary>
    public string Tag { get; protected set; }

    /// <summary>
    /// Gets the parent node. If the node is root, returns null
    /// </summary>
    public Node Parent { get; private set; }

    /// <summary>
    /// Creates a new node
    /// </summary>
    /// <param name="client">the client the node belongs to</param>
    /// <param name="name">the name of the node</param>
    /// <param name="parent">The parent node</param>
    protected Node(IClient<Node> client, string name, Node parent = null)
    {
      Client = client;
      Name = name;
      Parent = parent;
      if (parent != null && !string.IsNullOrEmpty(parent.Tag))
        Tag = parent.Tag + '.' + name;
      else
        Tag = name;
    }

    /// <summary>
    /// Gets the list of subnodes from the server
    /// </summary>
    public IEnumerable<Node> SubNodes
    {
      get
      {
        if (_subNodes == null)
          _subNodes = Client.ExploreFolder(Tag);
        return _subNodes;
      }
    }

    /// <summary>
    /// Overrides ToString()
    /// </summary>
    public override string ToString()
    {
      return Tag;
    }
  }
}
