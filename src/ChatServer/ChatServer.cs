// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatServer.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The chat server.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ChatServer
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// The chat server.
    /// </summary>
    public class ChatServer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatServer"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        private ChatServer(int port)
        {
            NickNames = new Hashtable(100);
            NickNameByConnect = new Hashtable(100);
            var chatServer = new TcpListener(IPAddress.Loopback, port);

            while (true)
            {
                chatServer.Start();
                if (!chatServer.Pending())
                {
                    continue;
                }

                var chatConnection = chatServer.AcceptTcpClient();
                Console.WriteLine("You are now connected.");
                _ = new DoCommunicate(chatConnection);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        /// <summary>
        /// Gets or sets the nick names.
        /// </summary>
        public static Hashtable NickNames { get; set; } = new();

        /// <summary>
        /// Gets or sets the nick names by connect.
        /// </summary>
        public static Hashtable NickNameByConnect { get; set; } = new();

        /// <summary>
        /// The main method.
        /// </summary>
        public static void Main()
        {
            _ = new ChatServer(4296);
        }

        /// <summary>
        /// Sends a message to all people in the chat.
        /// </summary>
        /// <param name="nickname">The nick name.</param>
        /// <param name="message">The message.</param>
        public static void SendMsgToAll(string nickname, string message)
        {
            var tcpClient = new TcpClient[NickNames.Count];
            NickNames.Values.CopyTo(tcpClient, 0);

            foreach (var t in tcpClient)
            {
                try
                {
                    if (message.Trim() == string.Empty || t == null)
                    {
                        continue;
                    }

                    var writer = new StreamWriter(t.GetStream());
                    writer.WriteLine($"{nickname}: {message}");
                    writer.Flush();
                }
                catch (Exception)
                {
                    if (t != null)
                    {
                        var nickName = NickNameByConnect[t];

                        if (nickName is string)
                        {
                            var text = nickName as string;
                            SendSystemMessage($"** {text} ** has left the room.");

                            if (!string.IsNullOrWhiteSpace(text))
                            {
                                NickNames.Remove(text);
                            }
                        }
                    }

                    if (t != null)
                    {
                        NickNameByConnect.Remove(t);
                    }
                }
            }
        }

        /// <summary>
        /// Sends a system message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void SendSystemMessage(string message)
        {
            var tcpClient = new TcpClient[NickNames.Count];
            NickNames.Values.CopyTo(tcpClient, 0);
            foreach (var t in tcpClient)
            {
                try
                {
                    if (message.Trim() == string.Empty || t == null)
                    {
                        continue;
                    }

                    var writer = new StreamWriter(t.GetStream());
                    writer.WriteLine(message);
                    writer.Flush();
                }
                catch (Exception)
                {
                    if (t == null)
                    {
                        continue;
                    }

                    var nickName = NickNameByConnect[t];

                    if (nickName is not null)
                    {
                        NickNames.Remove(nickName);
                    }

                    NickNameByConnect.Remove(t);
                }
            }
        }
    }
}