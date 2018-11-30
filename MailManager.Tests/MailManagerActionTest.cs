using MailManager.Action;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Text;
using MailManager.Config;
using Assert = NUnit.Framework.Assert;
using MailManager.Monitor;
using System.Diagnostics;
using System.Net.Mail;
using NUnit.Framework;

namespace MailManager.Tests
{
    [TestClass]
    public class MailManagerActionTest
    {
        private MailEntity MailEntity { get; set; }
        private ConfigEntity ConfigEntity { get; set; }

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
                Password = "pas"
            };
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Debug.WriteLine("Test Cleanup");
        }

        [TestMethod]
        public void MailAction_Actions_WasCalled()
        {
            var mockMailSender = new Mock<IMailSender>();
            var mockMailCopy = new Mock<IMailCopy>();
            var mockNotify = new Mock<INotify>();
            var mockPrint = new Mock<IPrint>();            

            var mailAction = new Action.MailAction(mockMailSender.Object, mockMailCopy.Object, mockNotify.Object, mockPrint.Object);
            mailAction.ActionSend(ConfigEntity, MailEntity, ConfigEntity.MailActions[3].ActTypeValue);
            mailAction.ActionCopy(ConfigEntity, MailEntity, ConfigEntity.MailActions[2].ActTypeValue);
            mailAction.ActionNotify(MailEntity);
            mailAction.ActionPrint(MailEntity);

            mockMailSender.Verify(x => x.SendTo(ConfigEntity, MailEntity, ConfigEntity.MailActions[3].ActTypeValue), Times.Once());
            mockMailCopy.Verify(x => x.CopyTo(ConfigEntity, MailEntity, ConfigEntity.MailActions[2].ActTypeValue), Times.Once());
            mockNotify.Verify(x => x.NotifyTo(MailEntity), Times.Once());
            mockPrint.Verify(x => x.PrintTo(MailEntity), Times.Once());
        }

        [TestMethod]
        public void MailAction_SmtpSender_Verify()
        {
            var mockMailSender = new Mock<IMailSender>();
            var mockMailCopy = new Mock<IMailCopy>();
            var mockNotify = new Mock<INotify>();
            var mockPrint = new Mock<IPrint>();

            mockMailSender.Setup(x => x.SendTo(It.IsAny<ConfigEntity>(), It.IsAny<MailEntity>(), It.IsAny<string>())).Returns(true);

            var mailAction = new Action.MailAction(mockMailSender.Object, mockMailCopy.Object, mockNotify.Object, mockPrint.Object);
            var res = mailAction.ActionSend(ConfigEntity, MailEntity, ConfigEntity.MailActions[3].ActTypeValue);

            Assert.That(res, Is.True);
        }

        [TestMethod]
        public void MailAction_CopyToFolder_Verify()
        {            
            var mockMailSender = new Mock<IMailSender>();
            var mockMailCopy = new Mock<IMailCopy>();
            var mockNotify = new Mock<INotify>();
            var mockPrint = new Mock<IPrint>();

            mockMailCopy.Setup(x => x.CopyTo(It.IsAny<ConfigEntity>(), It.IsAny<MailEntity>(), It.IsAny<string>())).Returns(true);

            var mailAction = new Action.MailAction(mockMailSender.Object, mockMailCopy.Object, mockNotify.Object, mockPrint.Object);
            var res = mailAction.ActionCopy(ConfigEntity, MailEntity, ConfigEntity.MailActions[2].ActTypeValue);            

            Assert.That(res, Is.True);
        }

        [TestMethod]
        public void MailAction_Notify_Verify()
        {
            var mockMailSender = new Mock<IMailSender>();
            var mockMailCopy = new Mock<IMailCopy>();
            var mockNotify = new Mock<INotify>();
            var mockPrint = new Mock<IPrint>();

            mockNotify.Setup(x => x.NotifyTo(It.IsAny<MailEntity>())).Returns(true);

            var mailAction = new Action.MailAction(mockMailSender.Object, mockMailCopy.Object, mockNotify.Object, mockPrint.Object);
            var res = mailAction.ActionNotify(MailEntity);            

            Assert.That(res, Is.True);
        }

        [TestMethod]
        public void MailAction_PrintDefault_Verify()
        {
            var mockMailSender = new Mock<IMailSender>();
            var mockMailCopy = new Mock<IMailCopy>();
            var mockNotify = new Mock<INotify>();
            var mockPrint = new Mock<IPrint>();

            mockPrint.Setup(x => x.PrintTo(It.IsAny<MailEntity>())).Returns(true);           

            var mailAction = new Action.MailAction(mockMailSender.Object, mockMailCopy.Object, mockNotify.Object, mockPrint.Object);
            var res = mailAction.ActionPrint(MailEntity);

            Assert.That(res, Is.True);
        }
    }
}
