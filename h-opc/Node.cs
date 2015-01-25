using Opc.Ua;

namespace Hylasoft.Opc
{
  /// <summary>
  /// Base class representing a node on the OPC server
  /// </summary>
  public abstract class Node
  {
    protected IClient Client { get; private set; }

    public string Tag { get; private set; }

    private readonly NodeId _id;

    /// <summary>
    /// Creates a new node
    /// </summary>
    /// <param name="client">the client to use for the queries</param>
    /// <param name="tag">the name of the tag</param>
    /// <param name="id">the id that represent the tag on the server. If it's null, the tag is used as ID</param>
    protected Node(IClient client, string tag, NodeId id = null)
    {
      // TODO I guess we cannot really use NodeId, since it's specific to UA...
      // use the tag if the node is null
      _id = id ?? tag;
      Client = client;
      Tag = tag;
    }

    /// <summary>
    /// The class of node
    /// </summary>
    public abstract NodeClass Class { get; }

    /// <summary>
    /// Gets the identifier of the node on the server
    /// </summary>
    public NodeId Id
    {
      get { return _id; }
    }
  }

  public enum NodeClass
  {
    /// <summary>
    /// The nodes is a folder containing other nodes
    /// </summary>
    Folder,
    /// <summary>
    /// The folder is an object conatining a value
    /// </summary>
    Object
  }
}