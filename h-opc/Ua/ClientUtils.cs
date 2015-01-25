using System.Linq;
using Opc.Ua;
using System;

namespace Hylasoft.Opc.Ua
{
  /// <summary>
  /// List of static utility methods
  /// </summary>
  internal static class ClientUtils
  {
    public static EndpointDescription SelectEndpoint(Uri discoveryUrl, bool useSecurity)
    {
      // TODO I didn't write this method. I should rewrite it once I understand whtat it does, beacuse it looks crazy
      var configuration = EndpointConfiguration.Create();
      configuration.OperationTimeout = 5000;
      EndpointDescription endpointDescription1 = null;
      using (var discoveryClient = DiscoveryClient.Create(discoveryUrl, configuration))
      {
        var endpoints = discoveryClient.GetEndpoints(null);
        foreach (var endpointDescription2 in endpoints.Where(endpointDescription2 => endpointDescription2.EndpointUrl.StartsWith(discoveryUrl.Scheme)))
        {
          if (useSecurity)
          {
            if (endpointDescription2.SecurityMode == MessageSecurityMode.None)
              continue;
          }
          else if (endpointDescription2.SecurityMode != MessageSecurityMode.None)
            continue;
          if (endpointDescription1 == null)
            endpointDescription1 = endpointDescription2;
          if (endpointDescription2.SecurityLevel > endpointDescription1.SecurityLevel)
            endpointDescription1 = endpointDescription2;
        }
        if (endpointDescription1 == null)
        {
          if (endpoints.Count > 0)
            endpointDescription1 = endpoints[0];
        }
      }
      var uri = Utils.ParseUri(endpointDescription1.EndpointUrl);
      if (uri != null && uri.Scheme == discoveryUrl.Scheme)
        endpointDescription1.EndpointUrl = new UriBuilder(uri)
        {
          Host = discoveryUrl.DnsSafeHost,
          Port = discoveryUrl.Port
        }.ToString();
      return endpointDescription1;
    }
  }
}

