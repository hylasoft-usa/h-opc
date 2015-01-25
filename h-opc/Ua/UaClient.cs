using System;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace Hylasoft.Opc.Ua
{
  public class UaClient : IClient
  {
    private readonly Uri _serverUrl;
    private Session _session;
    private Node _rootNode;

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
      Status = OpcStatus.Connected;
    }

    public OpcStatus Status { get; private set; }

    public T Read<T>(string tag)
    {
      throw new NotImplementedException(_session.ToString());
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
      _rootNode = new FolderNode(this, tag); // TODO
      return _rootNode;
    }

    #region private methods

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
