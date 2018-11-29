using MailManager.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Assert = NUnit.Framework.Assert;

namespace MailManager.Tests
{
    [TestClass]
    public class MailManagerConfigTest
    {
        private ConfigEntity ConfigEntity { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Debug.WriteLine("Test Initialize");

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
        public void ReadStream_Verify()
        {
            var mockConfigStream = new Mock<IConfigStream>();

            mockConfigStream
                .Setup(x => x.ReadStream())
                .Returns(It.IsAny<List<ConfigEntity>>());

            var configReader = new ConfigReader(mockConfigStream.Object);
            configReader.ReadConfig();

            mockConfigStream.VerifyAll();
        }

        [TestMethod]
        public void ReadStream_From_XML_SettingsPathIsNull_ExceptionThrown()
        {
            var configStream = new XmlConfigStream(null);
            string message = "Не указан путь к файлу конфигурации";

            Assert.That(() => configStream.ReadStream(),
                Throws.TypeOf<ArgumentNullException>()
                    .With.Message.Contain(message));
        }        

        [TestMethod]
        public void ReadConfig_IsCorrect()
        {
            var mockConfigStream = new Mock<IConfigStream>();
            List<ConfigEntity> configEntityListExp = new List<ConfigEntity> { ConfigEntity };

            mockConfigStream
                .Setup(x => x.ReadStream())
                .Returns(configEntityListExp);

            var configReader = new ConfigReader(mockConfigStream.Object);
            IList<ConfigEntity> configEntityList = configReader.ReadConfig();
            
            Assert.AreEqual(configEntityListExp[0].Mail, configEntityList[0].Mail);
            Assert.AreEqual(configEntityListExp[0].Port, configEntityList[0].Port);
            Assert.AreEqual(configEntityListExp[0].Login, configEntityList[0].Login);
            Assert.AreEqual(configEntityListExp[0].Password, configEntityList[0].Password);
            Assert.AreEqual(configEntityListExp[0].MailActions[0].ActType, configEntityList[0].MailActions[0].ActType);
            Assert.AreEqual(configEntityListExp[0].IdentityMessages[0].IdType, configEntityList[0].IdentityMessages[0].IdType);
        }

        [TestMethod]
        public void VerifyConfig_True_Returned()
        {
            List<ConfigEntity> configEntityList = new List<ConfigEntity> { ConfigEntity };

            var configVerify = new ConfigVerify();
            var result = configVerify.VerifyConfig(configEntityList);

            Assert.True(result);
        }

        [TestMethod]
        public void VerifyConfig_Not_Correct_Mail()
        {
            ConfigEntity.Mail = "popp.yandex.ru";
            List<ConfigEntity> configEntityList = new List<ConfigEntity> { ConfigEntity };

            var configVerify = new ConfigVerify();
            configVerify.VerifyConfig(configEntityList);

            Assert.False(configVerify.IsValidConfig);
        }

        [TestMethod]
        public void VerifyConfig_Not_Correct_Login()
        {
            ConfigEntity.Login = null;
            List<ConfigEntity> configEntityList = new List<ConfigEntity> { ConfigEntity };

            var configVerify = new ConfigVerify();
            configVerify.VerifyConfig(configEntityList);

            Assert.False(configVerify.IsValidConfig);
        }

        [TestMethod]
        public void VerifyConfig_Not_Correct_Password()
        {
            ConfigEntity.Password = null;
            List<ConfigEntity> configEntityList = new List<ConfigEntity> { ConfigEntity };

            var configVerify = new ConfigVerify();
            configVerify.VerifyConfig(configEntityList);

            Assert.False(configVerify.IsValidConfig);
        }

        [TestMethod]
        public void VerifyConfig_Not_Correct_Port()
        {
            ConfigEntity.Port = 0;
            List<ConfigEntity> configEntityList = new List<ConfigEntity> { ConfigEntity };

            var configVerify = new ConfigVerify();
            configVerify.VerifyConfig(configEntityList);

            Assert.False(configVerify.IsValidConfig);
        }

        [TestMethod]
        public void VerifyConfig_Not_Correct_MailActions_CopyTo()
        {
            ConfigEntity.MailActions[2].ActTypeValue = null;
            List<ConfigEntity> configEntityList = new List<ConfigEntity> { ConfigEntity };

            var configVerify = new ConfigVerify();
            configVerify.VerifyConfig(configEntityList);

            Assert.False(configVerify.IsValidConfig);
        }

        [TestMethod]
        public void VerifyConfig_Not_Correct_MailActions_Forward()
        {
            ConfigEntity.MailActions[3].ActTypeValue = "juk_777@@mail.ru";
            List<ConfigEntity> configEntityList = new List<ConfigEntity> { ConfigEntity };

            var configVerify = new ConfigVerify();
            configVerify.VerifyConfig(configEntityList);

            Assert.False(configVerify.IsValidConfig);
        }

        [TestMethod]
        public void VerifyConfig_Not_Correct_IdentityMessages_To()
        {
            ConfigEntity.IdentityMessages[0].IdTypeValue = "gus.guskovskij@@mail.ru";
            List<ConfigEntity> configEntityList = new List<ConfigEntity> { ConfigEntity };

            var configVerify = new ConfigVerify();
            configVerify.VerifyConfig(configEntityList);

            Assert.False(configVerify.IsValidConfig);
        }

        [TestMethod]
        public void VerifyConfig_Not_Correct_IdentityMessages_From()
        {
            ConfigEntity.IdentityMessages[1].IdTypeValue = "juk_777@@mail.ru";
            List<ConfigEntity> configEntityList = new List<ConfigEntity> { ConfigEntity };

            var configVerify = new ConfigVerify();
            configVerify.VerifyConfig(configEntityList);

            Assert.False(configVerify.IsValidConfig);
        }

        [TestMethod]
        public void VerifyConfig_Not_Correct_IdentityMessages_Title()
        {
            ConfigEntity.IdentityMessages[2].IdTypeValue = null;
            List<ConfigEntity> configEntityList = new List<ConfigEntity> { ConfigEntity };

            var configVerify = new ConfigVerify();
            configVerify.VerifyConfig(configEntityList);

            Assert.False(configVerify.IsValidConfig);
        }

        [TestMethod]
        public void VerifyConfig_Not_Correct_IdentityMessages_Body()
        {
            ConfigEntity.IdentityMessages[3].IdTypeValue = null;
            List<ConfigEntity> configEntityList = new List<ConfigEntity> { ConfigEntity };

            var configVerify = new ConfigVerify();
            configVerify.VerifyConfig(configEntityList);

            Assert.False(configVerify.IsValidConfig);
        }
    }
}
