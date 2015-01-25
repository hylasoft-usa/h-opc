namespace Hylasoft.Opc
{
  /// <summary>
  /// Base class representing a node on the OPC server
  /// </summary>
  public abstract class Node
  {
    protected IClient Client { get; private set; }

    protected string Tag { get; private set; }

    private readonly string _id;

    /// <summary>
    /// Creates a new node
    /// </summary>
    /// <param name="client">the client to use for the queries</param>
    /// <param name="tag">the name of the tag</param>
    /// <param name="id">the id that represent the tag on the server. If it's null, the tag is used as ID</param>
    protected Node(IClient client, string tag, string id = null)
    {
      // use the tag if the node is null
      _id = id ?? tag;
      Client = client;
      Tag = tag;
    }

    /// <summary>
    /// The type of node
    /// </summary>
    public abstract NodeType Type { get; }

    /// <summary>
    /// Gets the identifier of the node on the server
    /// </summary>
    public string Id
    {
      get { return _id; }
    }
  }

  public enum NodeType
  {
    // the nodes is a folder containing other nodes
    Folder,
    // the folder is an object conatining a value
    Object
  }
}