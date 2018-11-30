﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MailManager.Config
{
    public class ConfigVerifier : IConfigVerifier
    {
        public bool Verify(List<ConfigEntity> configEntityList)
        {
            foreach (ConfigEntity configEntity in configEntityList)
            {
                string mailPattern = @"^(pop\.|imap\.)\w*.\w*";
                if (!Regex.IsMatch(configEntity.Mail, mailPattern, RegexOptions.IgnoreCase))
                {
                    Console.WriteLine("Некорректное значение для поля Mail");
                    return false;
                }

                if (string.IsNullOrEmpty(configEntity.Login))
                {
                    Console.WriteLine("Логин не указан!");
                    return false;
                }

                if (string.IsNullOrEmpty(configEntity.Password))
                {
                    Console.WriteLine("Пароль не указан!");
                    return false;
                }

                if (configEntity.Port <= 0)
                {
                    Console.WriteLine("Порт задан неверно!");
                    return false;
                }

                string emailPattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";

                foreach (var action in configEntity.MailActions)
                {
                    if (action.ActType == ActionType.CopyTo)
                        if (string.IsNullOrEmpty(action.ActTypeValue))
                        {
                            Console.WriteLine("Не указана папка для сохранения копии письма!");
                            return false;
                        }
                    if (action.ActType == ActionType.Forward)
                        if (!Regex.IsMatch(action.ActTypeValue, emailPattern, RegexOptions.IgnoreCase))
                        {
                            Console.WriteLine("Некорректный email");
                            return false;
                        }
                }

                foreach (var identity in configEntity.IdentityMessages)
                {
                    if (identity?.IdType == IdentityType.To)
                        if (!Regex.IsMatch(identity.IdTypeValue, emailPattern, RegexOptions.IgnoreCase))
                        {
                            Console.WriteLine("Некорректный email");
                            return false;
                        }
                    if (identity?.IdType == IdentityType.From)
                        if (!Regex.IsMatch(identity.IdTypeValue, emailPattern, RegexOptions.IgnoreCase))
                        {
                            Console.WriteLine("Некорректный email");
                            return false;
                        }
                    if (identity?.IdType == IdentityType.Title)
                        if (string.IsNullOrEmpty(identity.IdTypeValue))
                        {
                            Console.WriteLine("Заголовок не указан!");
                            return false;
                        }
                    if (identity?.IdType == IdentityType.Body)
                        if (string.IsNullOrEmpty(identity.IdTypeValue))
                        {
                            Console.WriteLine("Содержание не указано!");
                            return false;
                        }
                }
            }

            Console.WriteLine("Проверка завершена успешно!");
            return true;
        }
    }
}
