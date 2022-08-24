using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace PasswordsCeeper
{
    public partial class MainForm : Form
    {
        public int selectedIndex = -1;
        bool isFirstLaunch = true;
        private string siteURLBeforeEdit;
        private string usernameBeforeEdit;
        private string passwordBeforeEdit;

        public MainForm()
        {
            InitializeComponent();
            
            Storage.LoadData();
        }
        
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            listBoxAccounts.DisplayMember = "siteURL";
            listBoxAccounts.DataSource = Storage.Accounts();
            listBoxAccounts.ClearSelected();

            toolTip.SetToolTip(pictureBoxCopy, "Копировать в буфер обмена");
            toolTip.SetToolTip(pictureBoxGeneratePassword, "Генерировать пароль");
        }

        private void listBoxAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            AccountInfoOutput();
        }

        private void AccountInfoOutput()
        {
            if (selectedIndex == listBoxAccounts.SelectedIndex)
                return;

            if (isFirstLaunch)
            {
                isFirstLaunch = false;
                return;
            }

            if (!panel1.Visible)
                panel1.Visible = true;

            selectedIndex = listBoxAccounts.SelectedIndex;

            if (listBoxAccounts.SelectedIndex != -1)
            {
                var acc = (Account)listBoxAccounts.SelectedItem;

                linkLabelSiteURL.Text = acc.SiteURL;
                textBoxUsername.Text = acc.Username;
                textBoxPassword.Text = acc.Password;

                pictureBoxCopy.Location = new Point(linkLabelSiteURL.Location.X + linkLabelSiteURL.Width + 5, 54);
                OutputPassRepeats(GetSitesPassRepeatsList(acc.SiteURL, acc.Password));
            }
        }

        private void OutputPassRepeats(List<string> sites)
        {
            if (sites.Count != 0)
            {
                label4.Visible = true;
                labelRepSites.Visible = true;
                pictureBoxWarning.Visible = true;
                labelRepSites.Text = "Данный пароль используется на таких веб-сайтах как: ";

                for (int i = 0; i < sites.Count; i++)
                {
                    labelRepSites.Text += sites[i];

                    if (i == sites.Count - 1)
                    {
                        labelRepSites.Text += ".";
                    }
                    else
                    {
                        labelRepSites.Text += ", ";
                    }
                }
            }
            else
            {
                label4.Visible = false;
                labelRepSites.Visible = false;
                pictureBoxWarning.Visible = false;
                labelRepSites.Text = "Данный пароль используется на таких веб-сайтах как: ";
            }
        }
        
        private List<string> GetSitesPassRepeatsList(string siteUrl, string password)
        {
            List<string> sites = new List<string>();

            foreach (var account in Storage.Accounts())
            {
                if (password == account.Password && siteUrl != account.SiteURL)
                {
                    sites.Add(account.SiteURL);
                }
            }

            return sites;
        }

        private void buttonAddAccount_Click(object sender, EventArgs e)
        {
            var addForm = new AccountAddForm(listBoxAccounts);
            addForm.ShowDialog();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            Storage.Accounts().Remove((Account)listBoxAccounts.SelectedItem);

            selectedIndex = -1;
            isFirstLaunch = true;
            panel1.Visible = false;

            listBoxAccounts.DataSource = null;
            listBoxAccounts.DataSource = Storage.Accounts();
            listBoxAccounts.DisplayMember = "siteURL";
            listBoxAccounts.ClearSelected();

            Storage.SaveData();
        }

        private void pictureBoxCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(linkLabelSiteURL.Text);
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            siteURLBeforeEdit = linkLabelSiteURL.Text;
            usernameBeforeEdit = textBoxUsername.Text;
            passwordBeforeEdit = textBoxPassword.Text;

            textBoxSiteURL.Text = linkLabelSiteURL.Text;
            linkLabelSiteURL.Visible = false;

            textBoxSiteURL.Visible = true;
            textBoxUsername.ReadOnly = false;
            textBoxPassword.ReadOnly = false;

            buttonEdit.Visible = false;
            buttonSave.Visible = true;
            buttonCancel.Visible = true;
            buttonDelete.Visible = false;
            buttonAddAccount.Enabled = false;

            pictureBoxClose.Visible = false;
            pictureBoxCopy.Visible = false;
            pictureBoxGeneratePassword.Visible = true;

            listBoxAccounts.Enabled = false;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Storage.AccountEdit((Account)listBoxAccounts.SelectedItem, textBoxSiteURL.Text, textBoxUsername.Text, textBoxPassword.Text);

            linkLabelSiteURL.Visible = true;
            linkLabelSiteURL.Text = textBoxSiteURL.Text;

            textBoxSiteURL.Visible = false;

            textBoxUsername.ReadOnly = true;
            textBoxPassword.ReadOnly = true;

            buttonEdit.Visible = true;
            buttonSave.Visible = false;
            buttonCancel.Visible = false;
            buttonDelete.Visible = true;
            buttonAddAccount.Enabled = true;

            pictureBoxClose.Visible = true;
            pictureBoxCopy.Visible = true;
            pictureBoxGeneratePassword.Visible = false;
            pictureBoxCopy.Location = new Point(linkLabelSiteURL.Location.X + linkLabelSiteURL.Width + 5, 54);

            listBoxAccounts.Enabled = true;

            listBoxAccounts.DataSource = null;
            listBoxAccounts.DisplayMember = "siteURL";
            listBoxAccounts.DataSource = Storage.Accounts();
            
            listBoxAccounts.SelectedIndex = listBoxAccounts.FindStringExact(textBoxSiteURL.Text);
            textBoxSiteURL.Clear();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            listBoxAccounts.ClearSelected();
            panel1.Visible = false;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            linkLabelSiteURL.Text = siteURLBeforeEdit;
            textBoxUsername.Text = usernameBeforeEdit;
            textBoxPassword.Text = passwordBeforeEdit;

            linkLabelSiteURL.Visible = true;

            textBoxSiteURL.Clear();
            textBoxSiteURL.Visible = false;

            textBoxUsername.ReadOnly = true;
            textBoxPassword.ReadOnly = true;

            buttonEdit.Visible = true;
            buttonSave.Visible = false;
            buttonCancel.Visible = false;
            buttonDelete.Visible = true;
            buttonAddAccount.Enabled = true;

            pictureBoxClose.Visible = true;
            pictureBoxCopy.Visible = true;
            pictureBoxGeneratePassword.Visible = false;

            listBoxAccounts.Enabled = true;
        }

        private void pictureBoxGeneratePassword_Click(object sender, EventArgs e)
        {
            textBoxPassword.Text = Storage.GeneratePassword();
        }

        private void linkLabelSiteURL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string https = "";


            System.Diagnostics.Process.Start(linkLabelSiteURL.Text);
        }
    }
}
