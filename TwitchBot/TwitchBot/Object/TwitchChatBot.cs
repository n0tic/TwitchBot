using TwitchBot.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TwitchBot.Object
{
    public class TwitchChatBot
    {
        ConnectionData connectionData;
        public HungryData hungry = new HungryData();

        TcpClient twitchChat;
        StreamReader reader;
        StreamWriter writer;
        Thread commandSender;
        Thread commandReader;
        bool runCommandSender = true;

        bool reconnect = true;

        public Queue<string> messageQueue = new Queue<string>();

        /// <summary>
        /// This will read the chat and put the thread in sleep.
        /// </summary>
        public void Update()
        {
            while(true)
            {
                //Does this trigger an error since it is running on a different thread? Probably.
                //if (twitchChat == null || !twitchChat.Connected && reconnect) Connect();

                ReadChat();
                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// Kills both threads.
        /// </summary>
        public void KillThreads()
        {
            commandSender.Abort();
            commandReader.Abort();
        }

        /// <summary>
        /// Creates an object of the twitchChatBot.
        /// </summary>
        /// <param name="host">string ip/address</param>
        /// <param name="port">int port address</param>
        /// <param name="username">string botname</param>
        /// <param name="password">string oAuthPassword</param>
        /// <param name="channelName">string channelname to join</param>
        /// <param name="reConnect">bool reconnect feature</param>
        public TwitchChatBot(string host, int port, string username, string password, string channelName, bool reConnect)
        {
            hungry = new HungryData();

            connectionData = new ConnectionData();
            connectionData.host = host;
            connectionData.port = port;

            connectionData.botName = username;
            connectionData.password = password;
            connectionData.channelName = channelName;

            reconnect = reConnect;

            Connect();
        }

        /// <summary>
        /// Creates the connection to the chat server.
        /// </summary>
        private void Connect()
        {
            try
            {
                twitchChat = new TcpClient(connectionData.host, connectionData.port);
            }
            catch(SocketException e) 
            {
                Console.WriteLine("Message: " + e);
                Thread.Sleep(8000);
                Environment.Exit(0);
            }

            reader = new StreamReader(twitchChat.GetStream());
            writer = new StreamWriter(twitchChat.GetStream());

            ConnectToChannel(connectionData.botName, connectionData.password, connectionData.channelName);

            if (twitchChat.Connected)
            {
                commandSender = new Thread(() => runQueue(writer));
                commandSender.Start();
                commandReader = new Thread(() => Update());
                commandReader.Start();
                Console.WriteLine("Bot connected and operational. Connected to " + connectionData.channelName);
            }
        }

        /// <summary>
        /// Connects the user to the channel.
        /// </summary>
        /// <param name="username">string botname</param>
        /// <param name="password">string oAuthPassword</param>
        /// <param name="channelName">string channelname</param>
        private void ConnectToChannel(string username, string password, string channelName)
        {
            writer.WriteLine("PASS " + password);
            writer.WriteLine("NICK " + username.ToLower());
            writer.WriteLine("USER " + username.ToLower() + " 8 * :" + username.ToLower());
            writer.WriteLine("JOIN #" + channelName);
            writer.Flush();
        }

        /// <summary>
        /// Runs the commandqueue and executes commands if ther are any.
        /// </summary>
        /// <param name="writer">twitch chat</param>
        void runQueue(StreamWriter writer)
        {
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            while (runCommandSender) {
                if (messageQueue.Count > 0 && twitchChat.Connected) {
                    if (stopWatch.ElapsedMilliseconds > 1750) { // Check so that enough time has passed.
                        writer.WriteLine(messageQueue.Peek());
                        writer.Flush();

                        messageQueue.Dequeue();

                        stopWatch.Reset();
                        stopWatch.Start();
                    }
                }
                else
                    Thread.Sleep(50); // Stop overflow.
            }
        }

        /// <summary>
        /// Reads the twitch chat
        /// </summary>
        void ReadChat()
        {
            if (twitchChat.Available > 0) {
                string message = reader.ReadLine();

                if (message.Contains("PING"))
                {
                    message = message.Replace("PING", "PONG");
                    writer.WriteLine(message);
                    writer.Flush();
                    return;
                }

                if (message.Contains("PRIVMSG"))
                {
                    string user, msg;

                    user = message.Split('!')[0].Trim(':');
                    msg = message.Split(':')[2];

                    UseInformation(user, msg);
                }
            }
        }

        /// <summary>
        /// Use the information from the chat.
        /// </summary>
        /// <param name="usr">string username of sender</param>
        /// <param name="msg">string message</param>
        void UseInformation(string usr, string msg)
        {
            msg = msg.ToLower();

            //DO switch case
            switch (msg)
            {
                case "!hungry":
                    hungry.timesHungryTotal++;
                    if (DateTime.Now > hungry.nextTime)
                    {
                        hungry.timesHungry++;
                        hungry.nextTime = DateTime.Now.AddSeconds(30);
                    }
                    break;
                case "!howhungry":
                    Console.WriteLine(DateTime.Now + ": The word \"hungry\" has been said " + hungry.timesHungry.ToString() + " time(s). (Only counts 1 per 30 seconds)");
                    SendMsg("The word \"hungry\" has been said " + hungry.timesHungry.ToString() + " time(s).");
                    break;
                case "!hungrycount":
                    Console.WriteLine(DateTime.Now + ": Counted the command \"!hungry\" being called " + hungry.timesHungryTotal.ToString() + " time(s).");
                    SendMsg("Counted the command \"!hungry\" being called " + hungry.timesHungryTotal.ToString() + " time(s).");
                    break;
                case "!help":
                    Console.WriteLine(DateTime.Now + ": !hungry, !howhungry, !hungrycount, !help. !hungry should be called once the word \"Hungry\" is heard on stream. !howhungry will print out how many times it has been counted. !hungrycount will print out all the counted commands, even those within the 1 per 30 seconds rule. !hungrycount will print out ALL the times !hungry was found in chat. !help displays this message.");
                    SendMsg("!hungry, !howhungry, !help. !hungry should be called once the word \"Hungry\" is heard on stream. !howhungry will print out how many times it has been counted. !hungrycount will print out all the counted commands, even those within the 1 per 30 seconds rule. !hungrycount will print out ALL the times !hungry was found in chat. !help displays this message.");
                    break;
            }
        }

        /// <summary>
        /// Sends the message to the chat
        /// </summary>
        /// <param name="msg"></param>
        void SendMsg(string msg) => messageQueue.Enqueue("PRIVMSG #" + connectionData.channelName + " :" + msg);
    }
}
