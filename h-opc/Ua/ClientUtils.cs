using System.Globalization;
using System.Linq;
using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hylasoft.Opc.Ua
{
  /// <summary>
  /// List of static utility methods
  /// </summary>
  internal static class ClientUtils
  {
    public static EndpointDescription SelectEndpoint(string discoveryUrl, bool useSecurity)
    {
      if (!discoveryUrl.StartsWith("opc.tcp") && !discoveryUrl.EndsWith("/discovery"))
        discoveryUrl = discoveryUrl + "/discovery";
      var discoveryUrl1 = new Uri(discoveryUrl);
      var configuration = EndpointConfiguration.Create();
      configuration.OperationTimeout = 5000;
      EndpointDescription endpointDescription1 = null;
      using (var discoveryClient = DiscoveryClient.Create(discoveryUrl1, configuration))
      {
        var endpoints = discoveryClient.GetEndpoints(null);
        foreach (var endpointDescription2 in endpoints.Where(endpointDescription2 => endpointDescription2.EndpointUrl.StartsWith(discoveryUrl1.Scheme)))
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
      if (uri != null && uri.Scheme == discoveryUrl1.Scheme)
        endpointDescription1.EndpointUrl = new UriBuilder(uri)
        {
          Host = discoveryUrl1.DnsSafeHost,
          Port = discoveryUrl1.Port
        }.ToString();
      return endpointDescription1;
    }
  }
}

