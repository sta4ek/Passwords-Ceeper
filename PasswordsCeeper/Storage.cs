using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Windows.Forms;

namespace PasswordsCeeper
{
    static class Storage
    {
        private static List<Account> accounts = new List<Account>();

        public static void LoadData()
        {
            if (!File.Exists("accounts.epam"))
            {
                SaveData();
            }
            
            CryptoManager.Decrypt();

            XmlSerializer formatter = new XmlSerializer(typeof(List<Account>));
            using (FileStream fileStream = new FileStream("accounts.epam", FileMode.Open))
                accounts = (List<Account>)formatter.Deserialize(fileStream);

            CryptoManager.Encrypt();
        }

        public static void SaveData()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<Account>));
            using (FileStream fileStream = new FileStream("accounts.epam", FileMode.Create))
                formatter.Serialize(fileStream, accounts);

            CryptoManager.Encrypt();
        }

        public static void AccountEdit(Account oldAccount, string newSiteURL, string newUsername, string newPassword)
        {
            foreach (var account in accounts)
                if (account.SiteURL == oldAccount.SiteURL && account.Username == oldAccount.Username)
                {
                    account.SiteURL = newSiteURL;
                    account.Username = newUsername;
                    account.Password = newPassword;
                }

            SaveData();
        }

        public static bool AccountAdd(string siteURL, string username, string password)
        {
            foreach (var account in accounts)
                if (account.SiteURL == siteURL && account.Username == username)
                {
                    MessageBox.Show("Аккаунт с таким пользователем на этом сайте уже есть");
                    return false;
                }

            Account acc = new Account(siteURL, username, password);
            accounts.Add(acc);

            SaveData();

            return true;
        }

        public static List<Account> Accounts()
        {
            return accounts;
        }

        public static string GeneratePassword()
        {
            string password = "";

            char[] alphabet = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            Random random = new Random();
            int upper = random.Next(18);
            int number = random.Next(18);

            while (upper == number)
            {
                number = random.Next(18);
            }

            int counter = 0;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    char toAdd = alphabet[random.Next(25)];

                    if (counter == upper)
                    {
                        toAdd = Char.ToUpper(toAdd);
                    }

                    if (counter == number)
                    {
                        toAdd = char.Parse(random.Next(10).ToString());
                    }

                    password += toAdd;
                    counter++;
                }

                if (i != 2)
                    password += '-';
            }

            return password;
        }
    }
}
