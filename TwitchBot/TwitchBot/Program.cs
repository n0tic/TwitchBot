using TwitchBot.Extra;
using TwitchBot.Object;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TwitchBot
{
    public class Program
    {
        public static TwitchChatBot twitchChatBot;

        static List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
        static string[] Loc_name = new string[3];
        static string[] Loc_value = new string[3];

        static void Main(string[] args)
        {
            Console.Title = "Simple Twitch Chat Bot";

            // Turn off Quick Edit Mode
            DisableConsoleQuickEdit.SetQuickEdit(true);

            //Load in information from external Config.txt source.
            LoadLocalization();

            try
            {
                // We create the chatbot object
                twitchChatBot = new TwitchChatBot(data[0]["Host"], Int32.Parse(data[0]["Port"]), data[0]["BotName"], data[0]["oAuthPassword"], data[0]["ChannelName"], true);
                Console.WriteLine("!help");
            }
            catch (KeyNotFoundException e)
            {
                //This is the result of the external data being wrong.
                Console.WriteLine("Config.txt is not configured correctly. Message: " + e);
                Thread.Sleep(8000);
                Environment.Exit(0);
            }

            // This will keep the console window open and also be used as a command prompt.
            while(true)
            {
                switch (Console.ReadLine().ToLower()) // Catch input and convert to lowercase.
                {
                    case "help":
                    case "commands":
                        Console.Clear();
                        Console.WriteLine("\"help\", \"commands\" will show this console window.");
                        Console.WriteLine("\"reset\" will reset the counter and clear memory.");
                        Console.WriteLine("\"reconnect\" will reconnect the bot but keep data.");
                        Console.WriteLine("\"quit\", \"exit\" will shutdown the bot.");
                        break;
                    case "reconnect": // Reconnect and save data
                        HungryData hungry = twitchChatBot.hungry;
                        twitchChatBot.KillThreads();
                        twitchChatBot = new TwitchChatBot(data[0]["Host"], Int32.Parse(data[0]["Port"]), data[0]["BotName"], data[0]["oAuthPassword"], data[0]["ChannelName"], true);
                        twitchChatBot.hungry = hungry;
                        break;
                    case "reset":
                        twitchChatBot.hungry = new HungryData();
                        Console.Clear();
                        Console.WriteLine("Program reset!");
                        break;
                    case "quit":
                    case "exit":
                        Environment.Exit(0);
                        break;
                }
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
