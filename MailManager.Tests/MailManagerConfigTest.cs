using MailManager.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Assert = NUnit.Framework.Assert;

namespace MailManager.Tests
{
    [TestClass]
    public class MailManagerConfigTest
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
        public void ReadConfig_Verify()
        {
            var mockConfigStream = new Mock<IConfigStream>();

            mockConfigStream
                .Setup(x => x.ReadStream())
                .Returns(It.IsAny<List<ConfigEntity>>());

            var configReader = new ConfigReader(mockConfigStream.Object);

            configReader.ReadConfig();

            mockConfigStream.Verify();
        }

        [TestMethod]
        public void ReadStream_From_XML_SettingsPathIsNull_ExceptionThrown()
        {
            var configStream = new XmlConfigStream(null);
            Assert.Throws<ApplicationException>(() => configStream.ReadStream());
        }

        [TestMethod]
        public void ReadStream_From_XML_ReturnValueIsNotNull()
        {
            var settingsPath = @"Files\MailManagerSettings.xml";
            var configStream = new XmlConfigStream(settingsPath);

            IList<ConfigEntity> confEntityList = configStream.ReadStream();

            Assert.IsNotEmpty(confEntityList);
        }

        [TestMethod]
        public void ReadStream_From_XML_IsCorrect()
        {
            var settingsPath = @"Files\MailManagerSettings.xml";
            var configStream = new XmlConfigStream(settingsPath);

            ConfigEntity configEntity1 = new ConfigEntity();
            configEntity1.MailActions = new[] { new MailAction { ActType = ActionType.Notify, ActTypeValue = "yes" }, new MailAction { ActType = ActionType.Forward, ActTypeValue = "" } };
            configEntity1.IdentityMessages = new[] { new IdentityMessage { IdType = IdentityType.To, IdTypeValue = "gus.guskovskij@mail.ru" }, new IdentityMessage { IdType = IdentityType.Title, IdTypeValue = "тест" } };
            configEntity1.Mail = "pop.mail.ru";
            configEntity1.Port = 995;
            configEntity1.Login = "gus.guskovskij";
            configEntity1.Password = "11guskovskij11";

            IList<ConfigEntity> confEntityList = configStream.ReadStream();
            
            Assert.AreEqual(confEntityList[0].Mail, configEntity1.Mail);
            Assert.AreEqual(confEntityList[0].Port, configEntity1.Port);
            Assert.AreEqual(confEntityList[0].Login, configEntity1.Login);
            Assert.AreEqual(confEntityList[0].Password, configEntity1.Password);
            Assert.AreEqual(confEntityList[0].MailActions[0].ActType, configEntity1.MailActions[0].ActType);
            Assert.AreEqual(confEntityList[0].IdentityMessages[0].IdType, configEntity1.IdentityMessages[0].IdType);
        }

        [TestMethod]
        public void ReadStream_From_AppConfig_ReturnValueIsNotNull()
        {
            var settingsPath = @"App.config";
            var configStream = new AppConfigStream(settingsPath);

            IList<ConfigEntity> confEntityList = configStream.ReadStream();

            Assert.IsNotEmpty(confEntityList);
        }
    }
}
