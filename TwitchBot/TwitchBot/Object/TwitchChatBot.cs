using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;

namespace TwitchBot.Object
{
    public class TwitchChatBot {
        public ConnectionData connectionData;
        public Data data;

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
                if (DateTime.Now > connectionData.timeOutTimer && !connectionData.connectedToChat)
                {
                    Console.WriteLine("Bot could not connect. Please check Config.txt information and try again...");
                    Thread.Sleep(8000);
                    Environment.Exit(0);
                }

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
            twitchChat.Close();
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
        public TwitchChatBot(string host, int port, string username, string password, string channelName, string _onlineMode, string _apiURL, bool reConnect) {
            data = new Data();

            connectionData = new ConnectionData();

            if (_onlineMode.ToUpper() == "TRUE")
            {
                connectionData.onlineMode = true;
                connectionData.apiURL = _apiURL;
            }
            else
                connectionData.onlineMode = false;

            connectionData.host = host;
            connectionData.port = port;

            connectionData.botName = username;
            connectionData.password = password;
            connectionData.channelName = channelName;

            connectionData.timeOutTimer = DateTime.Now.AddSeconds(5);

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
                Console.WriteLine("Connecting...");
            }
            catch(SocketException e) 
            {
                Console.WriteLine("Message: " + e.Message);
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
                        if(Program.debug)
                            Console.WriteLine("Sending message: " + messageQueue.Peek());
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

                if (message.Contains("You are in a maze of twisty passages, all alike."))
                {
                    connectionData.connectedToChat = true;
                    Console.WriteLine("Bot connected to " + connectionData.channelName + ". Online mode: " + connectionData.onlineMode.ToString());
                }

                if (!connectionData.connectedToChat)
                    return;

                if (message.Contains("PING"))
                {
                    message = message.Replace("PING", "PONG");
                    if (Program.debug)
                        Console.WriteLine("PING Recieved; Responding with PONG: " + message);
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
                        data.hungry.timesHungryTotal++;
                        data.hungry.session_timesHungryTotal++;
                        if (DateTime.Now > data.hungry.addNextTime)
                        {
                        data.hungry.timesHungry++;
                        data.hungry.session_timesHungry++;
                        if (connectionData.onlineMode)
                            SendData();
                        data.hungry.addNextTime = DateTime.Now.AddSeconds(30);
                            Console.WriteLine("IN: " + DateTime.Now + " - Hungry registered!");
                        }
                        break;
                case "!removehungry":
                    data.hungry.timesHungryTotal--;
                    data.hungry.session_timesHungry--;
                    if (DateTime.Now > data.hungry.removeNextTime)
                    {
                        data.hungry.timesHungry--;
                        data.hungry.session_timesHungry--;
                        if (connectionData.onlineMode)
                            SendData();
                        data.hungry.removeNextTime = DateTime.Now.AddSeconds(30);
                        Console.WriteLine("IN: " + DateTime.Now + " - Hungry removed!");
                    }
                    break;
                case "!howhungry":
                        Console.WriteLine("OUT: " + DateTime.Now + " - The word \"hungry\" has been said " + data.hungry.timesHungry.ToString() + " time(s). This session: " + data.hungry.session_timesHungry.ToString() + " time(s).");
                        SendMsg("The word \"hungry\" has been said " + data.hungry.timesHungry.ToString() + " time(s). This session: " + data.hungry.session_timesHungry.ToString() + " time(s).");
                        break;
                    case "!hungrycount":
                        Console.WriteLine("OUT: " + DateTime.Now + " - Counted the command \"!hungry\" being called " + data.hungry.timesHungryTotal.ToString() + " time(s). This session: " + data.hungry.session_timesHungryTotal.ToString() + " time(s).");
                        SendMsg("Counted the command \"!hungry\" being called " + data.hungry.timesHungryTotal.ToString() + " time(s). This session: " + data.hungry.session_timesHungryTotal.ToString() + " time(s).");
                        break;
                case "!death":
                case "!dead":
                case "!died":
                    data.deaths.deathsTotal++;
                    data.deaths.session_deathsTotal++;
                    if (DateTime.Now > data.deaths.addNextTime)
                    {
                        data.deaths.deaths++;
                        data.deaths.session_deaths++;
                        if (connectionData.onlineMode)
                            SendData();
                        data.deaths.addNextTime = DateTime.Now.AddSeconds(30);
                        Console.WriteLine("IN: " + DateTime.Now + " - Death registered!");
                    }
                    break;
                case "!removedeath":
                    data.deaths.deathsTotal--;
                    data.deaths.session_deathsTotal--;
                    if (DateTime.Now > data.deaths.removeNextTime)
                    {
                        data.deaths.deaths--;
                        data.deaths.session_deaths--;
                        if (connectionData.onlineMode)
                            SendData();
                        data.deaths.removeNextTime = DateTime.Now.AddSeconds(30);
                        Console.WriteLine("IN: " + DateTime.Now + " - Death removed!");
                    }
                    break; 
                case "!deathcount":
                    Console.WriteLine("OUT: " + DateTime.Now + " - " + connectionData.channelName + " has died " + data.deaths.deaths.ToString() + " time(s). This session: " + data.deaths.deaths.ToString() + " time(s).");
                    SendMsg("" + connectionData.channelName + " has died " + data.hungry.timesHungry.ToString() + " time(s). This session: " + data.deaths.deaths.ToString() + " time(s).");
                    break;
                case "!deathcounttotal":
                    Console.WriteLine("OUT: " + DateTime.Now + " - Counted the command death being called " + data.deaths.deathsTotal.ToString() + " time(s). This session: " + data.deaths.deathsTotal.ToString() + " time(s).");
                    SendMsg("Counted the command \"!hungry\" being called " + data.hungry.timesHungryTotal.ToString() + " time(s). This session: " + data.deaths.deathsTotal.ToString() + " time(s).");
                    break;
                case "!help":
                        Console.WriteLine("OUT: " +  DateTime.Now + " - Printed out !help message in chat.");
                        SendMsg("Write !hungry if the streamer said he/she is hungry to add to counter. !howhungry will tell you the counted number. !hungrycount will print out ALL information. !removehungry will decrease the counter.");
                        SendMsg("Write !death, !dead, !died once the streamer gets killed to add to counter. !deathcount will tell you the counted number. !deathcounttotal will print out ALL information. !removedeath will decrease the counter.");
                        SendMsg("!help displays this message");
                        break;
            }
        }

        public void DownloadHungry() {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadStringCompleted += Wc_DownloadStringCompleted;
                wc.Encoding = System.Text.Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                if (Program.debug)
                    Console.WriteLine("API: " + connectionData.apiURL + "?ch=" + connectionData.channelName + "&bot=" + connectionData.botName + "&d");
                wc.DownloadStringAsync(new Uri(connectionData.apiURL + "?ch=" + connectionData.channelName + "&bot=" + connectionData.botName + "&d"));
            }
        }

        private void Wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e) {
            try
            {
                if (e.Error != null) Console.WriteLine("Download: The remote server returned an error: " + e.Error.Message);
            }
            catch (System.Reflection.TargetInvocationException)
            {
                Console.WriteLine("Download Error: No data.");
            }

            if (Program.debug)
            {
                try
                {
                    if (e.Result != null) Console.WriteLine("Download: The remote server returned: " + e.Result);
                }
                catch (System.Reflection.TargetInvocationException)
                {
                    Console.WriteLine("Download Result: No data.");
                }
            }

            if (e.Error == null)
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
                            data = JsonConvert.DeserializeObject<Data>(e.Result);
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

        public void SendData() {
            string sndData = JsonConvert.SerializeObject(data);

            using(WebClient wc = new WebClient()) {
                wc.UploadStringCompleted += Wc_UploadStringCompleted;
                wc.Encoding = System.Text.Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.UploadStringAsync(new Uri(connectionData.apiURL + "?ch=" + connectionData.channelName + "&bot=" + connectionData.botName + "&u"), "POST", sndData);
            }
        }

        private void Wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e) {
            try
            {
                if (e.Error != null) Console.WriteLine("Upload: The remote server returned an error: " + e.Error.Message);
            }
            catch (System.Reflection.TargetInvocationException)
            {
                Console.WriteLine("Upload Error: No data.");
            }

            if (Program.debug)
            {
                try
                {
                    if (e.Result != null) Console.WriteLine("Download: The remote server returned: " + e.Result);
                }
                catch (System.Reflection.TargetInvocationException)
                {
                    Console.WriteLine("Upload Result: No data.");
                }
            }

            if (e.Error == null)
            {
                switch (e.Result)
                {
                    case "OK":
                        Console.WriteLine("Console: Upload complete.");
                        break;
                    case "NoWrite":
                    case "NoAccess":
                        Console.WriteLine("Console: Upload failed. Write access denied.");
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
