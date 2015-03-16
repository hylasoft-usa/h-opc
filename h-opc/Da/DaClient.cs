using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Hylasoft.Opc.Common;
using Opc;
using Factory = OpcCom.Factory;
using OpcDa = Opc.Da;

namespace Hylasoft.Opc.Da
{
  /// <summary>
  /// Client Implementation for DA
  /// </summary>
  public class DaClient : IClient<DaNode>
  {
    private readonly URL _url;
    private OpcDa.Server _server;
    private long _sub;
    private readonly IDictionary<string, DaNode> _nodesCache = new Dictionary<string, DaNode>();

    // default monitor interval in Milliseconds
    private const int DefaultMonitorInterval = 100;

    /// <summary>
    /// Initialize a new Data Access Client
    /// </summary>
    /// <param name="serverUrl">the url of the server to connect to</param>
    public DaClient(Uri serverUrl)
    {
      _url = new URL(serverUrl.AbsolutePath)
      {
        Scheme = serverUrl.Scheme,
        HostName = serverUrl.Host
      };
    }

    #region interface methods

    public void Dispose()
    {
      _server.Disconnect();
      _server.Dispose();
    }

    /// <summary>
    /// Connect the client to the OPC Server
    /// </summary>
    public void Connect()
    {
      if (Status == OpcStatus.Connected)
        return;
      _server = new OpcDa.Server(new Factory(), _url);
      _server.Connect();
      RootNode = new DaNode(this, string.Empty, string.Empty);
      AddNodeToCache(RootNode);
    }

    public OpcStatus Status
    {
      get
      {
        if (_server == null || _server.GetStatus().ServerState != OpcDa.serverState.running)
          return OpcStatus.NotConnected;
        return OpcStatus.Connected;
      }
    }

    public T Read<T>(string tag)
    {
      var item = new OpcDa.Item { ItemName = tag };
      var result = _server.Read(new[] { item })[0];

      CheckResult(result);

      return (T)result.Value;
    }

    public void Write<T>(string tag, T item)
    {
      var itmVal = new OpcDa.ItemValue
      {
        ItemName = tag,
        Value = item
      };
      var result = _server.Write(new[] { itmVal })[0];
      CheckResult(result);
    }

    public void Monitor<T>(string tag, Action<T, Action> callback)
    {
      var subItem = new OpcDa.SubscriptionState
      {
        Name = (++_sub).ToString(CultureInfo.InvariantCulture),
        Active = true,
        UpdateRate = DefaultMonitorInterval
      };
      var sub = _server.CreateSubscription(subItem);
      
      // I have to start a new thread here because unsubscribing
      // the subscription during a datachanged event causes a deadlock
      Action unsubscribe = () => new Thread(o =>
        _server.CancelSubscription(sub)).Start();
      
      sub.DataChanged += (handle, requestHandle, values) =>
        callback((T)values[0].Value, unsubscribe);
      sub.AddItems(new[] {new OpcDa.Item {ItemName = tag}});
      sub.SetEnabled(true);
    }

    public DaNode FindNode(string tag)
    {
      // if the tag already exists in cache, return it
      if (_nodesCache.ContainsKey(tag))
        return _nodesCache[tag];

      // try to find the tag otherwise
      var item = new OpcDa.Item { ItemName = tag };
      var result = _server.Read(new[] { item })[0];
      CheckResult(result);
      var node = new DaNode(this, item.ItemName, item.ItemName, RootNode);
      AddNodeToCache(node);
      return node;
    }

    public DaNode RootNode { get; private set; }

    public IEnumerable<DaNode> ExploreFolder(string tag)
    {
      var parent = FindNode(tag);
      OpcDa.BrowsePosition p;
      var nodes = _server.Browse(new ItemIdentifier(parent.Tag), new OpcDa.BrowseFilters(), out p)
        .Select(t => new DaNode(this, t.Name, t.ItemName, parent))
        .ToList();
      //add nodes to cache
      foreach (var node in nodes)
        AddNodeToCache(node);

      return nodes;
    }

    #endregion

    /// <summary>
    /// Adds a node to the cache using the tag as its key
    /// </summary>
    /// <param name="node">the node to add</param>
    private void AddNodeToCache(DaNode node)
    {
      if (!_nodesCache.ContainsKey(node.Tag))
        _nodesCache.Add(node.Tag, node);
    }

    private void CheckResult(IResult result)
    {
      if (result == null)
        throw new OpcException("The server replied with an empty response");
      if (result.ResultID.ToString() != "S_OK")
        throw new OpcException(string.Format("Invalid response from the server. (Response Status: {0})", result.ResultID));
    }
  }
}
