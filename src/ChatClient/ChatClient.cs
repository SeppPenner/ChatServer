// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatClient.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The chat client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ChatClient;

/// <summary>
/// The chat client.
/// </summary>
public partial class ChatClient
{
    /// <summary>
    /// The client.
    /// </summary>
    private static Form client = new();

    /// <summary>
    /// The TCP client.
    /// </summary>
    private static TcpClient tcpClient = new();

    /// <summary>
    /// The main method.
    /// </summary>
    public static void Main()
    {
        client = new Form { Text = "PCChat - Chat Client" };

        client.Closing += ChatClientClosing!;
        client.Controls.Add(new TextBox());
        client.Controls[0].Dock = DockStyle.Fill;
        client.Controls.Add(new TextBox());
        client.Controls[1].Dock = DockStyle.Bottom;
        ((TextBox)client.Controls[0]).Multiline = true;
        ((TextBox)client.Controls[1]).Multiline = true;
        client.WindowState = FormWindowState.Maximized;
        client.Show();
        ((TextBox)client.Controls[1]).KeyUp += KeyUp!;
        tcpClient = new TcpClient();
        tcpClient.Connect("127.0.0.1", 4296);
        var chatThread = new Thread(Run);
        chatThread.Start();

        while (true)
        {
            Application.DoEvents();
        }
    }

    /// <summary>
    /// Handles the key up event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private static void KeyUp(object sender, KeyEventArgs e)
    {
        var txtChat = (TextBox)sender;

        if (txtChat.Lines.Length <= 1)
        {
            return;
        }

        var writer = new StreamWriter(tcpClient.GetStream());
        writer.WriteLine(txtChat.Text);
        writer.Flush();
        txtChat.Text = string.Empty;
        txtChat.Lines = null;
    }

    /// <summary>
    /// Exits the progress.
    /// </summary>
    /// <param name="a">A process exit code.</param>
    [DllImport("kernel32.dll")]
    private static extern void ExitProcess(int a);

    /// <summary>
    /// Handles the chat client closing event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private static void ChatClientClosing(object sender, CancelEventArgs e)
    {
        e.Cancel = false;
        Application.Exit();
        ExitProcess(0);
    }

    /// <summary>
    /// Runs the chat client.
    /// </summary>
    private static void Run()
    {
        var reader = new StreamReader(tcpClient.GetStream());

        while (true)
        {
            Application.DoEvents();
            var txtChat = (TextBox)client.Controls[0];

            txtChat.UiThreadInvoke(
                () =>
                {
                    txtChat.AppendText($"{reader.ReadLine()}{Environment.NewLine}");
                    txtChat.SelectionStart = txtChat.Text.Length;
                });
        }
    }
}
