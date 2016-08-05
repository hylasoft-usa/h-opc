using System;
using System.Linq;
using System.Threading;
using Hylasoft.Behavior;
using Hylasoft.Behavior.Extensions;
using Hylasoft.Opc.Common;
using Hylasoft.Opc.Ua;
using NUnit.Framework;
using System.Configuration;
using Opc.Ua.Client;
using Opc.Ua;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hylasoft.Opc.Tests
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test Class"), TestFixture]
  public class UaTest : Spec
  {
    private UaClient _client;

    [SetUp]
    public void Init()
    {
      _client = new UaClient(new Uri(ConfigurationManager.AppSettings["UATestEndpoint"]));
      _client.Connect();
    }

    [TearDown]
    public void Cleanup()
    {
      _client.Dispose();
    }

    [Test]
    public void StatusTest()
    {
      Expect(_client.Status).ToBe(OpcStatus.Connected);
    }

    [Test]
    public void FindNodeTest()
    {
      var node = _client.FindNode("Data.Dynamic.Scalar.SByteValue");
      Expect(node).ToNotBeNull();
    }

    [Test]
    public void ReadNodeTest()
    {
      var val = _client.Read<string>("Server.ServerStatus.BuildInfo.ManufacturerName");
      Expect(val).ToBe("OPC Foundation");
    }

    [Test]
    public void WriteNodeTest()
    {
      const string tag = "Data.Static.Scalar.ByteValue";

      _client.Write(tag, (byte)3);
      var val = _client.Read<byte>(tag);
      Expect(val).ToBe(3);

      _client.Write(tag, (byte)13);
      val = _client.Read<byte>(tag);
      Expect(val).ToBe(13);
    }


    [Test]
    public void ReadArrayNodeTest()
    {
      var val = _client.Read<bool[]>("Data.Static.Array.BooleanValue");
      Expect(val.Length).ToBeGreaterThan(0);
    }

    [Test]
    public void WriteArrayNodeTest()
    {
      const string tag = "Data.Static.Array.BooleanValue";

      var val1 = new[] { true, false, false };
      var val2 = new[] { false, false };

      _client.Write(tag, val1);
      var val = _client.Read<bool[]>(tag);
      Expect(val.Zip(val1, (a, b) => a == b)).ToNotContain(false);

      _client.Write(tag, val2);
      val = _client.Read<bool[]>(tag);
      Expect(val.Zip(val2, (a, b) => a == b)).ToNotContain(false);
    }

    [Test]
    public void FailWriteTest()
    {
      // fails for wrong tag
      Expect<Action>(() => _client.Write("WRONG TAG", (byte)3))
        .ToThrowException<OpcException>();

      // fails for wrong type
      Expect<Action>(() => _client.Write("Data.Static.Scalar.ByteValue", "WRONG TYPE"))
        .ToThrowException<OpcException>();

      // fails for not writing allowed
      Expect<Action>(() => _client.Write("Server.ServerStatus.BuildInfo.ManufacturerName", "READ ONLY"))
        .ToThrowException<OpcException>();
    }

    [Test]
    public void FailReadTest()
    {
      // fails for wrong tag
      Expect<Action>(() => _client.Read<int>("XXXXXX"))
        .ToThrowException<OpcException>();

      // fails for not readable tag
      Expect<Action>(() => _client.Read<string>("Server"))
        .ToThrowException<OpcException>();
    }

    [Test]
    public void BrowseFolderTest()
    {
      var node = _client.FindNode("Server.ServerStatus.BuildInfo");

      var subNodes = _client.ExploreFolder(node.Tag);
      Expect(subNodes.Count()).ToBe(6);
    }

    [Test]
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

    [Test]
    public void UaTestExtension()
    {
      var client = new TestExtendUaClient(new Uri(ConfigurationManager.AppSettings["UATestEndpoint"]));
      client.Connect();
      Assert.IsInstanceOf(typeof(Session), client.SessionExtended);
    }

    [Test]
    public void UaTestKeepAliveNotifyDisconnect()
    {
      var client = new TestExtendUaClient(new Uri(ConfigurationManager.AppSettings["UATestEndpoint"]));
      client.Connect();
      var i = 0;
      client.ServerConnectionLost += (object sender, EventArgs e) =>
      {
        i++;
      };
      /* Basicallly kill server by not letting any
       * operations complete */
      client.SessionExtended.OperationTimeout = 0;
      // make sure sessionkeepalive executes at least once
      client.SessionExtended.KeepAliveInterval = 10;
      // give ample time to call sessionkeepalive
      Thread.Sleep(100);
      /* 'i' should only be one because SessionKeepAlive
       * only calls ServerConnectionLost if Status
       * is connected. Before it calls
       * ServerConnectionLost, it sets Status to
       * NotConnected */
      Assert.AreEqual(1, i);
    }

    [Test]
    public void UaTestReConnect()
    {
      var client = new TestExtendUaClient(new Uri(ConfigurationManager.AppSettings["UATestEndpoint"]));
      /* Should throw error because session should not be
       * initialized without calling connect */
      Assert.Throws<System.NullReferenceException>(() => client.ReConnect());
    }

    [Test]
    public void UaTestSessionRecreate()
    {
      var client = new TestExtendUaClient(new Uri(ConfigurationManager.AppSettings["UATestEndpoint"]));
      client.Connect();
      var i = 0;
      Session oldSession = null;
      client.ServerConnectionLost += (object sender, EventArgs e) =>
      {
        i++;
        Assert.AreEqual(OpcStatus.NotConnected, client.Status);
        // store the session to make sure a new one is created
        oldSession = client.SessionExtended;
        client.RecreateSession();
        // put server back in good working order
        client.SessionExtended.OperationTimeout = 200;
        client.SessionExtended.KeepAliveInterval = 100;
      };
      client.SessionExtended.OperationTimeout = 0;
      client.SessionExtended.KeepAliveInterval = 10;
      Thread.Sleep(100);
      Assert.Greater(i, 0);
      // Give some time to call recreate
      Thread.Sleep(100);
      Assert.AreNotSame(oldSession, client.SessionExtended);
    }
  }
}
