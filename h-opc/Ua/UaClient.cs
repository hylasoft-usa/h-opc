using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace Hylasoft.Opc.Ua
{
  public class UaClient : IClient
  {
    private readonly Uri _serverUrl;
    private Session _session;
    private FolderNode _rootNode;
    private readonly IDictionary<string, Node> _nodesCache = new Dictionary<string, Node>();

    /// <summary>
    /// Creates a server object
    /// </summary>
    /// <param name="serverUrl">the url of the server to connect to</param>
    public UaClient(Uri serverUrl)
    {
      _serverUrl = serverUrl;
      Status = OpcStatus.NotConnected;
    }

    public void Connect()
    {
      if (Status == OpcStatus.Connected)
        return;
      _session = InitializeSession(_serverUrl);
      var node = _session.NodeCache.Find(ObjectIds.ObjectsFolder);
      _rootNode = new FolderNode(this, "", (NodeId) node.NodeId);
      AddNodeToCache(_rootNode);
      Status = OpcStatus.Connected;
    }

    public OpcStatus Status { get; private set; }

    public T Read<T>(string tag)
    {
      var n = FindNode(tag);
      var nodesToRead = new ReadValueIdCollection
      {
        new ReadValueId
        {
          NodeId = n.Id,
          AttributeId = 13U
        }
      };
      DataValueCollection results;
      DiagnosticInfoCollection diag;
      var res = _session.Read(null, 0, TimestampsToReturn.Neither, nodesToRead, out results, out diag);

      var val = (T) results[0].Value;
      return val;
    }

    public void Write<T>(string tag, T item)
    {
      throw new NotImplementedException();
    }

    public void Monitor<T>(string tag, Action<T> callback)
    {
      throw new NotImplementedException();
    }

    public Node FindNode(string tag)
    {
      // if the tag already exists in cache, return it
      if (_nodesCache.ContainsKey(tag))
        return _nodesCache[tag];
      
      // try to find the tag otherwise
      var found = FindNode(tag, _rootNode);
      if (found != null)
      {
        AddNodeToCache(found);
        return found;
      }

      // throws an exception if not found
      throw new ArgumentException(String.Format("The tag \"{0}\" doesn't exist on the Server", tag), tag);
    }

    /// <summary>
    /// Finds a node starting from the specified node as the root folder
    /// </summary>
    /// <param name="tag">the tag to find</param>
    /// <param name="node">the root node</param>
    /// <returns></returns>
    private Node FindNode(string tag, Node node)
    {
      var folders = tag.Split('.');
      var head = folders.FirstOrDefault();
      Node found;
      try
      {
        var subNodes = ClientUtils.Browse(_session, node);
        found = subNodes.First(n => n.DisplayName == head).ToHylaNode(this);
      }
      catch (Exception ex)
      {
        throw new ArgumentException(String.Format("The tag \"{0}\" doesn't exist on folder \"{1}\"", head, node.Tag), tag, ex);
      }

      return folders.Length == 1
        ? found // last node, return it
        : FindNode(string.Join(".", folders.Except(new[] { head })), found); // find sub nodes
    }

    #region private methods

    /// <summary>
    /// Adds a node to the cache using the tag as its key
    /// </summary>
    /// <param name="node">the node to add</param>
    private void AddNodeToCache(Node node)
    {
      _nodesCache.Add(node.Tag, node);    
    }

    /// <summary>
    /// Crappy method to initialize the session. I don't know what many of these things do, sincerely.
    /// </summary>
    private static Session InitializeSession(Uri url)
    {
      var l = new CertificateValidator();
      l.CertificateValidation += (sender, eventArgs) =>
      {
        eventArgs.Accept = true;
      };
      var appInstance = new ApplicationInstance
      {
        ApplicationType = ApplicationType.Client,
        ConfigSectionName = "h-opc-client",
        ApplicationConfiguration = new ApplicationConfiguration
        {
          ApplicationUri = url.ToString(),
          ApplicationName = "h-opc-client",
          ApplicationType = ApplicationType.Client,
          CertificateValidator = l,
          SecurityConfiguration = new SecurityConfiguration
          {
            AutoAcceptUntrustedCertificates = true
          },
          TransportQuotas = new TransportQuotas
          {
            OperationTimeout = 600000,
            MaxStringLength = 1048576,
            MaxByteStringLength = 1048576,
            MaxArrayLength = 65535,
            MaxMessageSize = 4194304,
            MaxBufferSize = 65535,
            ChannelLifetime = 300000,
            SecurityTokenLifetime = 3600000
          },
          ClientConfiguration = new ClientConfiguration
          {
            DefaultSessionTimeout = 60000,
            MinSubscriptionLifetime = 10000
          },
          DisableHiResClock = true
        }
      };
      var endpoints = ClientUtils.SelectEndpoint(url, false);
      var session = Session.Create(appInstance.ApplicationConfiguration,
        new ConfiguredEndpoint(null, endpoints,
          EndpointConfiguration.Create(appInstance.ApplicationConfiguration)), false, false,
        appInstance.ApplicationConfiguration.ApplicationName, 60000U, null, new string[] { });

      return session;
    }

    #endregion

  }
}
