using System.IO;
using System.Net;
using System;
using System.Threading;
using N = System.Net;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices; 

namespace PC
{
    class ChatClient
    {

        static Form client;
        static N.Sockets.TcpClient tcpClient;

        [DllImport("kernel32.dll")]
        private static extern void ExitProcess(int a);

        public static void Main()
        {
            client = new Form();
            client.Text = "PCChat - Chat Client";
            client.Closing += new CancelEventHandler(talk_Closing);
            client.Controls.Add(new TextBox());
            client.Controls[0].Dock = DockStyle.Fill;
            client.Controls.Add(new TextBox());
            client.Controls[1].Dock = DockStyle.Bottom;
            ((TextBox)client.Controls[0]).Multiline = true;
            ((TextBox)client.Controls[1]).Multiline = true;
            client.WindowState = FormWindowState.Maximized;
            client.Show();
            ((TextBox)client.Controls[1]).KeyUp += new KeyEventHandler(key_up);
            tcpClient = new N.Sockets.TcpClient();
            tcpClient.Connect("enter your server IP here", 4296);
            Thread chatThread = new Thread(new ThreadStart(run));
            chatThread.Start();
            while (true)
            {
                Application.DoEvents();
            }
        }

        private static void talk_Closing(object s, CancelEventArgs e)
        {
            e.Cancel = false;
            Application.Exit();
            ExitProcess(0);
        }

        private static void key_up(object s, KeyEventArgs e)
        {
            TextBox txtChat = (TextBox)s;
            if (txtChat.Lines.Length > 1)
            {
                StreamWriter writer = new StreamWriter(tcpClient.GetStream());
                writer.WriteLine(txtChat.Text);
                writer.Flush();
                txtChat.Text = "";
                txtChat.Lines = null;
            }
        }

        private static void run()
        {
            StreamReader reader = new StreamReader(tcpClient.GetStream());
            while (true)
            {
                Application.DoEvents();
                TextBox txtChat = (TextBox)client.Controls[0];
                txtChat.AppendText(reader.ReadLine() + "\r\n");
                txtChat.SelectionStart = txtChat.Text.Length;
            }
        }
    }
}
