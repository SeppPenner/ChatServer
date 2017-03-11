using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;

//*****************************************************************************************
//                           LICENSE INFORMATION
//*****************************************************************************************
//   PC_Chat 1.0.0.0
//   Creates a basic basic server/client chat application in C#
//
//   Copyright (C) 2007  
//   Richard L. McCutchen 
//   Email: richard@psychocoder.net
//   Created: 16SEP07
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
//*****************************************************************************************

namespace PC
{
    public class ChatServer
    {
        public static Hashtable NickName;
        public static Hashtable NickNameByConnect;

        private ChatServer(int port)
        {
            //create our nickname and nickname by connection variables
            NickName = new Hashtable(100);
            NickNameByConnect = new Hashtable(100);
            //create our TCPListener object
            var chatServer = new TcpListener(port);
            //check to see if the server is running
            //while (true) do the commands
            while (true)
            {
                //start the chat server
                chatServer.Start();
                //check if there are any pending connection requests
                if (!chatServer.Pending()) continue;
                //if there are pending requests create a new connection
                var chatConnection = chatServer.AcceptTcpClient();
                //display a message letting the user know they're connected
                Console.WriteLine("You are now connected");
                //create a new DoCommunicate Object
                var doCommunicate = new DoCommunicate(chatConnection);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public static void Main()
        {
            var chatServer = new ChatServer(4296);
        }

        public static void SendMsgToAll(string nick, string msg)
        {
            //create a new TCPClient Array
            var tcpClient = new TcpClient[NickName.Count];
            //copy the users nickname to the CHatServer values
            NickName.Values.CopyTo(tcpClient, 0);
            //loop through and write any messages to the window
            foreach (var t in tcpClient)
                try
                {
                    //check if the message is empty, of the particular
                    //index of out array is null, if it is then continue
                    if (msg.Trim() == "" || t == null)
                        continue;
                    //Use the GetStream method to get the current memory
                    //stream for this index of our TCPClient array
                    var writer = new StreamWriter(t.GetStream());
                    //white our message to the window
                    writer.WriteLine(nick + ": " + msg);
                    //make sure all bytes are written
                    writer.Flush();
                }
                //here we catch an exception that happens
                //when the user leaves the chatroow
                catch (Exception)
                {
                    if (t != null)
                    {
                        var str = (string) NickNameByConnect[t];
                        //send the message that the user has left
                        SendSystemMessage("** " + str + " ** Has Left The Room.");
                        //remove the nickname from the list
                        NickName.Remove(str);
                    }
                    //remove that index of the array, thus freeing it up
                    //for another user
                    if (t != null) NickNameByConnect.Remove(t);
                }
        }

        public static void SendSystemMessage(string msg)
        {
            //create our TcpClient array
            var tcpClient = new TcpClient[NickName.Count];
            //copy the nickname value to the chat servers list
            NickName.Values.CopyTo(tcpClient, 0);
            //loop through and write any messages to the window
            foreach (var t in tcpClient)
                try
                {
                    //check if the message is empty, of the particular
                    //index of out array is null, if it is then continue
                    if (msg.Trim() == "" || t == null)
                        continue;
                    //Use the GetStream method to get the current memory
                    //stream for this index of our TCPClient array
                    var writer = new StreamWriter(t.GetStream());
                    //send our message
                    writer.WriteLine(msg);
                    //make sure the buffer is empty
                    writer.Flush();
                }
                catch (Exception)
                {
                    if (t == null) continue;
                    NickName.Remove(NickNameByConnect[t]);
                    NickNameByConnect.Remove(t);
                }
        }
    } //end of class ChatServer
}