using System;
using System.Linq;
using System.Threading;
using Hylasoft.Behavior;
using Hylasoft.Behavior.Extensions;
using Hylasoft.Opc.Common;
using Hylasoft.Opc.Da;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using OpcDa = Opc.Da;
using Moq;

namespace Hylasoft.Opc.Tests
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test Class"), TestFixture]
  public class DaTest : Spec
  {
    private DaClient _client;

    [SetUp]
    public void Init()
    {
      _client = new DaClient(new Uri("opcda://localhost/Graybox.Simulator"));
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
      var node = _client.FindNode("numeric.random.double");
      Expect(node).ToNotBeNull();
    }
    [Test]
    public void DaReadDouble()
    {
      var val = _client.Read<double>("numeric.random.double");
      Expect(val).ToBeInstanceOf(typeof(double));
    }
  }
}
