using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChatBird
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            Form chat = new Chat(callsignTxt.Text);
            chat.Show();
            this.Hide();
            callsignTxt.Enabled = false;
            loginBtn.Enabled = false;
        }

        private void callsignTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Form chat = new Chat(callsignTxt.Text);
                chat.Show();
                this.Hide();
                callsignTxt.Enabled = false;
                loginBtn.Enabled = false;
            }
        }
    }
}
