// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoCommunicate.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The communication class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ChatServer;

/// <summary>
/// The communication class.
/// </summary>
public class DoCommunicate
{
    /// <summary>
    /// The TCP client.
    /// </summary>
    private readonly TcpClient tcpClient;

    /// <summary>
    /// The nick name.
    /// </summary>
    private string? nickName;

    /// <summary>
    /// The reader.
    /// </summary>
    private StreamReader? reader;

    /// <summary>
    /// The writer.
    /// </summary>
    private StreamWriter? writer;

    /// <summary>
    /// Initializes a new instance of the <see cref="DoCommunicate"/> class.
    /// </summary>
    /// <param name="tcpClient">The TCP client.</param>
    public DoCommunicate(TcpClient tcpClient)
    {
        this.tcpClient = tcpClient;
        var chatThread = new Thread(this.StartChat);
        chatThread.Start();
    }

    /// <summary>
    /// Gets the nick name.
    /// </summary>
    /// <returns>The nick name as <see cref="string"/>.</returns>
    private string? GetNickName()
    {
        if (this.writer is null)
        {
            return null;
        }

        this.writer.WriteLine("What is your nickname? ");
        this.writer.Flush();
        return this.reader?.ReadLine();
    }

    /// <summary>
    /// Runs the chat.
    /// </summary>
    private void RunChat()
    {
        try
        {
            while (true)
            {
                var line = this.reader?.ReadLine();

                if (!string.IsNullOrWhiteSpace(this.nickName) && !string.IsNullOrWhiteSpace(line))
                {
                    ChatServer.SendMsgToAll(this.nickName, line);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// Starts the chat.
    /// </summary>
    private void StartChat()
    {
        this.reader = new StreamReader(this.tcpClient.GetStream());
        this.writer = new StreamWriter(this.tcpClient.GetStream());
        this.writer.WriteLine("Welcome to PCChat!");
        this.nickName = this.GetNickName();

        if (string.IsNullOrWhiteSpace(this.nickName))
        {
            return;
        }

        while (ChatServer.NickNames.Contains(this.nickName!))
        {
            this.writer.WriteLine("ERROR - Nickname already exists! Please try a new one");
            this.nickName = this.GetNickName();
        }

        ChatServer.NickNames.Add(this.nickName!, this.tcpClient);
        ChatServer.NickNameByConnect.Add(this.tcpClient, this.nickName);
        ChatServer.SendSystemMessage("** " + this.nickName + " ** Has joined the room");
        this.writer.WriteLine($"Now Talking.....{Environment.NewLine}-------------------------------");
        this.writer.Flush();
        var chatThread = new Thread(this.RunChat);
        chatThread.Start();
    }
}
