using Hylasoft.Opc.Common;
using Hylasoft.Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hylasoft.Opc.Tests
{
  public class TestUaClientNotConnected : UaClient
  {
    private OpcStatus _discard;

    public TestUaClientNotConnected(Uri server)
      : base(server)
    {
    }

    public Session SessionExtended
    {
      get
      {
        return this.Session;
      }
    }

    // always disconnected
    public new OpcStatus Status
    {
      get
      {
        return OpcStatus.NotConnected;
      }

      set
      {
        _discard = value;
      }
    }
  }
}
