# TwitchBot
A simple twitchbot which was a "requested" "HungryBot" ~ A hungry counter with a few commands on both client and chat side.
Very easy to follow and modify.

It reads the connection information from an external Config.txt source file.
Syntax is [VarName] = "VarValue".

The twitch features are running on 3 different threads. Main thread is covering command input, another is for reading chat and one additional for sending messages.

![](http://bytevaultstudio.se/ShareX/HungryBot_o6fQtuDLs4.png)

## A few commands on client side
```
help or commands - Shows a list of commands and a brief decription
reconnect - Kills the current connection and all active threads. Creates new connection but keeps hungry statistics.
reset - Resets the hungry statistics
stats, status, count - Shows data from the current session.
quit or exit - Application will shutdown.
```
## A few commands on twitch chat
```
!hungry - This command will add +1 to the counter IF enough time has passed (30 seconds since last time)
!howhungry - This command will print out how many times the word hungry has been counted.
!hungrycount - This command will print out how many times !hungry has been said in the chat regardless of the timer.
!help - Will print out the commands and a brief description.
```
---------------------------------------------------------------------------------------------------------------------------------------
# Setup steps:
1. (Optional)My recommendation is to create a new twitch account and name it something "witty" like "HungryBot". Witty, right?
2. Sign in to the account you want the bot to operate from and request an oAuth key from https://twitchapps.com/tmi/
3. Edit Config.txt with the correct data input and syntaxes:

>[BotName] = "HungryBot"  
>[oAuthPassword] = "" //Key from: https://twitchapps.com/tmi/ (Sign-in into the correct account first)   
>[ChannelName] = "" // The channel the bot should join  
>  
>[Host] = "irc.chat.twitch.tv" // IRC address | IP  
>[Port] = "6667" // Port

### Twitch updates
Twitch had an update recently which affects the chat. I noticed the bot was unable or was refused to use the chat of offline channels. This may be a permanent patch from twitch or a temporary one. In any case, this is important to note.
