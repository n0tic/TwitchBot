# TwitchBot
A simple twitchbot which was a requested "HungryBot" ~ A hungry counter with a few commands on both client and chat side.
Very easy to follow and modify.

The twitch features are running on 3 different threads. Main thread is covering command input, another is for reading chat and one additional for sending messages. Async operations.

The Online features are uploading statistics to my website by creating a .json file with the statistic data from the channel name. PHP Preview is coming soon.

It reads the connection information from an external Config.txt source file.
Syntax is [VarName] = "VarValue".

![](http://bytevaultstudio.se/ShareX/HungryBot_jWH9wpuXaL.png)

## A few commands on client side
```
help or commands - Shows a list of commands and a brief decription
reset - Reset the counter and clear memory.
reconnect - Kills the current connection and all active threads. Creates new connection but keeps hungry statistics.
stats, status, count - Shows data from the current session.
clear - Clears the console.
upload - Uploads statistics to website.
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

### Bugs
>I noticed that the bot was having problems connecting and reading some chats. I tried logging in to the bot account and it started working again.

# Third Parties Licenses
### Newtonsoft Json
```
The MIT License (MIT)

Copyright (c) 2007 James Newton-King

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
```
