using System;
using System.Linq;
using Hylasoft.Behavior;
using Hylasoft.Opc.Ua;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hylasoft.Opc.Tests
{
  [TestClass]
  public class UaTest : Spec
  {
    private UaClient _client;

    [TestInitialize]
    public void Init()
    {
      _client = new UaClient(new Uri("opc.tcp://giacomo-hyla:51210/UA/SampleServer"));
      _client.Connect();
    }

    [TestMethod]
    public void FindNodeTest()
    {
      var node = _client.FindNode("Data.Dynamic.Scalar.SByteValue");
      Expect(node).ToNotBeNull();
    }

    [TestMethod]
    public void ReadNodeTest()
    {
      var val = _client.Read<string>("Server.ServerStatus.BuildInfo.ManufacturerName");
      Expect(val).ToBe("OPC Foundation");
    }

    [TestMethod]
    public void BrowseFolderTest()
    {
      var node = _client.FindNode("Server.ServerStatus.BuildInfo");

      // ReSharper disable once PossibleNullReferenceException
      var subNodes = node.SubNodes;
      Expect(subNodes.Count()).ToBe(6);
    }
  }
}
