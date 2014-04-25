using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace HomeProjectWebForms.Helpers
{
    public static class GUIHelper
    {
        /// <summary>
        ///     Получение логина пользователя из доменного имени
        /// </summary>
        /// <param name="fullName">Полное имя пользователя, включая домен</param>
        /// <returns></returns>
        //public static string SplitUserName(string fullName)
        //{
        //    //Пользователь авторизуется по домену
        //    //В домене имя и домен разделены символом \
        //    //В случае когда не удалось распознать имя будет возвращенно полное имя
        //    string[] splitedName = @fullName.Split('\\');
        //    if (splitedName.Any())
        //        return splitedName[1];
        //    return fullName;
        //}
        public static string GetDomain(this IIdentity identity)
        {
            string s = identity.Name;
            int stop = s.IndexOf("\\");
            return (stop > -1) ? s.Substring(0, stop) : string.Empty;
        }

        public static string GetLogin(this IIdentity identity)
        {
            string s = identity.Name;
            int stop = s.IndexOf("\\");
            return (stop > -1) ? s.Substring(stop + 1, s.Length - stop - 1) : string.Empty;
        }

        public static string ParsePhone(string getString, string phoneType, string phoneCode)
        {
            string userPhoneNumber=string.Empty;
            string parsedPhoneNumber = ParsePhone(getString, phoneType);
            if (phoneType.Trim().ToUpper() == "ГАТС")
                userPhoneNumber += "("+phoneCode+")" + parsedPhoneNumber;
            else
                userPhoneNumber = parsedPhoneNumber;
            return userPhoneNumber;
        }

        public static string ParsePhone(string getString,string phoneType)
        {
            try
            {
                char[] phoneArra = getString.ToCharArray();
                var parsedPhone = new List<string>();
                var phoneNumberPair = new List<char>();
                int phoneSimbolsPair = 0;
                bool isTripleNumbersAdded = false;
                for (int i = phoneArra.Count() - 1; i >= 0; i--)
                {
                    if (char.IsNumber(phoneArra[i]))
                        phoneNumberPair.Insert(0, phoneArra[i]);
                    else continue;

                    if (phoneNumberPair.Count() == 2 && i >= 2 && phoneSimbolsPair < 2)
                    {
                        string phoneItem = "-";
                        phoneItem = phoneNumberPair.Aggregate(phoneItem, (current, simbol) => current + simbol);

                        parsedPhone.Insert(0, phoneItem);
                        phoneNumberPair.Clear();
                        phoneSimbolsPair++;
                    }
                    else if (phoneSimbolsPair == 2 &&
                             phoneNumberPair.Count() == 3 && !isTripleNumbersAdded)
                    {
                        string phone = string.Empty;
                        phone = phoneNumberPair.Aggregate(phone, (current, simbol) => current + simbol);
                        parsedPhone.Insert(0, phone);
                        phoneNumberPair.Clear();
                        isTripleNumbersAdded = true;
                    }
                    else if (i == 0 && phoneType.Trim().ToUpper() != "МИНИАТС" &&
                             phoneType.Trim().ToUpper() != "ГАТС")
                    {
                        string phoneRest = string.Empty;
                        if (phoneNumberPair.Count > 3)
                        {
                            phoneRest += "8(";
                            for (int j = phoneNumberPair.Count-3; j <= phoneNumberPair.Count - 1; j++)
                                phoneRest += phoneNumberPair[j];
                            phoneRest += ")";
                        }
                        else
                            phoneRest = "8(" +
                                        phoneNumberPair.Aggregate(phoneRest, (current, simbol) => current + simbol) +
                                        ")";
                        parsedPhone.Insert(0, phoneRest);
                        phoneNumberPair.Clear();
                    }
                    else if (i == 0)
                    {
                        string phoneRest = string.Empty;
                        phoneRest = phoneNumberPair.Aggregate(phoneRest, (current, simbol) => current + simbol);
                        parsedPhone.Insert(0, phoneRest);
                        phoneNumberPair.Clear();
                    }
                }
                string result = string.Empty;
                return parsedPhone.Aggregate(result, (current, simbol) => current + simbol);
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка парсинга телефона:" + getString, ex);
            }
            return getString;
        }
    }
}