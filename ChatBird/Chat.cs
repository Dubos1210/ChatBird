using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ChatBird
{
    public partial class Chat : Form
    {
        public string callsign;
        public string path;
        public string key = "ChatBird";
        public string temp;

        public string StringXOR(string t, string k, bool decode)
        {
            string t1 = "";
            if (t.Length > 0)
            {
                char ch;
                for (int i = 0; i < t.Length; i++)
                {
                    ch = (char)(t[i] ^ (char) (key[i % key.Length] + (byte) i));
                    t1 += ch;
                }
            }
            return t1;
        }

        public string time() {
            string t = DateTime.Now.ToString();
            t = t.Substring(t.IndexOf(" ") + 1);

            return t;
        }

        public Chat(string callsgn)
        {
            InitializeComponent();

            label1.Text = Properties.Settings.Default.title;

            Thread.Sleep(500);

            //Получаем настройки
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(@"settings");
                key = file.ReadLine();
                path = file.ReadLine();
                if (file.ReadLine()[0] == '1') baloonBox.Checked = true;
                if (file.ReadLine()[0] == '1') pcnameBox.Checked = true;
                file.Close();
            }
            catch (Exception ex)
            {
                chatBox.AppendText("Ошибка: " + ex.Message + "\n");
                chatBox.ScrollToCaret();
            }

            if (callsgn != "") callsign = callsgn;
            else callsign = System.Net.Dns.GetHostName();

            notifyIconMenu.Items["callsignItm"].Text = callsign;

            label2.Text = "Ваш позывной: " + callsign + "\n"
                           + "Имя ПК: " + System.Net.Dns.GetHostName() + "\n"
                           + "Поcледнее обновление чата: " + time() + "\n" + "\n"
                           + "Не забудьте выйти из системы и завершить работу утилиты!" + "\n" + "\n"
                           + "А кому теперь легко?";

            //Пишем "Присоединился"
            try
            {
                System.IO.StreamWriter file = File.AppendText(path);
                string data = "";
                if ((pcnameBox.Checked) && (callsign != System.Net.Dns.GetHostName())) data = "[" + time() + "] " + callsign + "@" + System.Net.Dns.GetHostName() + " присоединился к беседе";
                else data = "[" + time() + "] " + callsign + " присоединился к беседе";
                file.WriteLine(StringXOR(data, key, false));
                file.Close();
                msgBox.Text = "";
            }
            catch (Exception ex)
            {
                chatBox.AppendText("Ошибка: " + ex.Message + "\n");
                chatBox.ScrollToCaret();
            
            }

            try
            {
                chatBox.Clear();
                System.IO.StreamReader file = new System.IO.StreamReader(path);
                temp = file.ReadLine();

                while (temp != null)
                {
                    chatBox.AppendText(StringXOR(temp, key, true) + "\r\n");
                    chatBox.ScrollToCaret();

                    temp = file.ReadLine();
                }
                file.Close();
            }
            catch (Exception ex)
            {
                chatBox.AppendText("Ошибка: " + ex.Message + "\n");
                chatBox.ScrollToCaret();
            }

            //Настраиваем Watcher
            try
            {
                string abspath = Environment.CurrentDirectory + "\\" + path;
                fileSystemWatcher.Path = abspath.Substring(0, abspath.LastIndexOf('\\'));
                fileSystemWatcher.Filter = abspath.Substring(abspath.LastIndexOf('\\') + 1);
            }
            catch (Exception ex)
            {
                chatBox.AppendText("Ошибка: " + ex.Message + "\n");
                chatBox.ScrollToCaret();
            }
        }

        private void logoutBtn_Click(object sender, EventArgs e)
        {
            Form login = Application.OpenForms[0];
            login.Close();
            this.Close();
        }

        private void Chat_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (callsign != "")
            {
                try
                {
                    System.IO.StreamWriter file = File.AppendText(path);
                    if ((pcnameBox.Checked) && (callsign != System.Net.Dns.GetHostName())) file.WriteLine(StringXOR("[" + time() + "] " + callsign + "@" + System.Net.Dns.GetHostName() + " покинул беседу", key, false));
                    else file.WriteLine(StringXOR("[" + time() + "] " + callsign + " покинул беседу", key, false));
                    file.Close();
                    callsign = "";
                }
                catch (Exception ex)
                {
                    chatBox.Invoke((MethodInvoker)delegate
                    {
                        chatBox.AppendText("Ошибка: " + ex.Message + "\n");
                        chatBox.ScrollToCaret();
                    });
                }
            }
            Application.Exit();
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void Chat_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
                this.ShowInTaskbar = false;
                if(baloonBox.Checked) notifyIcon.ShowBalloonTip(500, Properties.Settings.Default.title + " " + Properties.Settings.Default.version, "Программа все еще работает в фоновом режиме", ToolTipIcon.Warning);
            }
        }

        private void msgBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (msgBox.Text != "")
                {
                    try
                    {
                        System.IO.StreamWriter file = File.AppendText(path);
                        string data = "";
                        if ((pcnameBox.Checked) && (callsign != System.Net.Dns.GetHostName())) data = "[" + time() + "] " + callsign + "@" + System.Net.Dns.GetHostName() + ": " + msgBox.Text;
                        else data = "[" + time() + "] " + callsign + ": " + msgBox.Text;
                        file.WriteLine(StringXOR(data, key, false));
                        file.Close();
                        msgBox.Text = "";
                    }
                    catch (Exception ex)
                    {
                        chatBox.Invoke((MethodInvoker)delegate
                        {
                            chatBox.AppendText("Ошибка: " + ex.Message + "\n");
                            chatBox.AppendText("Попробуйте повторить отправку сообщения" + "\n");
                            chatBox.ScrollToCaret();
                        });
                    }
                }
            }
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            if (msgBox.Text != "")
            {
                try
                {
                    System.IO.StreamWriter file = File.AppendText(path);
                    string data = "";
                    if ((pcnameBox.Checked) && (callsign != System.Net.Dns.GetHostName())) data = "[" + time() + "] " + callsign + "@" + System.Net.Dns.GetHostName() + ": " + msgBox.Text;
                    else data = "[" + time() + "] " + callsign + ": " + msgBox.Text;
                    file.WriteLine(StringXOR(data, key, false));
                    file.Close();
                    msgBox.Text = "";
                }
                catch (Exception ex)
                {
                    chatBox.Invoke((MethodInvoker)delegate
                    {
                        chatBox.AppendText("Ошибка: " + ex.Message + "\n");
                        chatBox.AppendText("Попробуйте повторить отправку сообщения" + "\n");
                        chatBox.ScrollToCaret();
                    });
                }
            }
        }

        private void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            label2.Invoke((MethodInvoker)delegate
            {
                label2.Text = "Ваш позывной: " + callsign + "\n"
                           + "Имя ПК: " + System.Net.Dns.GetHostName() + "\n"
                           + "Поcледнее обновление чата: " + time() + "\n" + "\n"
                           + "Не забудьте выйти из системы и завершить работу утилиты!" + "\n" + "\n"
                           + "А кому теперь легко?";
            });
            
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(path);            
                temp = file.ReadLine();

                string filedata = "";

                while (temp != null)
                {
                    filedata += StringXOR(temp, key, true) + "\r\n";
                    temp = file.ReadLine();
                }
                chatBox.Invoke((MethodInvoker)delegate
                {
                    //chatBox.Clear();
                    chatBox.Text = filedata;
                    //chatBox.AppendText(filedata);
                    chatBox.SelectionStart = chatBox.Text.Length;
                    chatBox.ScrollToCaret();
                });
                filedata = null;
                file.Close();
            }
            catch (Exception ex)
            {
                chatBox.Invoke((MethodInvoker)delegate
                {
                    chatBox.AppendText("Ошибка: " + ex.Message + "\n");
                    chatBox.ScrollToCaret();
                });
            }

            if ((chatBox.Lines.Length > 1) && baloonBox.Checked)
            {
                string temp = chatBox.Lines[chatBox.Lines.Length - 2];
                temp = temp.Substring(temp.IndexOf("]") + 2);
                if (temp.IndexOf("@") >= 0)
                {
                    if (temp.Substring(0, temp.IndexOf("@")) != callsign)
                    {
                        notifyIcon.ShowBalloonTip(500, "Сообщение", temp, ToolTipIcon.Info);
                    }
                }
                else
                {
                    if (temp.IndexOf(":") > 0)
                    {
                        if (temp.Substring(0, temp.IndexOf(":")) != callsign)
                        {
                            notifyIcon.ShowBalloonTip(500, "Сообщение", temp, ToolTipIcon.Info);
                        }
                    }
                    else if (temp.IndexOf(" ") > 0)
                    {
                        if (temp.Substring(0, temp.IndexOf(" ")) != callsign)
                        {
                            notifyIcon.ShowBalloonTip(500, "Сообщение", temp, ToolTipIcon.Info);
                        }
                    }
                }
            }
        }

        private void openChatItm_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void exitItm_Click(object sender, EventArgs e)
        {
            Form login = Application.OpenForms[0];
            login.Close();
            this.Close();
        }

        private void toolStripMenuTitle_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
        }
    }
}
