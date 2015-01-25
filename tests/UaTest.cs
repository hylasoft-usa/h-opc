using System;
using Hylasoft.Behavior;
using Hylasoft.Opc.Ua;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hylasoft.Opc.Tests
{
  [TestClass]
  public class UaTest : Spec
  {
    [TestMethod]
    public void ConnectTest()
    {
      var client = new UaClient(new Uri("opc.tcp://giacomo-hyla:51210/UA/SampleServer"));
      Expect(client.Status).ToBe(OpcStatus.NotConnected);

      client.Connect();

      Expect(client.Status).ToBe(OpcStatus.Connected);
    }

    [TestMethod]
    public void FindNodeTest()
    {
      var client = new UaClient(new Uri("opc.tcp://giacomo-hyla:51210/UA/SampleServer"));
      client.Connect();

      var node = client.FindNode("Data.Dynamic.Scalar.SByteValue");
      Expect(node).ToNotBeNull();
    }

    [TestMethod]
    public void ReadNodeTest()
    {
      var client = new UaClient(new Uri("opc.tcp://giacomo-hyla:51210/UA/SampleServer"));
      client.Connect();

      var val = client.Read<string>("Server.ServerStatus.BuildInfo.ManufacturerName");
      Expect(val).ToBe("OPC Foundation");
    }
  }
}
