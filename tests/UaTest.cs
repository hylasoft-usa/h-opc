using System;
using System.Linq;
using System.Threading;
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
  public class UaTest
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
      Assert.AreEqual(OpcStatus.Connected, _client.Status);
    }

    [Test]
    public void FindNodeTest()
    {
      var node = _client.FindNode("Data.Dynamic.Scalar.SByteValue");
      Assert.NotNull(node);
    }

    [Test]
    public void ReadNodeTest()
    {
      var val = _client.Read<string>("Server.ServerStatus.BuildInfo.ManufacturerName");
      Assert.AreEqual("OPC Foundation", val.Value);
    }

    [Test]
    public void WriteNodeTest()
    {
      const string tag = "Data.Static.Scalar.ByteValue";

      _client.Write(tag, (byte)3);
      var val = _client.Read<byte>(tag);
      Assert.AreEqual(3, val.Value);

      _client.Write(tag, (byte)13);
      val = _client.Read<byte>(tag);
      Assert.AreEqual(13, val.Value);
    }


    [Test]
    public void ReadArrayNodeTest()
    {
      var val = _client.Read<bool[]>("Data.Static.Array.BooleanValue");
      Assert.Greater(val.Value.Length, 0);
    }

    [Test]
    public void WriteArrayNodeTest()
    {
      const string tag = "Data.Static.Array.BooleanValue";

      var val1 = new[] { true, false, false };
      var val2 = new[] { false, false };

      _client.Write(tag, val1);
      var val = _client.Read<bool[]>(tag);
      Assert.True(!val.Value.Zip(val1, (a, b) => a == b).Contains(false));

      _client.Write(tag, val2);
      val = _client.Read<bool[]>(tag);
      Assert.True(!val.Value.Zip(val2, (a, b) => a == b).Contains(false));
    }

    [Test]
    public void FailWriteTest()
    {
      // fails for wrong tag
      Assert.Throws<OpcException>(() => _client.Write("WRONG TAG", (byte)3));

      // fails for wrong type
      Assert.Throws<OpcException>(() => _client.Write("Data.Static.Scalar.ByteValue", "WRONG TYPE"));

      // fails for not writing allowed
      Assert.Throws<OpcException>(() => _client.Write("Server.ServerStatus.BuildInfo.ManufacturerName", "READ ONLY"));
    }

    [Test]
    public void FailReadTest()
    {
      // fails for wrong tag
      Assert.Throws<OpcException>(() => _client.Read<int>("XXXXXX"));

      // fails for not readable tag
      Assert.AreEqual(Quality.Bad, _client.Read<string>("Server").Quality);
    }

    [Test]
    public void BrowseFolderTest()
    {
      var node = _client.FindNode("Server.ServerStatus.BuildInfo");

      var subNodes = _client.ExploreFolder(node.Tag);
      Assert.AreEqual(6, subNodes.Count());
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
      Assert.AreEqual(3, executed);
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
        if (i > 0) return;
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

    [Test]
    public void UaTestReadTagAsync()
    {
      var task = _client.ReadAsync<string>("Server.ServerStatus.BuildInfo.ManufacturerName");
      task.Wait();
      Assert.AreEqual("OPC Foundation", task.Result.Value);
    }

    [Test]
    public void UaWriteAsync()
    {
      const string tag = "Data.Static.Scalar.ByteValue";
      var task = _client.WriteAsync(tag, (byte)3);
      var i = 0;
      /* task.Wait broken because task is never set
       * unless exception */
      Task.Run(() =>
      {
        task.Wait();
        i++;
      });
      Thread.Sleep(200);
      Assert.AreEqual(1, i);
      var val = _client.Read<byte>(tag);
      Assert.AreEqual(3, val.Value);

      task = _client.WriteAsync(tag, (byte)13);
      task.Wait();
      val = _client.Read<byte>(tag);
      Assert.AreEqual(13, val.Value);
    }
    [Test]
    public void DisposeWithoutException()
    {
      var nodes = _client.ExploreFolder("Data");
      foreach (var node in nodes)
      {
        _client.Monitor<object>(node.Tag, (value, unsubscribe) =>
        {
          Console.WriteLine("{0}:={1}", node.Name, value);
        });
      }
      Thread.Sleep(500);
      _client.Dispose();
    }
    [Test]
    public void UaGetDataType()
    {
      var type = _client.GetDataType("Data.Dynamic.Scalar.Int32Value");
      Assert.AreEqual(typeof(int), type);
      type = _client.GetDataType("Data.Dynamic.Scalar.Int16Value");
      Assert.AreEqual(typeof(short), type);
    }
  }
}
