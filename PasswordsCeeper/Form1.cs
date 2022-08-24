using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.IO;

namespace PasswordsCeeper
{
    public partial class Form1 : Form
    {
        private LoginParameters param = new LoginParameters();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists("logInParameters.epam"))
            {
                labelRegistration.Visible = true;
                labelPassword.Visible = false;
                buttonApply.Visible = true;
                buttonLogin.Visible = false;
            }
            else
            {
                ReadParameters();
            }
        }

        private string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hash);
        }

        private string PassPrep()
        {
            string pass = textBoxPassword.Text;

            byte[] passwordBasedKey;

            using (SHA256 mySHA256 = SHA256.Create())
            {
                passwordBasedKey = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(pass));
            }

            return Convert.ToBase64String(passwordBasedKey);
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            WriteParameters();

            Hide();

            CryptoManager.SetPass(PassPrep());
            textBoxPassword.Clear();
            MainForm mainForm = new MainForm();
            mainForm.Show();
        }

        private void WriteParameters()
        {
            param.LoginPassword = GetHash(textBoxPassword.Text);

            XmlSerializer formatter = new XmlSerializer(typeof(LoginParameters));

            using (FileStream fs = new FileStream("logInParameters.epam", FileMode.Create))
            {
                formatter.Serialize(fs, param);
            }
        }

        private void ReadParameters()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(LoginParameters));

            using (FileStream fs = new FileStream("logInParameters.epam", FileMode.Open))
            {
                param = (LoginParameters)formatter.Deserialize(fs);
            }
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (GetHash(textBoxPassword.Text) == param.LoginPassword)
            {
                labelWrongPassword.Visible = false;

                Hide();

                CryptoManager.SetPass(PassPrep());
                textBoxPassword.Clear();
                MainForm mainForm = new MainForm();
                mainForm.Show();
            }
            else
            {
                labelWrongPassword.Visible = true;
            }
        }
    }
}
