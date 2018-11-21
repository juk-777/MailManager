﻿using MailManager.Action;
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
        public MailEntity MailEntity { get; set; }
        public ConfigEntity ConfigEntity { get; set; }

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
                MailActions =
                    new Config.MailAction[]
                    {
                        new Config.MailAction {ActType = ActionType.Notify, ActTypeValue = "yes"},
                        new Config.MailAction {ActType = ActionType.Print, ActTypeValue = "yes"},
                        new Config.MailAction {ActType = ActionType.CopyTo, ActTypeValue = "folder"},
                        new Config.MailAction {ActType = ActionType.Forward, ActTypeValue = "juk_777@mail.ru"},
                    },
                IdentityMessages = new IdentityMessage[]
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
            mailAction.ActionSend(ConfigEntity, MailEntity, 3);
            mailAction.ActionCopy(ConfigEntity, MailEntity, 2);
            mailAction.ActionNotify(ConfigEntity, MailEntity, 0);
            mailAction.ActionPrint(ConfigEntity, MailEntity, 1);

            mockMailSender.Verify(x => x.SendTo(ConfigEntity, MailEntity, 3), Times.Once());
            mockMailCopy.Verify(x => x.CopyTo(ConfigEntity, MailEntity, 2), Times.Once());
            mockNotify.Verify(x => x.NotifyTo(ConfigEntity, MailEntity, 0), Times.Once());
            mockPrint.Verify(x => x.PrintTo(ConfigEntity, MailEntity, 1), Times.Once());
        }

        [TestMethod]
        public void MailAction_SmtpSender_Verify()
        {
            var mockMailSender = new Mock<IMailSender>();
            var mockMailCopy = new Mock<IMailCopy>();
            var mockNotify = new Mock<INotify>();
            var mockPrint = new Mock<IPrint>();

            mockMailSender.Setup(x => x.SendTo(It.IsAny<ConfigEntity>(), It.IsAny<MailEntity>(), It.IsAny<int>())).Returns(true);

            var mailAction = new Action.MailAction(mockMailSender.Object, mockMailCopy.Object, mockNotify.Object, mockPrint.Object);
            var res = mailAction.ActionSend(ConfigEntity, MailEntity, 1);

            Assert.That(res, Is.True);
        }

        [TestMethod]
        public void MailAction_CopyToFolder_Verify()
        {            
            var mockMailSender = new Mock<IMailSender>();
            var mockMailCopy = new Mock<IMailCopy>();
            var mockNotify = new Mock<INotify>();
            var mockPrint = new Mock<IPrint>();

            mockMailCopy.Setup(x => x.CopyTo(It.IsAny<ConfigEntity>(), It.IsAny<MailEntity>(), It.IsAny<int>())).Returns(true);

            var mailAction = new Action.MailAction(mockMailSender.Object, mockMailCopy.Object, mockNotify.Object, mockPrint.Object);
            var res = mailAction.ActionCopy(ConfigEntity, MailEntity, 1);            

            Assert.That(res, Is.True);
        }

        [TestMethod]
        public void MailAction_Notify_Verify()
        {
            var mockMailSender = new Mock<IMailSender>();
            var mockMailCopy = new Mock<IMailCopy>();
            var mockNotify = new Mock<INotify>();
            var mockPrint = new Mock<IPrint>();

            mockNotify.Setup(x => x.NotifyTo(It.IsAny<ConfigEntity>(), It.IsAny<MailEntity>(), It.IsAny<int>())).Returns(true);

            var mailAction = new Action.MailAction(mockMailSender.Object, mockMailCopy.Object, mockNotify.Object, mockPrint.Object);
            var res = mailAction.ActionNotify(ConfigEntity, MailEntity, 1);            

            Assert.That(res, Is.True);
        }

        [TestMethod]
        public void MailAction_PrintDefault_Verify()
        {
            var mockMailSender = new Mock<IMailSender>();
            var mockMailCopy = new Mock<IMailCopy>();
            var mockNotify = new Mock<INotify>();
            var mockPrint = new Mock<IPrint>();

            mockPrint.Setup(x => x.PrintTo(It.IsAny<ConfigEntity>(), It.IsAny<MailEntity>(), It.IsAny<int>())).Returns(true);           

            var mailAction = new Action.MailAction(mockMailSender.Object, mockMailCopy.Object, mockNotify.Object, mockPrint.Object);
            var res = mailAction.ActionPrint(ConfigEntity, MailEntity, 1);

            Assert.That(res, Is.True);
        }
    }
}
