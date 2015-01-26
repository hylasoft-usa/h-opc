using System;
using System.Linq;
using System.Threading;
using Hylasoft.Behavior;
using Hylasoft.Behavior.Extensions;
using Hylasoft.Opc.Common;
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

    [TestCleanup]
    public void Cleanup()
    {
      _client.Dispose();
    }

    [TestMethod]
    public void StatusTest()
    {
      Expect(_client.Status).ToBe(OpcStatus.Connected);
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
    public void WriteNodeTest()
    {
      const string tag = "Data.Static.Scalar.ByteValue";

      _client.Write(tag, (byte) 3);
      var val = _client.Read<byte>(tag);
      Expect(val).ToBe(3);

      _client.Write(tag, (byte)13);
      val = _client.Read<byte>(tag);
      Expect(val).ToBe(13);
    }

    [TestMethod]
    public void FailWriteTest()
    {
      // fails for wrong tag
      Expect<Action>(() => _client.Write("WRONG TAG", (byte)3))
        .ToThrowException<OpcException>();

      // fails for wrong type
      Expect<Action>(() => _client.Write("Data.Static.Scalar.ByteValue", "WRONG TYPE"))
        .ToThrowException<OpcException>();

      // fails for not writing allowed
      Expect<Action>(() =>_client.Write("Server.ServerStatus.BuildInfo.ManufacturerName", "READ ONLY"))
        .ToThrowException<OpcException>();
    }

    [TestMethod]
    public void FailReadTest()
    {
      // fails for wrong tag
      Expect<Action>(() => _client.Read<int>("XXXXXX"))
        .ToThrowException<OpcException>();

      // fails for not readable tag
      Expect<Action>(() => _client.Read<string>("Server"))
        .ToThrowException<OpcException>();
    }

    [TestMethod]
    public void BrowseFolderTest()
    {
      var node = _client.FindNode("Server.ServerStatus.BuildInfo");

      // ReSharper disable once PossibleNullReferenceException
      var subNodes = node.SubNodes;
      Expect(subNodes.Count()).ToBe(6);
    }

    [TestMethod]
    public void MonitorTest()
    {
      const string tag = "Data.Static.Scalar.ByteValue";
      var executed = 0;
      _client.Monitor<byte>(tag, (val1, u) =>
      {
        executed++;
        u();
      });
      _client.Monitor<byte>(tag, (val1, u) =>
      {
        executed++;
        u();
      });
      _client.Monitor<byte>(tag, (val1, u) =>
      {
        executed++;
        u();
      });
      const int interval = 100;
      Thread.Sleep(interval);
      _client.Write(tag, (byte)10);
      Thread.Sleep(interval);
      _client.Write(tag, (byte)11);
      Thread.Sleep(interval);
      _client.Write(tag, (byte)12);
      Thread.Sleep(interval);
      _client.Write(tag, (byte)13);
      Thread.Sleep(interval);
      Expect(executed).ToBe(3);
    }
  }
}
