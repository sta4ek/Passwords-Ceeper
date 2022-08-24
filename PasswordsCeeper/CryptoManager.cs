using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Windows.Forms;

namespace PasswordsCeeper
{
    static class CryptoManager
    {
        private static string _pass;

        public static void SetPass(string pass)
        {
            _pass = pass;
        }

        public static void Encrypt()
        {
            try
            {
                string textToEncrypt = File.ReadAllText("accounts.epam");

                using (FileStream fileStream = new FileStream("accounts.epam", FileMode.OpenOrCreate))
                {
                    using (Aes aes = Aes.Create())
                    {
                        byte[] passwordBasedKey;

                        using (SHA256 mySHA256 = SHA256.Create())
                        {
                            passwordBasedKey = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(_pass));
                        }

                        aes.Key = passwordBasedKey;

                        byte[] iv = aes.IV;
                        fileStream.Write(iv, 0, iv.Length);

                        using (CryptoStream cryptoStream = new CryptoStream(
                            fileStream,
                            aes.CreateEncryptor(),
                            CryptoStreamMode.Write))
                        {
                            using (StreamWriter encryptWriter = new StreamWriter(cryptoStream))
                            {
                                encryptWriter.WriteLine(textToEncrypt);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The encryption failed. {ex}");
            }
        }

        public static void Decrypt()
        {
            try
            {
                string decryptedMessage;

                using (FileStream fileStream = new FileStream("accounts.epam", FileMode.Open))
                {
                    using (Aes aes = Aes.Create())
                    {
                        byte[] iv = new byte[aes.IV.Length];
                        int numBytesToRead = aes.IV.Length;
                        int numBytesRead = 0;
                        while (numBytesToRead > 0)
                        {
                            int n = fileStream.Read(iv, numBytesRead, numBytesToRead);
                            if (n == 0) break;

                            numBytesRead += n;
                            numBytesToRead -= n;
                        }

                        byte[] passwordBasedKey;

                        using (SHA256 mySHA256 = SHA256.Create())
                        {
                            passwordBasedKey = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(_pass));
                        }

                        using (CryptoStream cryptoStream = new CryptoStream(
                           fileStream,
                           aes.CreateDecryptor(passwordBasedKey, iv),
                           CryptoStreamMode.Read))
                        {
                            using (StreamReader decryptReader = new StreamReader(cryptoStream))
                            {
                                decryptedMessage = decryptReader.ReadToEnd();
                            }
                        }
                    }
                }

                File.WriteAllText("accounts.epam", decryptedMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The decryption failed. {ex}");
            }
        }
    }
}
