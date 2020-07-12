using TwitchBot.Extra;
using TwitchBot.Object;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using System.Net;

namespace TwitchBot
{
    public class Program
    {
        const string version = "V0.3";
        public static bool debug = true;

        //Twitch bot (Read, Send, Resolve)
        public static TwitchChatBot twitchChatBot;

        static List<Dictionary<string, string>> data = new List<Dictionary<string, string>>(); //Config.txt data

        static void Main(string[] args)
        {
            //Title of the console window
            Console.Title = "Simple Twitch Chat Bot " + version;

            // Turn off Quick Edit Mode
            DisableConsoleQuickEdit.SetQuickEdit(true);

            // SecurityProtocol SSL/TSL
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //Load in information from external Config.txt source.
            LoadLocalization();

            try
            {
                if (data[0]["Host"] == string.Empty || data[0]["BotName"] == string.Empty || data[0]["BotName"] == string.Empty || data[0]["oAuthPassword"] == string.Empty || data[0]["ChannelName"] == string.Empty || data[0]["OnlineMode"] == string.Empty || data[0]["OnlineURL"] == string.Empty || data[0]["DEBUG"] == string.Empty)
                {
                    Console.WriteLine("Config.txt is not configured correctly. Message: Empty config entry found.");
                    Thread.Sleep(8000);
                    Environment.Exit(0);
                }

                if (data[0]["DEBUG"].ToUpper() == "TRUE")
                    debug = true;
                else
                    debug = false;

                // We create the chatbot object
                twitchChatBot = new TwitchChatBot(data[0]["Host"], Int32.Parse(data[0]["Port"]), data[0]["BotName"], data[0]["oAuthPassword"], data[0]["ChannelName"], data[0]["OnlineMode"], data[0]["OnlineURL"], true);
                Console.WriteLine("Client commands? Write \"help\" or \"commands\"");
            }
            catch (KeyNotFoundException e)
            {
                //This is the result of the external data being wrong.
                Console.WriteLine("Config.txt is not configured correctly. Message: " + e);
                Thread.Sleep(8000);
                Environment.Exit(0);
            }

            // This will keep the console window open and also be used as a command prompt.
            while (true)
            {
                switch (Console.ReadLine().ToLower()) // Catch input and convert to lowercase.
                {
                    case "help":
                    case "commands":
                        Console.Clear();
                        Console.WriteLine("\"help\", \"commands\" will show this console window.");
                        Console.WriteLine("\"reset\" will reset the counter and clear memory.");
                        Console.WriteLine("\"reconnect\" will reconnect the bot but keep data.");
                        Console.WriteLine("\"stats\", \"status\", \"count\" will print out data from the current session.");
                        Console.WriteLine("\"clear\" will clear the console.");
                        Console.WriteLine("\"upload\" will upload the statistics data to the server.(If online-mode is set \"TRUE\")");
                        Console.WriteLine("\"quit\", \"exit\" will shutdown the bot.");
                        break;
                    case "reconnect": // Reconnect and save data
                        HungryData hungry = ObjectClone<HungryData>(twitchChatBot.hungry);
                        twitchChatBot.KillThreads();
                        twitchChatBot = new TwitchChatBot(data[0]["Host"], Int32.Parse(data[0]["Port"]), data[0]["BotName"], data[0]["oAuthPassword"], data[0]["ChannelName"], data[0]["OnlineMode"], data[0]["OnlineURL"], true);
                        twitchChatBot.hungry = hungry;
                        break;
                    case "reset":
                        twitchChatBot.hungry = new HungryData();
                        Console.Clear();
                        Console.WriteLine("Program reset! Online stats are refreshed upon next !hungry command.");
                        break;
                    case "stats":
                    case "status":
                    case "count":
                        Console.Clear();
                        Console.WriteLine("The word \"hungry\" has been said " + twitchChatBot.hungry.timesHungry.ToString() + " time(s). Total count of !hungry is " + twitchChatBot.hungry.timesHungryTotal.ToString());
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    case "upload":
                        Console.Clear();
                        if (twitchChatBot.connectionData.onlineMode)
                            twitchChatBot.SendHungry();
                        break;
                    case "quit":
                    case "exit":
                        Environment.Exit(0);
                        break;
                }
            }
        }

        /// <summary>
        /// Takes an object and copies it rather than storing the reference data.
        /// </summary>
        /// <typeparam name="T">Object Type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Copied Object Type + Data</returns>
        static T ObjectClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// This will check if the Config.txt exists and start reading the file data if it does.
        /// </summary>
        static void LoadLocalization()
        {
            string filepath = GetDirectory() + "\\Config.txt";
            if (File.Exists(filepath))
                ResolveContent(File.ReadAllLines(filepath));
        }

        /// <summary>
        /// This will take the filecontent and resolve its data. It splits each string multiple times checking for syntax comparison. [DataName] = "DataValue".
        /// </summary>
        /// <param name="fileContent">String array of content data</param>
        static void ResolveContent(string[] fileContent)
        {
            Dictionary<string, string> language = new Dictionary<string, string>();
            string[] Loc_name = new string[3]; // Temp data from Config.txt
            string[] Loc_value = new string[3]; // Temp data from Config.txt

            for (int i = 0; i < fileContent.Length; i++)
            {
                Loc_name = fileContent[i].Split('[', ']');
                Loc_value = fileContent[i].Split('"', '"');

                if (Loc_name.Length > 1 && Loc_value.Length > 1)
                    language.Add(Loc_name[1], Loc_value[1]);

                if (i < fileContent.Length && i != fileContent.Length - 1)
                {
                    Loc_name = new string[3];
                    Loc_value = new string[3];
                }
                else if (i == fileContent.Length - 1)
                {
                    Loc_name = new string[0];
                    Loc_value = new string[0];
                }
            }

            data.Add(language);
        }

        /// <summary>
        /// Get the software base directory.
        /// </summary>
        /// <returns></returns>
        static string GetDirectory() => Path.GetFullPath(@AppDomain.CurrentDomain.BaseDirectory);
    }
}
