# TwitchBot
A simple twitchbot which was a "requested" "HungryBot" ~ A hungry counter with a few commands on both client and chat side.
Very easy to follow and modify.

It reads the connection information from an external Config.txt source file.
Syntax is [VarName] = "VarValue".

The twitch features are running on 3 different threads. Main thread is covering command input, another is for reading chat and one additional for sending messages.

---------------------------------------------------------------------------------------------------------------------------------------
# Setup steps:
1. (Optional)My recommendation is to create a new twitch account and name it something "witty" like "HungryBot". Witty, right?
2. Sign in to the account you want the bot to operate from and request an oAuth key from https://twitchapps.com/tmi/
3. Edit Config.txt with the correct data input and syntaxes:

>[BotName] = "HungryBot"  
>[oAuthPassword] = "" //Key from: https://twitchapps.com/tmi/ (Make sure you are signed into the correct account.)
>[ChannelName] = "" // The channel the bot should join  
>  
>[Host] = "irc.chat.twitch.tv" // IRC address | IP  
>[Port] = "6667" // Port
