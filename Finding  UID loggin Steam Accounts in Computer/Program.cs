using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Finding_UID_loggin_Steam_Accounts_in_Computer
{
    class Program
    {
        struct LoginUsers
        {
            public string UID;
            public string AccountName;
            public string PersonaName;
            public string RememberPassword;
            public int MostRecent;
            public int Timestamp;
            public void DisplayInfo()
            {
                Console.WriteLine($"UID: '{UID}'\nAccountName: '{AccountName}'\nPersonaName: '{PersonaName}'\n" +
                    $"RememberPassword: '{RememberPassword}'\nMostRecent: '{MostRecent}'\nTimestamp: '{Timestamp}'\n");
            }

        };
        static void Main(string[] args)
        {
            List<LoginUsers> ResultList = new List<LoginUsers>();   // в это прилетит результат GetSteamUIDs // Created variable to store all account
            ResultList = GetSteamUIDs(GetSteamFolder());            // вызов получения ОБЩЕЙ информации о Steam UID // Get all Steam UID and e.t.s.
            Console.WriteLine("GetEnabledAccountUID: " + GetEnabledAccount(ResultList) + "\n\n\n");  // получение активного Steam UID  ! // Get active Steam UID 
            foreach (LoginUsers steam in ResultList)
            {
                steam.DisplayInfo();                                // вывод в консоль, для теста // Write in console for TEST.
            }
            Console.ReadLine();
        }
        static string GetEnabledAccount(List<LoginUsers> loginuser)    // метод для получения залогининого аккаунта // Method for getting logged in account      
        {
            foreach (LoginUsers EnabledAccount in loginuser)
            {
                if (EnabledAccount.MostRecent == 1)
                    return EnabledAccount.UID;
            }
            return "No active accounts";
        }
        static string GetSteamFolder()
        {
            try
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    return Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", "Nothing").ToString();   // для х64  // for x64
                }
                else
                {
                    return Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", "Nothing").ToString();       // для х32 // for x32
                }
            }
            catch
            {
                return null;
            }
        }
        static List<LoginUsers> GetSteamUIDs(string file)
        {
            StreamReader UIDvds = null;
            List<LoginUsers> UIDsList = new List<LoginUsers>(); // это вернет метод при вызове // return variable
            LoginUsers loginUsers = new LoginUsers();
            string line;
            try
            {
                UIDvds = new StreamReader(file + @"\config\loginusers.vdf");

                while ((line = UIDvds.ReadLine()) != null)
                {
                    Match UID = Regex.Match(line, @"[\d]{17}");     // простой поиск по регулярным выражениям  // Simple search on regular expressions
                    #region  Заполнение структуры. Все что в регионе обеспечивает заполнение структуры  LoginUsers с последующей загрузкой в List<LoginUsers> UIDsList
                    if (UID.Success)
                    {
                        if (loginUsers.UID != null)
                            UIDsList.Add(loginUsers);
                        loginUsers.UID = UID.Value;
                        continue;
                    }
                    if (line.Contains("AccountName"))
                    {
                        loginUsers.AccountName = line.Substring(18);
                        loginUsers.AccountName = loginUsers.AccountName.Remove(loginUsers.AccountName.Length - 1);
                        continue;
                    }
                    if (line.Contains("PersonaName"))
                    {
                        loginUsers.PersonaName = line.Substring(18);
                        loginUsers.PersonaName = loginUsers.PersonaName.Remove(loginUsers.PersonaName.Length - 1);
                        continue;
                    }
                    if (line.Contains("RememberPassword"))
                    {
                        loginUsers.RememberPassword = line.Substring(23);
                        loginUsers.RememberPassword = loginUsers.RememberPassword.Remove(loginUsers.RememberPassword.Length - 1);
                        continue;
                    }
                    if (line.Contains("MostRecent") || line.Contains("mostrecent"))
                    {
                        line = line.Substring(17);
                        loginUsers.MostRecent = Convert.ToInt32(line.Remove(line.Length - 1));
                        continue;
                    }
                    if (line.Contains("Timestamp"))
                    {
                        line = line.Substring(16);
                        loginUsers.Timestamp = Convert.ToInt32(line.Remove(line.Length - 1));

                        continue;
                    }
                    #endregion
                }
                if (UIDsList != null)
                    UIDsList.Add(loginUsers);
                UIDvds.Close();
                return UIDsList;
            }
            catch
            {
                return UIDsList;
            }
        }

    }
}
