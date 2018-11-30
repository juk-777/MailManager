using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using MailManager.Config;
using System.Diagnostics;
using MailManager.Monitor;
using MailManager.Action;
using System.Net.Mail;
using System;
using System.Text;
using Assert = NUnit.Framework.Assert;
using StringAssert = NUnit.Framework.StringAssert;

namespace MailManager.Tests
{
    [TestClass]
    public class MailManagerMonitorTest
    {
        private ConfigEntity ConfigEntity { get; set; }
        private MailEntity MailEntity { get; set; }
        private MailTransfer MailTransfer { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Debug.WriteLine("Test Initialize");

            MailEntity = new MailEntity();
            MailEntity.To.Add(new MailAddress("gus.guskovskij@mail.ru"));
            MailEntity.From = new MailAddress("juk_777@mail.ru");
            MailEntity.Subject = "1 письмо";
            MailEntity.Body = new StringBuilder("вап ваппррр");
            MailEntity.DateSent = DateTime.Now;

            ConfigEntity = new ConfigEntity
            {
                MailActions = new[]
                {
                    new Config.MailAction {ActType = ActionType.Notify, ActTypeValue = "yes"},
                    new Config.MailAction {ActType = ActionType.Print, ActTypeValue = "yes"},
                    new Config.MailAction {ActType = ActionType.CopyTo, ActTypeValue = "folder"},
                    new Config.MailAction {ActType = ActionType.Forward, ActTypeValue = "juk_777@mail.ru"},
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
                Password = "2TiraS2"
            };

            MailTransfer = new MailTransfer();
            MailTransfer.MailEntities.Add(MailEntity);
            MailTransfer.Uids.Add("1542704384292");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Debug.WriteLine("Test Cleanup");
        }

        [TestMethod]
        public void MailMonitor_GetAllMessages_WasCalled()
        {
            var mockMailProvider = new Mock<IMailProvider>();
            var mockMailAction = new Mock<IMailAction>();
            var mockSeenUidsManager = new Mock<ISeenUidsManager>();

            mockMailProvider
                .Setup(x => x.GetAllMessages(ConfigEntity))
                .Returns(MailTransfer);
            mockSeenUidsManager
                .Setup(x => x.Write(It.IsAny<ConfigEntity>(), It.IsAny<List<string>>(), It.IsAny<bool>()))
                .Returns(true);

            var mailMonitor = new MailMonitor(mockMailProvider.Object, mockMailAction.Object, mockSeenUidsManager.Object);
            mailMonitor.StartMonitorTask(ConfigEntity);

            mockMailProvider.Verify(x => x.GetAllMessages(ConfigEntity), Times.Once);
        }

        [TestMethod]
        public void MailMonitor_GetUnseenMessages_WasCalled()
        {
            var mockMailProvider = new Mock<IMailProvider>();
            var mockMailAction = new Mock<IMailAction>();
            var mockSeenUidsManager = new Mock<ISeenUidsManager>();

            mockMailProvider
                .Setup(x => x.GetAllMessages(ConfigEntity))
                .Returns(MailTransfer);
            mockMailProvider
                .Setup(x => x.GetUnseenMessages(ConfigEntity, It.IsAny<List<string>>()))
                .Returns(MailTransfer);
            mockSeenUidsManager
                .Setup(x => x.Write(It.IsAny<ConfigEntity>(), It.IsAny<List<string>>(), It.IsAny<bool>()))
                .Returns(true);

            var mailMonitor = new MailMonitor(mockMailProvider.Object, mockMailAction.Object, mockSeenUidsManager.Object);
            mailMonitor.StartMonitorTask(ConfigEntity);

            mockMailProvider.Verify(x => x.GetUnseenMessages(ConfigEntity, It.IsAny<List<string>>()), Times.AtLeastOnce);
        }

        [TestMethod]
        public void MailMonitor_StartMonitorTask_Verify()
        {
            var mockMailProvider = new Mock<IMailProvider>();
            var mockMailAction = new Mock<IMailAction>();
            var mockSeenUidsManager = new Mock<ISeenUidsManager>();

            mockMailProvider
                .Setup(x => x.GetAllMessages(ConfigEntity))
                .Returns(MailTransfer);
            mockMailProvider
                .Setup(x => x.GetUnseenMessages(ConfigEntity, It.IsAny<List<string>>()))
                .Returns(MailTransfer);
            mockSeenUidsManager
                .Setup(x => x.Write(It.IsAny<ConfigEntity>(), It.IsAny<List<string>>(), It.IsAny<bool>()))
                .Returns(true);
            mockSeenUidsManager
                .Setup(x => x.Read(It.IsAny<ConfigEntity>()))
                .Returns(It.IsAny<List<string>>());

            var mailMonitor = new MailMonitor(mockMailProvider.Object, mockMailAction.Object, mockSeenUidsManager.Object);
            mailMonitor.StartMonitorTask(ConfigEntity);

            mockMailProvider.Verify();
            mockSeenUidsManager.Verify();
        }

        [TestMethod]
        public void MailMonitor_MailActions_WasCalled()
        {
            var mockMailProvider = new Mock<IMailProvider>();
            var mockMailAction = new Mock<IMailAction>();
            var mockSeenUidsManager = new Mock<ISeenUidsManager>();

            mockMailProvider
                .Setup(x => x.GetAllMessages(ConfigEntity))
                .Returns(MailTransfer);
            mockMailProvider
                .Setup(x => x.GetUnseenMessages(It.IsAny<ConfigEntity>(), It.IsAny<List<string>>()))
                .Returns(new MailTransfer());
            mockSeenUidsManager
                .Setup(x => x.Write(It.IsAny<ConfigEntity>(), It.IsAny<List<string>>(), It.IsAny<bool>()))
                .Returns(true);
            mockSeenUidsManager
                .Setup(x => x.Read(It.IsAny<ConfigEntity>()))
                .Returns(It.IsAny<List<string>>());

            var mailMonitor = new MailMonitor(mockMailProvider.Object, mockMailAction.Object, mockSeenUidsManager.Object);
            mailMonitor.StartMonitorTask(ConfigEntity);

            mockMailAction.Verify(x => x.ActionSend(ConfigEntity, MailEntity, ConfigEntity.MailActions[3].ActTypeValue), Times.Once());
            mockMailAction.Verify(x => x.ActionCopy(ConfigEntity, MailEntity, ConfigEntity.MailActions[2].ActTypeValue), Times.Once());
            mockMailAction.Verify(x => x.ActionNotify(MailEntity), Times.Once());
            mockMailAction.Verify(x => x.ActionPrint(MailEntity), Times.Once());
        }

        [TestMethod]
        public void MailMonitor_StartMonitorTask_SeenUidsManager_Write_ExceptionThrown()
        {
            var mockMailProvider = new Mock<IMailProvider>();
            var mockMailAction = new Mock<IMailAction>();
            var mockSeenUidsManager = new Mock<ISeenUidsManager>();

            mockMailProvider
                .Setup(x => x.GetAllMessages(ConfigEntity))
                .Returns(MailTransfer);
            mockSeenUidsManager
                .Setup(x => x.Write(It.IsAny<ConfigEntity>(), It.IsAny<List<string>>(), It.IsAny<bool>()))
                .Returns(false);

            var mailMonitor = new MailMonitor(mockMailProvider.Object, mockMailAction.Object, mockSeenUidsManager.Object);

            string message = "Ошибка при сохранении Uid прочитанных писем";
            var ex = Assert.Throws<ApplicationException>(() => mailMonitor.StartMonitorTask(ConfigEntity));

            StringAssert.Contains(message, ex.Message);
        }

        [TestMethod]
        public void MailMonitor_StartMonitorTask_SeenUidsManager_Verify()
        {
            var mockMailProvider = new Mock<IMailProvider>();
            var mockMailAction = new Mock<IMailAction>();
            var mockSeenUidsManager = new Mock<ISeenUidsManager>();

            mockMailProvider
                .Setup(x => x.GetAllMessages(ConfigEntity))
                .Returns(MailTransfer);
            mockMailProvider
                .Setup(x => x.GetUnseenMessages(It.IsAny<ConfigEntity>(), It.IsAny<List<string>>()))
                .Returns(MailTransfer);
            mockSeenUidsManager
                .Setup(x => x.Write(It.IsAny<ConfigEntity>(), It.IsAny<List<string>>(), It.IsAny<bool>()))
                .Returns(true);
            mockSeenUidsManager
                .Setup(x => x.Read(It.IsAny<ConfigEntity>()))
                .Returns(It.IsAny<List<string>>());

            var mailMonitor = new MailMonitor(mockMailProvider.Object, mockMailAction.Object, mockSeenUidsManager.Object);
            mailMonitor.StartMonitorTask(ConfigEntity);

            mockSeenUidsManager.Verify();
        }
    }
}
