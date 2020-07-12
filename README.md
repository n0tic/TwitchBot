# TwitchBot
A simple twitchbot which was a requested "HungryBot" ~ A hungry counter with a few commands on both client and chat side.
Should not be too advanced to modify for your own needs.

The twitch features are running on 3 different threads. 
Main thread is covering command input, another is for reading chat and one additional for sending messages. 
Async operations are used with online functionality.

It reads the connection information from an external Config.txt source file.
Syntax is [VarName] = "VarValue".

NOTE: You must verify if the bot responds to anything in the chat once it has been started. If it doesn't, please verify settings and try again. There is no way to know if the bot was successful.

### Online Features
You can set the bot to use online-mode in the Config.txt file. If you do, you need a webserver and index.php file accessable.
OnlineMode does upload your stats when !hungry command is called from chat (Once every 30 seconds max, unless upload has been entered in bot client). 
Upload is done with Async operation and sends Json data to the URL specified. Data is saved as "[BotName]_[ChannelName].json" so they can be somewhat uniqueue.
If you use the same bot name and the same channel name as another bot it will overwrite the same save file. Keep in mind to make it uniqueue.


![](http://bytevaultstudio.se/ShareX/HungryBot_jWH9wpuXaL.png)

## A few commands on client side
```
"help" or "commands" - Shows a list of commands and a brief decription
"reset" - Reset the counter and clear memory.
"reconnect" - Kills the current connection and all active threads. Creates new connection but keeps hungry statistics.
"stats", "status", "count" - Shows data from the current session.
"clear" - Clears the console.
"upload" - Uploads statistics to website.
"quit" or "exit" - Application will shutdown.
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
1. (Optional) My recommendation is to create a new twitch account and name it something "witty" like "HungryBot". Witty, right?
2. Sign in to the account you want the bot to operate from and request an oAuth key from https://twitchapps.com/tmi/
3. Edit Config.txt with the correct data input and syntaxes.
4. (Optional) Upload index.php to a webserver and simply enter the URL to that specific file in Config.txt.

>[BotName] = "YourBotName_MakeItUnique"  
>[oAuthPassword] = "oauth:FullAuthKeyHere" //Key from: https://twitchapps.com/tmi/ (Sign-in into the correct account first)   
>[ChannelName] = "bert_ika" // The channel the bot should join  
>[OnlineMode] = "FALSE"   
>[OnlineURL] = "https://bytevaultstudio.se/projects/mix/TwitchBot/index.php"
>  
>[Host] = "irc.chat.twitch.tv" // IRC address | IP  
>[Port] = "6667" // Port
>
>[DEBUG] = "FALSE"

### Bugs
>I noticed that the bot was having problems connecting and reading some chats. I tried logging in to the bot account and it started working again.
>However, it did work on SOME chats... which is the wierd part.

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
