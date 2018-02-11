using System;
using System.Linq;
using System.Threading;
using Hylasoft.Opc.Common;
using Hylasoft.Opc.Da;
using NUnit.Framework;
using OpcDa = Opc.Da;
using System.Threading.Tasks;

namespace Hylasoft.Opc.Tests
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test Class"), TestFixture]
  public class DaTest
  {
    private DaClient _client;
    private const string TestRegister = "storage.numeric.reg06";
    private const string ClientUrl = "opcda://localhost/Graybox.Simulator";

    [SetUp]
    public void Init()
    {
      _client = new DaClient(new Uri(ClientUrl));
      _client.Connect();
      // have to assign to graybox simulation items once to activate them
      _client.Write<double>(TestRegister, 4);
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
      var node = _client.FindNode(TestRegister);
      Assert.NotNull(node);
    }
    [Test]
    public void DaReadDouble()
    {
      var val = _client.Read<double>(TestRegister);
      Assert.AreEqual(4, val.Value);
    }
    [Test]
    public void DaReadAsyncDouble()
    {
      var task = _client.ReadAsync<double>(TestRegister);
      task.Wait();
      Assert.AreEqual(4, task.Result.Value);
      task = _client.ReadAsync<double>(TestRegister);
      // didn't wait
      Assert.False(task.IsCompleted);
    }
    [Test]
    public void DaReadWrongType()
    {
      Assert.Throws<InvalidCastException>(() =>
      {
        _client.Read<bool>(TestRegister);
      });
    }
    [Test]
    public void DaTestExtend()
    {
      var extendedClient = new TestExtendDaClient(new Uri(ClientUrl));
      extendedClient.Connect();
      Assert.AreEqual(typeof(OpcDa.Server), extendedClient.ExposedServer.GetType());
      extendedClient.Dispose();
    }
    [Test]
    public void DaMonitor()
    {
      var executed = 0;
      var tag = TestRegister;
      _client.Monitor<double>(tag, (val1, u) =>
      {
        executed++;
        u();
      });
      _client.Monitor<double>(tag, (val1, u) =>
      {
        executed++;
        u();
      });
      _client.Monitor<double>(tag, (val1, u) =>
      {
        executed++;
        u();
      });
      const int interval = 100;
      Thread.Sleep(interval);
      _client.Write(tag, 10);
      Thread.Sleep(interval);
      _client.Write(tag, 11);
      Thread.Sleep(interval);
      _client.Write(tag, 12);
      Thread.Sleep(interval);
      _client.Write(tag, 13);
      Thread.Sleep(interval);
      Assert.AreEqual(3, executed);
    }
    [Test]
    public void DaExploreFolder()
    {
      var rootTags = _client.ExploreFolder(string.Empty);
      Assert.Greater(rootTags.Count(), 0);
    }
    [Test]
    public void DaGetDataType()
    {
      var type = _client.GetDataType(TestRegister);
      Assert.AreEqual(typeof(short), type);
    }
  }
}
