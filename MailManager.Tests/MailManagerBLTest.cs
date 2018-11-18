using MailManager.BL;
using MailManager.Config;
using MailManager.Monitor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Assert = NUnit.Framework.Assert;

namespace MailManager.Tests
{
    [TestClass]
    public class MailManagerBLTest
    {
        public CancellationTokenSource Cts { get; set; }        
        public CancellationToken Token { get; set; }         

        [TestInitialize]
        public void TestInitialize()
        {
            Debug.WriteLine("Test Initialize");
            Cts = new CancellationTokenSource();
            Token = Cts.Token;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Debug.WriteLine("Test Cleanup");
        }

        [TestMethod]
        public void StartJob_ConfEntitysNotCreated_ExceptionThrown_Null()
        {
            var mockConfigReader = new Mock<IConfigReader>();
            var mockMailMonitor = new Mock<IMailMonitor>();

            mockConfigReader
                .Setup(x => x.ReadConfig())              // при вызове метода ReadConfig 
                .Returns<List<ConfigEntity>>(null);     // должно возвращаться значение null

            var mmBL = new MailManagerBL(mockConfigReader.Object, mockMailMonitor.Object);

            // тест закончится успешно только в том случае если будет выброшено исключение типа ApplicationException
            Assert.ThrowsAsync<ApplicationException>(() => mmBL.StartJob(Token));
        }

        [TestMethod]
        public void StartJob_ConfEntitysNotCreated_ExceptionThrown_ZeroValues()
        {
            var mockConfigReader = new Mock<IConfigReader>();
            var mockMailMonitor = new Mock<IMailMonitor>();

            mockConfigReader
                .Setup(x => x.ReadConfig())
                .Returns(new List<ConfigEntity>());

            var mmBL = new MailManagerBL(mockConfigReader.Object, mockMailMonitor.Object);

            // тест закончится успешно только в том случае если будет выброшено исключение типа ApplicationException
            Assert.ThrowsAsync<ApplicationException>(() => mmBL.StartJob(Token));
        }

        [TestMethod]
        public void StartJob_CancellationRequested()
        {
            var mockConfigReader = new Mock<IConfigReader>();
            var mockMailMonitor = new Mock<IMailMonitor>();

            mockConfigReader
                .Setup(x => x.ReadConfig())
                .Returns(It.IsAny<List<ConfigEntity>>());

            var mmBL = new MailManagerBL(mockConfigReader.Object, mockMailMonitor.Object);
            Cts.Cancel();

            mmBL.StartJob(Token);
        }

        [TestMethod]
        public void StartJob_StartMonitor_WasCalled()
        {
            var mockConfigReader = new Mock<IConfigReader>();
            var mockMailMonitor = new Mock<IMailMonitor>();

            List<ConfigEntity> configEntityList = new List<ConfigEntity>();
            configEntityList.Add(new ConfigEntity());

            mockConfigReader
                .Setup(x => x.ReadConfig())
                .Returns(configEntityList);

            //mockMailMonitor
            //    .Setup(x => x.StartMonitor(It.IsAny<List<ConfigEntity>>()));

            var mmBL = new MailManagerBL(mockConfigReader.Object, mockMailMonitor.Object);
            mmBL.StartJob(Token);

            mockMailMonitor.Verify(x => x.StartMonitor(It.IsAny<List<ConfigEntity>>()), Times.Once());
        }

        [TestMethod]
        public void StopJob_StopMonitor_WasCalled()
        {
            var mockConfigReader = new Mock<IConfigReader>();
            var mockMailMonitor = new Mock<IMailMonitor>();

            var mmBL = new MailManagerBL(mockConfigReader.Object, mockMailMonitor.Object);
            mmBL.StopJob();

            mockMailMonitor.Verify(x => x.StopMonitor(), Times.Once());
        }
    }
}
