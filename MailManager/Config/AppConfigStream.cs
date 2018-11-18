using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailManager.Config
{
    public class AppConfigStream : IConfigStream
    {
        public string SettingsPath { get; set; }

        public AppConfigStream(string setPath)
        {
            SettingsPath = setPath;
        }
        public List<ConfigEntity> ReadStream()
        {
            List<ConfigEntity> configEntityList = new List<ConfigEntity>();

            // Read all the keys from the config file
            NameValueCollection sAll;
            sAll = ConfigurationManager.AppSettings;

            ConfigEntity configEntity = new ConfigEntity();
            foreach (string s in sAll.AllKeys)
            {
                //Console.WriteLine("Key: " + s + " Value: " + sAll.Get(s));
                if (s=="Mail")
                    configEntity.Mail = sAll.Get(s);
                if (s == "Port")
                    configEntity.Port = Convert.ToInt32(sAll.Get(s));
                if (s == "Login")
                    configEntity.Login = sAll.Get(s);
                if (s == "Password")
                    configEntity.Password = sAll.Get(s);

                #region Set_MailAction

                if (s == "MailAction")
                {
                    string[] tempArray = sAll.Get(s).Split(';');
                    configEntity.MailActions = new MailAction[tempArray.Length];
                    var i = 0;
                    foreach (string arr in tempArray)
                    {
                        string[] arrVal = arr.Split('=');
                        for (int j = 0; j < arrVal.Length; j++)
                        {
                            arrVal[j] = arrVal[j].Trim();
                        }
                        if (arrVal[0] == "Notify")
                        {
                            configEntity.MailActions[i] = new MailAction
                            {
                                ActType = ActionType.Notify,
                                ActTypeValue = null
                            };
                            i++;
                        }
                        if (arrVal[0] == "CopyTo")
                        {
                            configEntity.MailActions[i] = new MailAction
                            {
                                ActType = ActionType.CopyTo,
                                ActTypeValue = arrVal[1]
                            };
                            i++;
                        }
                        if (arrVal[0] == "Forward")
                        {
                            configEntity.MailActions[i] = new MailAction
                            {
                                ActType = ActionType.Forward,
                                ActTypeValue = arrVal[1]
                            };
                            i++;
                        }
                        if (arrVal[0] == "Print")
                        {
                            configEntity.MailActions[i] = new MailAction
                            {
                                ActType = ActionType.Print,
                                ActTypeValue = null
                            };
                            i++;
                        }
                    }
                }

                #endregion

                #region Set_IdentityMessage

                if (s == "IdentityMessage")
                {
                    string[] tempArray = sAll.Get(s).Split(';');
                    configEntity.IdentityMessages = new IdentityMessage[tempArray.Length];
                    var i = 0;
                    foreach (string arr in tempArray)
                    {
                        string[] arrVal = arr.Split('=');
                        for (int j = 0; j < arrVal.Length; j++)
                        {
                            arrVal[j] = arrVal[j].Trim();
                        }
                        if (arrVal[0] == "To")
                        {
                            configEntity.IdentityMessages[i] = new IdentityMessage
                            {
                                IdType = IdentityType.To,
                                IdTypeValue = arrVal[1]
                            };
                            i++;
                        }
                        if (arrVal[0] == "From")
                        {
                            configEntity.IdentityMessages[i] = new IdentityMessage
                            {
                                IdType = IdentityType.From,
                                IdTypeValue = arrVal[1]
                            };
                            i++;
                        }
                        if (arrVal[0] == "Title")
                        {
                            configEntity.IdentityMessages[i] = new IdentityMessage
                            {
                                IdType = IdentityType.Title,
                                IdTypeValue = arrVal[1]
                            };
                            i++;
                        }
                        if (arrVal[0] == "Body")
                        {
                            configEntity.IdentityMessages[i] = new IdentityMessage
                            {
                                IdType = IdentityType.Body,
                                IdTypeValue = arrVal[1]
                            };
                            i++;
                        }
                    }
                }

                #endregion


            }

            configEntityList.Add(configEntity);
            Console.WriteLine("Конфигурация считана!");

            return configEntityList;
        }
    }
}
