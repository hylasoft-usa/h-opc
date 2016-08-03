using Hylasoft.Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hylasoft.Opc.Tests
{
  public class TestExtendUaClient : UaClient
  {
    public TestExtendUaClient(Uri server)
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
  }
}
