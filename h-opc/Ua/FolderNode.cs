using System;
using System.Collections.Generic;

namespace Hylasoft.Opc.Ua
{
  /// <summary>
  /// A node that contains subnodes
  /// </summary>
  public class FolderNode : Node
  {
    private IEnumerable<Node> _subNodes;

    public FolderNode(IClient client, string tag) : base(client, tag)
    {
    }

    public override NodeType Type
    {
      get { return NodeType.Folder; }
    }

    /// <summary>
    /// Gets the list of subnodes from the server
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "WIP")]
    public IEnumerable<Node> SubNodes
    {
      get
      {
        _subNodes = null; //TODO
        if (_subNodes != null)
          return _subNodes;
        throw new NotImplementedException();
      }
    }
  }
}