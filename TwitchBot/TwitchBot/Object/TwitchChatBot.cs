using TwitchBot.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Newtonsoft.Json;

namespace TwitchBot.Object
{
    public class TwitchChatBot {
        public ConnectionData connectionData;
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
                //Does this trigger an error since it is running on a different thread? Probably, commenting out.
                //if (twitchChat == null || !twitchChat.Connected && reconnect) Connect();

                ReadChat();
                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// Kills both threads.
        /// </summary>
        public void KillThreads() {
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
        public TwitchChatBot(string host, int port, string username, string password, string channelName, string _onlineMode, bool reConnect) {
            hungry = new HungryData();

            connectionData = new ConnectionData();
            if (_onlineMode == "TRUE")
                connectionData.onlineMode = true;
            else
                connectionData.onlineMode = false;
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
                Console.WriteLine("Bot connected to " + connectionData.channelName + ". Online mode: " + connectionData.onlineMode.ToString());

                if(connectionData.onlineMode)
                    DownloadHungry();
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
                        if (connectionData.onlineMode)
                            SendHungry();
                        hungry.nextTime = DateTime.Now.AddSeconds(30);
                        Console.WriteLine("IN: " + DateTime.Now + " - Hungry registered!");
                    }
                    break;
                case "!howhungry":
                    Console.WriteLine("OUT: " + DateTime.Now + " - The word \"hungry\" has been said " + hungry.timesHungry.ToString() + " time(s).");
                    SendMsg("The word \"hungry\" has been said " + hungry.timesHungry.ToString() + " time(s).");
                    break;
                case "!hungrycount":
                    Console.WriteLine("OUT: " + DateTime.Now + " - Counted the command \"!hungry\" being called " + hungry.timesHungryTotal.ToString() + " time(s).");
                    SendMsg("Counted the command \"!hungry\" being called " + hungry.timesHungryTotal.ToString() + " time(s).");
                    break;
                case "!help":
                    Console.WriteLine("OUT: " +  DateTime.Now + " - !hungry should be called once the word \"Hungry\" is heard verbally on stream. !howhungry will print out how many times it has been counted. !hungrycount will print out ALL the times \"!hungry\" was found in chat. !help displays this message.");
                    SendMsg("!hungry should be called once the word \"Hungry\" is heard verbally on stream. !howhungry will print out how many times it has been counted. !hungrycount will print out ALL the times \"!hungry\" was found in chat. !help displays this message.");
                    break;
            }
        }

        public void DownloadHungry()
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadStringCompleted += Wc_DownloadStringCompleted;
                wc.Encoding = System.Text.Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.DownloadStringAsync(new Uri("https://bytevaultstudio.se/projects/mix/TwitchBot/?ch=" + connectionData.channelName + "&d"));
            }
        }

        private void Wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if(e.Error == null)
            {
                switch (e.Result)
                {
                    case "NoContent":
                    case "NoFile":
                        Console.WriteLine("Console: No data could be found. Using new data.");
                        break;
                    default:
                        try
                        {
                            hungry = JsonConvert.DeserializeObject<HungryData>(e.Result);
                            Console.WriteLine("Console: Data loaded.");
                        }
                        catch (JsonException)
                        {
                            Console.WriteLine("Console: Deserialize of data failed. Resettings data.");
                        }
                        break;
                }
            }
        }

        public void SendHungry() {
            string data = JsonConvert.SerializeObject(hungry);

            using(WebClient wc = new WebClient()) {
                wc.UploadStringCompleted += Wc_UploadStringCompleted;
                wc.Encoding = System.Text.Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.UploadStringAsync(new Uri("https://bytevaultstudio.se/projects/mix/TwitchBot/?ch=" + connectionData.channelName + "&u"), "POST", data);
            }
        }

        private void Wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e) {
            if (e.Error == null)
            {
                switch (e.Result)
                {
                    case "OK":
                        Console.WriteLine("Console: Upload complete.");
                        break;
                    case "NoWrite":
                    case "NoAccess":
                        Console.WriteLine("Console: Upload failed.");
                        break;
                }
            }
        }

        /// <summary>
        /// Sends the message to the chat
        /// </summary>
        /// <param name="msg"></param>
        void SendMsg(string msg) => messageQueue.Enqueue("PRIVMSG #" + connectionData.channelName + " :" + msg);
    }
}
