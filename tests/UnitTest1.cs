using Hylasoft.Behavior;
using Hylasoft.Behavior.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tests
{
  [TestClass]
  public class UnitTest1 : Spec
  {
    [TestMethod]
    public void TestMethod1()
    {
      Expect(true).ToBeTrue();
    }
  }
}
