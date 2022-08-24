using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasswordsCeeper
{
    public partial class AccountAddForm : Form
    {
        private readonly ListBox _listBoxAccounts;

        public AccountAddForm(ListBox listBoxAccounts)
        {
            InitializeComponent();

            _listBoxAccounts = listBoxAccounts;

            toolTip.SetToolTip(pictureBoxGeneratePassword, "Генерировать пароль");
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (textBoxSiteURL.Text == String.Empty || textBoxUsername.Text == String.Empty || textBoxPassword.Text == String.Empty)
            {
                MessageBox.Show("Заполните все поля.");

                return;
            }

            if (Storage.AccountAdd(textBoxSiteURL.Text, textBoxUsername.Text, textBoxPassword.Text))
            {
                _listBoxAccounts.DataSource = null;
                _listBoxAccounts.DisplayMember = "siteURL";
                _listBoxAccounts.DataSource = Storage.Accounts();

                Close();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void pictureBoxGeneratePassword_Click(object sender, EventArgs e)
        {
            textBoxPassword.Text = Storage.GeneratePassword();
        }
    }
}
