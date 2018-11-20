using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using MailManager.Config;
using System.Threading;
using System.Diagnostics;
using System.Net.Mail;
using MailManager.Monitor;
using MailManager.Action;

namespace MailManager.Tests
{
    [TestClass]
    public class MailManagerMonitorTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Debug.WriteLine("Test Initialize");
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

            var mm = new MailMonitor(mockMailProvider.Object, mockMailAction.Object);
            mm.StartMonitorTask(new ConfigEntity());

            List<MailEntity> allMessages = new List<MailEntity>();
            List<string> allUids = new List<string>();

            mockMailProvider.Verify(x => x.GetAllMessages(It.IsAny<ConfigEntity>(), out allMessages, out allUids), Times.AtLeastOnce);
            //mockMailProvider.Verify(x => x.GetUnseenMessages(It.IsAny<ConfigEntity>(), allUids, out allUids), Times.AtLeastOnce);            
            //mockMailProvider.Verify(x => x.Dispose(), Times.AtLeastOnce);

            mockMailAction.VerifyAll();
        }

        [TestMethod]
        public void MailMonitor_StoptMonitor_Verify()
        {
            var mockMailProvider = new Mock<IMailProvider>();
            var mockMailAction = new Mock<IMailAction>();
            var mockTimer = new Mock<IDisposable>();

            mockTimer
                .Setup(x => x.Dispose());
            //void Func() => mm.StopMonitor();

            var mm = new MailMonitor(mockMailProvider.Object, mockMailAction.Object);
            var t = mockTimer.Object;

            // устанавливаем метод обратного вызова
            TimerCallback tm = new TimerCallback(mm.OtherAccessToMail);
            // создаем таймер
            Timer timer = new Timer(tm, new ConfigEntity(), 0, 1000);
            mm._timers.Add(timer);

            mm.StopMonitor();

            mockTimer.Verify();

        }

        [TestMethod]
        public void MailMonitor_StartMonitor_Verify()
        {
            var mockMailProvider = new Mock<IMailProvider>();
            var mockMailAction = new Mock<IMailAction>();

            ConfigEntity configEntity = new ConfigEntity();
            configEntity.MailActions = new Config.MailAction[]
            {
                new Config.MailAction { ActType = ActionType.Notify, ActTypeValue = "yes" },
                new Config.MailAction { ActType = ActionType.Print, ActTypeValue = "yes" },
                new Config.MailAction { ActType = ActionType.CopyTo, ActTypeValue = "folder" },
                new Config.MailAction { ActType = ActionType.Forward, ActTypeValue = "juk_777@mail.ru" },
            };
            configEntity.IdentityMessages = new IdentityMessage[]
            {
                new IdentityMessage { IdType = IdentityType.To, IdTypeValue = "gus.guskovskij@mail.ru" },
                new IdentityMessage { IdType = IdentityType.From, IdTypeValue = "juk_777@mail.ru" },
                new IdentityMessage { IdType = IdentityType.Title, IdTypeValue = "пис" },
                new IdentityMessage { IdType = IdentityType.Body, IdTypeValue = "вап" }
            };
            configEntity.Mail = "pop.yandex.ru";
            configEntity.Port = 995;
            configEntity.Login = "tiras.school.2";
            configEntity.Password = "pas";

            List<ConfigEntity> configEntities = new List<ConfigEntity>();
            configEntities.Add(configEntity);

            var mm = new MailMonitor(mockMailProvider.Object, mockMailAction.Object);

            mm.StartMonitor(configEntities);

            mockMailProvider.VerifyAll();
            mockMailAction.VerifyAll();

        }

        [TestMethod]
        public void MailMonitor_FirstAccessToMail_GetAllMessages_WasCalled()
        {
            var mockMailProvider = new Mock<IMailProvider>();
            var mockMailAction = new Mock<IMailAction>();

            List<MailEntity> allMessages = new List<MailEntity>();
            List<string> allUids = new List<string>();

            //mockMailProvider
            //    .Setup(x => x.GetAllMessages(out allMessages, out allUids));

            var mm = new MailMonitor(mockMailProvider.Object, mockMailAction.Object);
            mm.FirstAccessToMail(new ConfigEntity());

            mockMailProvider.Verify(x => x.GetAllMessages(It.IsAny<ConfigEntity>(), out allMessages, out allUids), Times.AtLeastOnce);

        }

        [TestMethod]
        public void MailMonitor_ProcessingMail_Verify()
        {
            var mockMailProvider = new Mock<IMailProvider>();
            var mockMailAction = new Mock<IMailAction>();

            var mailEntity = new MailEntity();
            mailEntity.To.Add(new MailAddress("gus.guskovskij@mail.ru"));
            mailEntity.From = new MailAddress("juk_777@mail.ru");
            mailEntity.Subject = "1 письмо";
            mailEntity.Body = new StringBuilder("вап ваппррр");
            mailEntity.DateSent = DateTime.Now;

            ConfigEntity configEntity = new ConfigEntity();
            configEntity.MailActions = new Config.MailAction[]
            {
                new Config.MailAction { ActType = ActionType.Notify, ActTypeValue = "yes" },
                new Config.MailAction { ActType = ActionType.Print, ActTypeValue = "yes" },
                new Config.MailAction { ActType = ActionType.CopyTo, ActTypeValue = "folder" },
                new Config.MailAction { ActType = ActionType.Forward, ActTypeValue = "juk_777@mail.ru" },
            };
            configEntity.IdentityMessages = new IdentityMessage[]
            {
                new IdentityMessage { IdType = IdentityType.To, IdTypeValue = "gus.guskovskij@mail.ru" },
                new IdentityMessage { IdType = IdentityType.From, IdTypeValue = "juk_777@mail.ru" },
                new IdentityMessage { IdType = IdentityType.Title, IdTypeValue = "пис" },
                new IdentityMessage { IdType = IdentityType.Body, IdTypeValue = "вап" }
            };
            configEntity.Mail = "pop.yandex.ru";
            configEntity.Port = 995;
            configEntity.Login = "tiras.school.2";
            configEntity.Password = "pas";

            List<MailEntity> allMessages = new List<MailEntity>();
            allMessages.Add(mailEntity);
            List<string> allUids = new List<string>();

            mockMailProvider
                .Setup(x => x.GetAllMessages(It.IsAny<ConfigEntity>(), out allMessages, out allUids));

            var mm = new MailMonitor(mockMailProvider.Object, mockMailAction.Object);
            mm.ProcessingMail(allMessages, configEntity);

            mockMailProvider.Verify();
            mockMailAction.VerifyAll();

        }
    }
}
