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
        public bool baloon = false;
        public bool pcname = false;
        public byte key = 0xFF;
        public string temp;

        public string StringXOR(string t, byte k) {
            string t1 = "";
            for (int i = 0; i < t.Length; i++)
            {
                char ch = (char)(t[i] ^ (char) k);
                t1 += ch;
            }
            return t1;
        }

        public Chat(string callsgn)
        {
            InitializeComponent();

            label1.Text = Properties.Settings.Default.title;

            Thread.Sleep(500);        

            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(@"settings");
                temp = file.ReadLine();
                if (temp.Length > 0)
                {
                    int sum = 0;
                    for (int i = 0; i < temp.Length; i++)
                    {
                        sum += temp[i];
                    }
                    sum /= temp.Length;
                    key = (byte) sum;
                }
                path = file.ReadLine();
                if (file.ReadLine()[0] == '1') baloon = true;
                if (file.ReadLine()[0] == '1') pcname = true;
                file.Close();
            }
            catch (Exception ex)
            {
                chatBox.AppendText("Ошибка: " + ex.Message + "\n");
                chatBox.ScrollToCaret();
            }

            if (callsgn != "") callsign = callsgn;
            else callsign = System.Net.Dns.GetHostName();

            label2.Text = "Ваш позывной: " + callsign + "\n"
                           + "Имя ПК: " + System.Net.Dns.GetHostName() + "\n"
                           + "Поcледнее обновление чата: " + DateTime.Now.ToString() + "\n" + "\n"
                           + "Не забудьте выйти из системы и завершить работу утилиты!" + "\n" + "\n"
                           + "А кому теперь легко?";

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

            try
            {
                System.IO.StreamWriter file = File.AppendText(path);
                if ((pcname) && (callsign != System.Net.Dns.GetHostName())) file.WriteLine(StringXOR("[" + DateTime.Now.ToString() + "] " + callsign + "@" + System.Net.Dns.GetHostName() + " присоединился к беседе", key));
                else file.WriteLine(StringXOR("[" + DateTime.Now.ToString() + "] " + callsign + " присоединился к беседе", key));
                file.Close();
                msgBox.Text = "";
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
                    if ((pcname) && (callsign != System.Net.Dns.GetHostName())) file.WriteLine(StringXOR("[" + DateTime.Now.ToString() + "] " + callsign + "@" + System.Net.Dns.GetHostName() + " покинул беседу", key));
                    else file.WriteLine(StringXOR("[" + DateTime.Now.ToString() + "] " + callsign + " покинул беседу", key));
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
                notifyIcon.ShowBalloonTip(500, "ChatBird", "Программа все еще работает в фоновом режиме", ToolTipIcon.Warning);
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
                        if ((pcname) && (callsign != System.Net.Dns.GetHostName())) file.WriteLine(StringXOR("[" + DateTime.Now.ToString() + "] " + callsign + "@" + System.Net.Dns.GetHostName() + ": " + msgBox.Text, key));
                        else file.WriteLine(StringXOR("[" + DateTime.Now.ToString() + "] " + callsign + ": " + msgBox.Text, key));
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
                    if ((pcname) && (callsign != System.Net.Dns.GetHostName())) file.WriteLine(StringXOR("[" + DateTime.Now.ToString() + "] " + callsign + "@" + System.Net.Dns.GetHostName() + ": " + msgBox.Text, key));
                    else file.WriteLine(StringXOR("[" + DateTime.Now.ToString() + "] " + callsign + ": " + msgBox.Text, key));
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
                           + "Поcледнее обновление чата: " + DateTime.Now.ToString() + "\n" + "\n"
                           + "Не забудьте выйти из системы и завершить работу утилиты!" + "\n" + "\n"
                           + "А кому теперь легко?";
            });
            
            try
            {
                chatBox.Invoke((MethodInvoker)delegate
                {
                    chatBox.Clear();
                });
                System.IO.StreamReader file = new System.IO.StreamReader(path);            
                temp = file.ReadLine();
                while (temp != null)
                {
                    chatBox.Invoke((MethodInvoker)delegate
                    {
                        chatBox.AppendText(StringXOR(temp, key) + "\r\n");
                        chatBox.ScrollToCaret();
                    });
                    temp = file.ReadLine();
                }
                file.Close();
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

            if ((chatBox.Lines.Length > 1) && baloon)
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
