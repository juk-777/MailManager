using MailManager.BusinessLogic;
using MailManager.Config;
using MailManager.Monitor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Assert = NUnit.Framework.Assert;
using StringAssert = NUnit.Framework.StringAssert;

namespace MailManager.Tests
{
    [TestClass]
    public class MailManagerBusinessLogicTest
    {
        private CancellationTokenSource Cts { get; set; }
        private CancellationToken Token { get; set; }
        private ConfigEntity ConfigEntity { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Debug.WriteLine("Test Initialize");
            Cts = new CancellationTokenSource();
            Token = Cts.Token;

            ConfigEntity = new ConfigEntity
            {
                MailActions = new[]
                {
                    new MailAction {ActType = ActionType.Notify, ActTypeValue = "yes"},
                    new MailAction {ActType = ActionType.Print, ActTypeValue = "yes"},
                    new MailAction {ActType = ActionType.CopyTo, ActTypeValue = "folder"},
                    new MailAction {ActType = ActionType.Forward, ActTypeValue = "juk_777@mail.ru"},
                },
                IdentityMessages = new[]
                {
                    new IdentityMessage {IdType = IdentityType.To, IdTypeValue = "gus.guskovskij@mail.ru"},
                    new IdentityMessage {IdType = IdentityType.From, IdTypeValue = "juk_777@mail.ru"},
                    new IdentityMessage {IdType = IdentityType.Title, IdTypeValue = "пис"},
                    new IdentityMessage {IdType = IdentityType.Body, IdTypeValue = "вап"}
                },
                Mail = "pop.yandex.ru",
                Port = 995,
                Login = "tiras.school.2",
                Password = "pas"
            };
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Debug.WriteLine("Test Cleanup");
        }

        [TestMethod]
        public async Task StartJob_CancellationRequested()
        {
            var mockConfigReader = new Mock<IConfigReader>();
            var mockConfigVerifier = new Mock<IConfigVerifier>();
            var mockMailMonitor = new Mock<IMailMonitor>();

            var businessLogic = new MailBusinessLogic(mockConfigReader.Object, mockConfigVerifier.Object, mockMailMonitor.Object);

            Cts.Cancel();
            await businessLogic.StartJob(Token);

            mockConfigReader.Verify(x => x.ReadConfig(), Times.Never);
            mockConfigVerifier.Verify(x => x.Verify(It.IsAny<List<ConfigEntity>>()), Times.Never);
            mockMailMonitor.Verify(x => x.StartMonitor(It.IsAny<List<ConfigEntity>>()), Times.Never);
        }

        [TestMethod]
        public async Task StartJob_ReadConfig_Verify()
        {
            var mockConfigReader = new Mock<IConfigReader>();
            var mockConfigVerifier = new Mock<IConfigVerifier>();
            var mockMailMonitor = new Mock<IMailMonitor>();

            List<ConfigEntity> configEntityList = new List<ConfigEntity> { ConfigEntity };

            mockConfigReader
                .Setup(x => x.ReadConfig())
                .Returns(configEntityList);
            mockConfigVerifier
                .Setup(x => x.Verify(configEntityList))
                .Returns(true);

            var businessLogic = new MailBusinessLogic(mockConfigReader.Object, mockConfigVerifier.Object, mockMailMonitor.Object);            
            await businessLogic.StartJob(Token);

            mockConfigReader.Verify();
        }

        [TestMethod]
        public void StartJob_ConfEntitysNotCreated_ExceptionThrown_Null()
        {
            var mockConfigReader = new Mock<IConfigReader>();
            var mockConfigVerifier = new Mock<IConfigVerifier>();
            var mockMailMonitor = new Mock<IMailMonitor>();

            mockConfigReader
                .Setup(x => x.ReadConfig())
                .Returns<List<ConfigEntity>>(null);

            var businessLogic = new MailBusinessLogic(mockConfigReader.Object, mockConfigVerifier.Object, mockMailMonitor.Object);
            string message = "Файл конфигурации пуст!";
            var ex = Assert.ThrowsAsync<ArgumentException>(() => businessLogic.StartJob(Token));

            StringAssert.Contains(message, ex.Message);
        }

        [TestMethod]
        public void StartJob_ConfEntitysNotCreated_ExceptionThrown_ZeroValues()
        {
            var mockConfigReader = new Mock<IConfigReader>();
            var mockConfigVerifier = new Mock<IConfigVerifier>();
            var mockMailMonitor = new Mock<IMailMonitor>();

            mockConfigReader
                .Setup(x => x.ReadConfig())
                .Returns(new List<ConfigEntity>());

            var businessLogic = new MailBusinessLogic(mockConfigReader.Object, mockConfigVerifier.Object, mockMailMonitor.Object);
            string message = "Файл конфигурации пуст!";
            var ex = Assert.ThrowsAsync<ArgumentException>(() => businessLogic.StartJob(Token));

            StringAssert.Contains(message, ex.Message);
        }

        [TestMethod]
        public async Task StartJob_Verify_Config()
        {
            var mockConfigReader = new Mock<IConfigReader>();
            var mockConfigVerifier = new Mock<IConfigVerifier>();
            var mockMailMonitor = new Mock<IMailMonitor>();

            List<ConfigEntity> configEntityList = new List<ConfigEntity> { ConfigEntity };

            mockConfigReader
                .Setup(x => x.ReadConfig())
                .Returns(configEntityList);
            mockConfigVerifier
                .Setup(x => x.Verify(configEntityList))
                .Returns(true);

            var businessLogic = new MailBusinessLogic(mockConfigReader.Object, mockConfigVerifier.Object, mockMailMonitor.Object);
            await businessLogic.StartJob(Token);

            mockConfigVerifier.Verify();
        }

        [TestMethod]
        public void StartJob_Verify_Config_ExceptionThrown_ReturnFalse()
        {
            var mockConfigReader = new Mock<IConfigReader>();
            var mockConfigVerifier = new Mock<IConfigVerifier>();
            var mockMailMonitor = new Mock<IMailMonitor>();

            List<ConfigEntity> configEntityList = new List<ConfigEntity> { ConfigEntity };

            mockConfigReader
                .Setup(x => x.ReadConfig())
                .Returns(configEntityList);
            mockConfigVerifier
                .Setup(x => x.Verify(configEntityList))
                .Returns(false);

            var businessLogic = new MailBusinessLogic(mockConfigReader.Object, mockConfigVerifier.Object, mockMailMonitor.Object);
            string message = "Проверка завершена с ошибкой!";
            var ex = Assert.ThrowsAsync<ArgumentException>(() => businessLogic.StartJob(Token));

            StringAssert.Contains(message, ex.Message);
        }

        [TestMethod]
        public async Task StartJob_StartMonitor_WasCalled()
        {
            var mockConfigReader = new Mock<IConfigReader>();
            var mockConfigVerifier = new Mock<IConfigVerifier>();
            var mockMailMonitor = new Mock<IMailMonitor>();

            List<ConfigEntity> configEntityList = new List<ConfigEntity> {new ConfigEntity()};

            mockConfigReader
                .Setup(x => x.ReadConfig())
                .Returns(configEntityList);
            mockConfigVerifier
                .Setup(x => x.Verify(configEntityList))
                .Returns(true);

            var businessLogic = new MailBusinessLogic(mockConfigReader.Object, mockConfigVerifier.Object, mockMailMonitor.Object);
            await businessLogic.StartJob(Token);

            mockMailMonitor.Verify(x => x.StartMonitor(It.IsAny<List<ConfigEntity>>()), Times.Once());
        }

        [TestMethod]
        public void Dispose_MailMonitorDispose_WasCalled()
        {
            var mockConfigReader = new Mock<IConfigReader>();
            var mockConfigVerifier = new Mock<IConfigVerifier>();
            var mockMailMonitor = new Mock<IMailMonitor>();

            var businessLogic = new MailBusinessLogic(mockConfigReader.Object, mockConfigVerifier.Object, mockMailMonitor.Object);
            businessLogic.Dispose();

            mockMailMonitor.Verify(x => x.Dispose(), Times.Once());
        }
    }
}
