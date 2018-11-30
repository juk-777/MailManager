using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace MailManager.Config
{
    public class AppConfigStream : IConfigStream
    {        
        public List<ConfigEntity> ReadStream()
        {
            List<ConfigEntity> configEntityList = new List<ConfigEntity>();
            NameValueCollection sAll = ConfigurationManager.AppSettings;

            ConfigEntity configEntity = new ConfigEntity();
            foreach (string s in sAll.AllKeys)
            {
                if (s != "Mail" && s != "Port" && s != "Login" && s != "Password" && s != "MailAction" && s != "IdentityMessage")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nОшибочные данные! {s}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    throw new ArgumentException();
                }

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

                        if (arrVal[0] != ActionType.Notify.ToString() && arrVal[0] != ActionType.CopyTo.ToString() && arrVal[0] != ActionType.Forward.ToString() && arrVal[0] != ActionType.Print.ToString())
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"\nОшибочные данные! {arrVal[0]} - {arrVal[1]}");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            throw new ArgumentException();
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

                        if (arrVal[0] != IdentityType.To.ToString() && arrVal[0] != IdentityType.From.ToString() && arrVal[0] != IdentityType.Title.ToString() && arrVal[0] != IdentityType.Body.ToString())
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"\nОшибочные данные! {arrVal[0]} - {arrVal[1]}");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            throw new ArgumentException();
                        }

                        if (arrVal[0] == IdentityType.To.ToString())
                        {
                            configEntity.IdentityMessages[i] = new IdentityMessage
                            {
                                IdType = IdentityType.To,
                                IdTypeValue = arrVal[1]
                            };
                            i++;
                        }
                        if (arrVal[0] == IdentityType.From.ToString())
                        {
                            configEntity.IdentityMessages[i] = new IdentityMessage
                            {
                                IdType = IdentityType.From,
                                IdTypeValue = arrVal[1]
                            };
                            i++;
                        }
                        if (arrVal[0] == IdentityType.Title.ToString())
                        {
                            configEntity.IdentityMessages[i] = new IdentityMessage
                            {
                                IdType = IdentityType.Title,
                                IdTypeValue = arrVal[1]
                            };
                            i++;
                        }
                        if (arrVal[0] == IdentityType.Body.ToString())
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
