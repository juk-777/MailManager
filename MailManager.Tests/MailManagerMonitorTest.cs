using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using MailManager.Config;
using System.Diagnostics;
using MailManager.Monitor;
using MailManager.Action;

namespace MailManager.Tests
{
    [TestClass]
    public class MailManagerMonitorTest
    {
        public ConfigEntity ConfigEntity { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Debug.WriteLine("Test Initialize");

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
                Password = "pas"
            };
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

            var mailMonitor = new MailMonitor(mockMailProvider.Object, mockMailAction.Object);
            mailMonitor.StartMonitorTask(new ConfigEntity());

            mockMailProvider.Verify(x => x.GetAllMessages(It.IsAny<ConfigEntity>()), Times.AtLeastOnce);
            mockMailAction.VerifyAll();
        }

        [TestMethod]
        public void MailMonitor_StartMonitor_Verify()
        {
            var mockMailProvider = new Mock<IMailProvider>();
            var mockMailAction = new Mock<IMailAction>();            

            List<ConfigEntity> configEntities = new List<ConfigEntity>();
            configEntities.Add(ConfigEntity);

            var mailMonitor = new MailMonitor(mockMailProvider.Object, mockMailAction.Object);

            mailMonitor.StartMonitor(configEntities);

            mockMailProvider.VerifyAll();
            mockMailAction.VerifyAll();
        }
    }
}
