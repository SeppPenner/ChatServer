using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Chat = System.Net;

namespace PC
{
    internal class DoCommunicate
    {
        private readonly TcpClient _client;
        private string _nickName;
        private StreamReader _reader;
        private StreamWriter _writer;

        public DoCommunicate(TcpClient tcpClient)
        {
            //create our TcpClient
            _client = tcpClient;
            //create a new thread
            var chatThread = new Thread(StartChat);
            //start the new thread
            chatThread.Start();
        }

        private string GetNick()
        {
            //ask the user what nickname they want to use
            _writer.WriteLine("What is your nickname? ");
            //ensure the buffer is empty
            _writer.Flush();
            //return the value the user provided
            return _reader.ReadLine();
        }

        private void RunChat()
            //use a try...catch to catch any exceptions
        {
            try
            {
                //set out line variable to an empty string
                while (true)
                {
                    //read the curent line
                    var line = _reader.ReadLine();
                    //send our message
                    ChatServer.SendMsgToAll(_nickName, line);
                }
            }
            catch (Exception e44)
            {
                Console.WriteLine(e44);
            }
        }

        private void StartChat()
        {
            //create our StreamReader object to read the current stream
            _reader = new StreamReader(_client.GetStream());
            //create our StreamWriter objec to write to the current stream
            _writer = new StreamWriter(_client.GetStream());
            _writer.WriteLine("Welcome to PCChat!");
            //retrieve the users nickname they provided
            _nickName = GetNick();
            //check is the nickname is already in session
            //prompt the user until they provide a nickname not in use
            while (ChatServer.NickName.Contains(_nickName))
            {
                //since the nickname is in use we display that message,
                //then prompt them again for a nickname
                _writer.WriteLine("ERROR - Nickname already exists! Please try a new one");
                _nickName = GetNick();
            }
            //add their nickname to the chat server
            ChatServer.NickName.Add(_nickName, _client);
            ChatServer.NickNameByConnect.Add(_client, _nickName);
            //send a system message letting the other user
            //know that a new user has joined the chat
            ChatServer.SendSystemMessage("** " + _nickName + " ** Has joined the room");
            _writer.WriteLine("Now Talking.....\r\n-------------------------------");
            //ensure the buffer is empty
            _writer.Flush();
            //create a new thread for this user
            var chatThread = new Thread(RunChat);
            //start the thread
            chatThread.Start();
        }
    }
}