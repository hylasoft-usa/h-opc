using Hylasoft.Opc.Common;
using Hylasoft.Opc.Da;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpcDa = Opc.Da;

namespace Hylasoft.Opc.Tests
{
  public class TestExtendDaClient : DaClient
  {
    public TestExtendDaClient(Uri server) : base(server)
    {
    }

    public OpcDa.Server ExposedServer
    {
      get
      {
        return this.Server;
      }
    }
  }
}
