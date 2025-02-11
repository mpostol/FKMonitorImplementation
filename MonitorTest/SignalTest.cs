using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MonitorImplementation.HoareMonitor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MonitorTest
{
    [TestClass]
    public class SignalTest
    {
        [TestMethod]
        public void TestWait()
        {
            SignalTestClass signalTestClass = new();
            // Prepare
            bool isFalse = false;
            signalTestClass.CreateTheSignal();

            Thread thread = new Thread(() =>
            {
                signalTestClass.WaitSignal();
                isFalse = true;
            });

            // Act
            thread.Start();
            Thread.Sleep(100);
            thread.Join();// TODO if there is no send we should wait here forever

            // Test
            Assert.IsFalse(isFalse);

            // Dispose
            signalTestClass.Dispose();  //TODO replace explicit Dispose invocation by the using instruction
        }

        [TestMethod]
        public void TestSignal()
        {
            // Prepare
            SignalTestClass signalTestClass2 = new();
            bool isTrue = false;
            signalTestClass2.CreateTheSignal();

            Thread thread = new Thread(() =>
            {
                signalTestClass2.WaitSignal();
                isTrue = true;
            });

            // Act
            thread.Start();
            Thread.Sleep(100);
            signalTestClass2.SendTheSignal();
            thread.Join();

            // Test
            Assert.IsTrue(isTrue);

            // Dispose
            signalTestClass2.Dispose();
        }

        private class SignalTestClass : HoareMonitorImplementation, IDisposable
        {
            private ISignal? signal;

            internal void CreateTheSignal()
            {
                enterMonitorSection();
                try
                {
                    signal = CreateSignal();
                }
                finally
                {
                    exitHoareMonitorSection();
                }
            }

            internal void SendTheSignal()
            {
                enterMonitorSection();
                try
                {
                    signal?.Send();
                }
                finally
                {
                    exitHoareMonitorSection();
                }
            }

            internal void WaitSignal()
            {
                enterMonitorSection();
                try
                {
                    signal?.Wait();
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