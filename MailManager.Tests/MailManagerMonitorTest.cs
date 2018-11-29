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
            MailTransfer.Uids.Add("1540884182881");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Debug.WriteLine("Test Cleanup");
        }

        [TestMethod]
        public void MailMonitor_StartMonitorTask_Verify()
        {
            var mockMailProvider = new Mock<IMailProvider>();
            var mockMailAction = new Mock<IMailAction>();
            var mockSaverSeenUids = new Mock<ISaveSeenUids>();
            var mockReaderSeenUids = new Mock<IReadSeenUids>();

            mockMailProvider
                .Setup(x => x.GetAllMessages(ConfigEntity))
                .Returns(MailTransfer);

            mockMailProvider
                .Setup(x => x.GetUnseenMessages(It.IsAny<ConfigEntity>(), It.IsAny<List<string>>()))
                .Returns(MailTransfer);

            mockSaverSeenUids
                .Setup(x => x.Save(It.IsAny<ConfigEntity>(), It.IsAny<List<string>>(), It.IsAny<bool>()))
                .Returns(true);

            mockReaderSeenUids
                .Setup(x => x.Read(It.IsAny<ConfigEntity>()))
                .Returns(It.IsAny<List<string>>());

            var mailMonitor = new MailMonitor(mockMailProvider.Object, mockMailAction.Object, mockSaverSeenUids.Object, mockReaderSeenUids.Object);
            mailMonitor.StartMonitorTask(ConfigEntity);

            mockMailProvider.Verify(x => x.GetAllMessages(It.IsAny<ConfigEntity>()), Times.AtLeastOnce);
            mockMailProvider.Verify(x => x.GetUnseenMessages(It.IsAny<ConfigEntity>(), It.IsAny<List<string>>()), Times.AtLeastOnce);
            mockMailAction.VerifyAll();
        }

        [TestMethod]
        public void MailMonitor_StartMonitor_Verify()
        {
            var mockMailProvider = new Mock<IMailProvider>();
            var mockMailAction = new Mock<IMailAction>();
            var mockSaverSeenUids = new Mock<ISaveSeenUids>();
            var mockReaderSeenUids = new Mock<IReadSeenUids>();

            List<ConfigEntity> configEntities = new List<ConfigEntity> { ConfigEntity };

            var mailMonitor = new MailMonitor(mockMailProvider.Object, mockMailAction.Object, mockSaverSeenUids.Object, mockReaderSeenUids.Object);

            mailMonitor.StartMonitor(configEntities);

            mockMailProvider.VerifyAll();
            mockMailAction.VerifyAll();
        }

        [TestMethod]
        public void MailMonitor_Dispose_Verify()
        {
            var mockMailProvider = new Mock<IMailProvider>();
            var mockMailAction = new Mock<IMailAction>();
            var mockSaverSeenUids = new Mock<ISaveSeenUids>();
            var mockReaderSeenUids = new Mock<IReadSeenUids>();

            var mailMonitor = new MailMonitor(mockMailProvider.Object, mockMailAction.Object, mockSaverSeenUids.Object, mockReaderSeenUids.Object);
            mailMonitor.Dispose();

            mockMailProvider.VerifyAll();
            mockMailAction.VerifyAll();
        }

        [TestMethod]
        public void OpenPopProvider_GetAllMessages_Verify()
        {
            var openPop = new OpenPopProvider();
            var mailTransfer = openPop.GetAllMessages(ConfigEntity);

            Assert.IsNotEmpty(mailTransfer.MailEntities);
            Assert.IsNotEmpty(mailTransfer.Uids);
        }

        [TestMethod]
        public void OpenPopProvider_GetUnseenMessages_Verify()
        {
            var openPop = new OpenPopProvider();
            var mailTransfer = openPop.GetUnseenMessages(ConfigEntity, new List<string>());

            Assert.IsNotEmpty(mailTransfer.MailEntities);
            Assert.IsNotEmpty(mailTransfer.Uids);
        }
    }
}
