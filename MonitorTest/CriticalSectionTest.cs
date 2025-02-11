using MonitorImplementation.HoareMonitor;

namespace MonitorTest
{
  [TestClass]
  public class CriticalSectionTest
  {
    [TestMethod]
    public void TestProtectedMethod()
    {
      // Prepare
      CriticalSectionTestClass criticalSectionTestClass = new CriticalSectionTestClass();

      // Act
      Thread threadProtected = new Thread(() =>
      {
        criticalSectionTestClass.HoareMonitorMethod();
      });

      threadProtected.Start();
      threadProtected.Join();

      // Test
      Assert.IsTrue(criticalSectionTestClass.checkValidity);

      // Dispose
      criticalSectionTestClass.Dispose();
    }

    [TestMethod]
    public void TestNotProtectedMethod()
    {
      // Prepare
      CriticalSectionTestClass criticalSectionTestClass = new CriticalSectionTestClass();

      // Act
      Thread threadProtected = new Thread(() =>
      {
        criticalSectionTestClass.TestMethod(); //TODO there is only one thread, so critical section is useless 
      });

      threadProtected.Start();
      threadProtected.Join();

      // Test
      Assert.IsTrue(criticalSectionTestClass.checkValidity);//TODO in case of not-protected method the checkValidity should be false ?

      // Dispose
      criticalSectionTestClass.Dispose();
    }

    private class CriticalSectionTestClass : HoareMonitorImplementation, IDisposable
    {
      internal void HoareMonitorMethod()
      {
        enterMonitorSection();
        try
        {
          TestMethod();
        }
        finally
        {
          exitHoareMonitorSection();
        }
      }

      private int intA = 0;
      private int intB = 0;
      private int counter = 0;

      internal void TestMethod()
      {
        for (int i = 0; i < 100000; i++)
        {
          counter++;
          intA = counter;
          intB = counter;
          if (intA - intB != 0)
          {
            checkValidity = false;
          }
        }
      }

      public bool checkValidity = true;

      public new void Dispose()
      {
        base.Dispose();
      }
    }
  }
}