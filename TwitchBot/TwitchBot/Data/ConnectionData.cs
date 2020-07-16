using System;

namespace TwitchBot
{
    public struct ConnectionData
    {
        public DateTime timeOutTimer;
        public bool connectedToChat;

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
