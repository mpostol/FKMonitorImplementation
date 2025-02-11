//TODO remove all useless using directives
using MonitorImplementation.HoareMonitor;

namespace MonitorTest
{
  [TestClass]
  public class BoundedBufferTest
  {
    [TestMethod]
    public void TestAddItem()
    {
      // Prepare
      BoundedBuffer buffer = new BoundedBuffer();
      int item = 0;

      // Act
      Thread threadAdd = new Thread(() =>
      {
        buffer.AddItem(2);
        item = buffer.RemoveItem();
      });

      threadAdd.Start();
      threadAdd.Join();

      // Test
      Assert.AreEqual(2, item);

      // Dispose
      buffer.Dispose();
    }

    [TestMethod]
    public void TestRemoveItem()
    {
      // Prepare
      BoundedBuffer buffer = new BoundedBuffer();
      int item = 0;

      Thread threadRemove = new Thread(() =>
      {
        item = buffer.RemoveItem();
      });

      Thread threadAdd = new Thread(() =>
      {
        buffer.AddItem(2);
      });

      // Act
      threadRemove.Start();
      threadAdd.Start();
      // TODO before testing we must wait for finishing the threads
      // Test
      Assert.AreEqual(2, item);

      // Dispose
      buffer.Dispose();
    }

    [TestMethod]
    public void TestBufferIsEmpty() //TODO the test must be revised
    {
      // Prepare
      BoundedBuffer buffer = new BoundedBuffer();
      const int count = 10; // TODO must be much greater than the buffer size
      const int sleepTime = 100; 
      bool isTrue = true;

      // Act
      Thread threadAdd = new Thread(() =>
      {
        for (int i = 0; i < count + 1; i++)
        {
          Thread.Sleep(sleepTime); //TODO buffer full condition requires that the producer is much faster than the consumer - the wrong location of the Sleep method
          buffer.AddItem(count); //TODO it must be recognized as producer
        }
      });

      threadAdd.Start();
      threadAdd.Join(); //TODO it prevents concurrent adding and removing items
      if (buffer.RemoveItem() != 9)
      {
        isTrue = false;
      }

      // Test // //TODO isFull must be tested
      Assert.IsTrue(buffer.isEmpty);

      // Dispose
      buffer.Dispose();
    }

    // TODO Are we using aspect programming here? If so, how?
    // TODO the BoundedBuffer must be generic declaration
    private class BoundedBuffer : HoareMonitorImplementation, IDisposable
    {
      private readonly ISignal? nonempty;
      private readonly ISignal? nonfull;
      private const int N = 10;
      private readonly int[] buffer = new int[N];
      private int lastPointer = 0;
      private int count = 0;
      internal bool isfull = false;  //TODO isfull is never used
      internal bool isEmpty = false;

      public BoundedBuffer()
      {
        nonempty = CreateSignal();
        nonfull = CreateSignal();
      }

      internal void AddItem(int x)
      {
        enterMonitorSection(); //Are we using aspect programming here? If so, how?
        try
        {
          if (count == N)
          {
            isfull |= true;  //TODO isfull is never used ? As far as I remember it is to be used for testing a scenario where the producer is faster than consument
            nonfull.Wait();
          }

          buffer[lastPointer] = x;
          lastPointer = (lastPointer + 1) % N;
          count++;

          nonempty.Send();
        }
        finally
        {
          exitHoareMonitorSection();
        }
      }

      internal int RemoveItem()
      {
        enterMonitorSection();
        try
        {
          if (count == 0)
          {
            isEmpty |= true; 
            nonempty.Wait();
          }

          int x = buffer[(lastPointer - count + N) % N];
          count--;

          nonfull.Send();
          return x;
        }
        finally
        {
          exitHoareMonitorSection();
        }
      }

      public new void Dispose()
      {
        base.Dispose();
      }
    }
  }
}