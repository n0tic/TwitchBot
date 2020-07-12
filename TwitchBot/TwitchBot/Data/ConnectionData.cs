namespace TwitchBot.Data
{
    public struct ConnectionData
    {
        // Online
        public bool onlineMode;
        public string apiURL;

        // TCP Connection
        public string host;
        public int port;

        //Bot Information
        public string botName;
        public string password;
        public string channelName;
    }
}
