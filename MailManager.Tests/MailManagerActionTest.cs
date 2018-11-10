using MailManager.Action;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailManager.Config;
using Assert = NUnit.Framework.Assert;
using MailManager.Monitor;
using System.Diagnostics;
using System.Net.Mail;
using NUnit.Framework;
using System.Drawing.Printing;
using System.Drawing;

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
            mailAction.ActionSendAsync(ConfigEntity, MailEntity, 3);
            mailAction.ActionCopy(ConfigEntity, MailEntity, 2);
            mailAction.ActionNotify(ConfigEntity, MailEntity, 0);
            mailAction.ActionPrint(ConfigEntity, MailEntity, 1);

            mockMailSender.Verify(x => x.SendAsync(ConfigEntity, MailEntity, 3), Times.Once());
            mockMailCopy.Verify(x => x.CopyTo(ConfigEntity, MailEntity, 2), Times.Once());
            mockNotify.Verify(x => x.NotifyTo(ConfigEntity, MailEntity, 0), Times.Once());
            mockPrint.Verify(x => x.PrintTo(ConfigEntity, MailEntity, 1), Times.Once());

        }

        [TestMethod]
        public async Task MailAction_SmtpSender_Verify()
        {
            var mock = new Mock<ISendWork>();
            mock.Setup(x => x.DoWork(It.IsAny<ConfigEntity>(), It.IsAny<MailEntity>(), It.IsAny<int>())).Returns(true);

            var smtpSender = new SmtpSender(mock.Object);
            var res = await smtpSender.SendAsync(ConfigEntity, MailEntity, 3);

            Assert.That(res, Is.True);
        }

        [TestMethod]
        public void MailAction_CopyToFolder_Verify()
        {
            var mock = new Mock<ICopyWork>();
            mock.Setup(x => x.DoWork(It.IsAny<ConfigEntity>(), It.IsAny<MailEntity>(), It.IsAny<int>())).Returns(true);

            var copyToFolder = new CopyToFolder(mock.Object);
            var res = copyToFolder.CopyTo(ConfigEntity, MailEntity, 2);

            Assert.That(res, Is.True);
        }

        [TestMethod]
        public void MailAction_Notify_Verify()
        {
            var consoleNotify = new ConsoleNotify();
            var res = consoleNotify.NotifyTo(ConfigEntity, MailEntity, 0);

            Assert.That(res, Is.True);
        }

        [TestMethod]
        public async Task MailAction_PrintDefault_Verify()
        {
            var mock = new Mock<IPrintWork>();
            mock.Setup(x => x.DoWork(It.IsAny<StringBuilder>())).Returns(true);           

            var print = new PrintDefault(mock.Object);
            var res = await print.PrintTo(ConfigEntity, MailEntity, 1);

            Assert.That(res, Is.True);
        }
    }
}
