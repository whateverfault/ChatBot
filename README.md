# Twitch Chat bot

1. [Build](#Build)
2. [Setting Everything up](#Setting-Everything-Up)
   - [Getting bot's access token](#Getting-access-token)
   - [Services](#Services)
     - [Core](#Core)
       - [Chat Commands](#Chat-Commands)
       - [Chat Logs](#Chat-Logs)
       - [Message Filter](#Message-Filter)
       - [Moderation](#Moderation)
     - [Useful](#Useful)
       - [Ai](#Ai)
       - [Translator](#Translator)
       - [Demon List](#Demon-List)
       - [Level Requests](#Level-Requests)
       - [Game Requests](#Game-Requests)
       - [Notifications](#Notifications)
     - [Fun](#Fun)
       - [Message Randomizer](#Message-Randomizer)
       - [Text Generator](#Text-Generator)
       - [Points](#Points)
          - [Bank](#Bank)
          - [Shop](#Shop)
          - [Casino](#Casino)
      - [Debug](#Debug)
         - [Stream State Checker](#Stream-State-Checker)
         - [Logger](#Logger)

## Build

```console
$ dotnet run ./src/Program.cs
```

## Setting Everything Up
Once you've started the program you're going to see some options to choose, enter one's number to make a choice.

### Getting access token
First of all you need to sign up the bot's account on Twitch. Then follow the [link](https://twitchtokengenerator.com/) and grab the access token(don't forget to set the required permissions). Once you've got the bot's access token paste it into Bot -> Credentials -> Secret -> OAuth. <br><br>
Some functionality might be broken without Broadcasters OAuth(access token) like !title and !game commands to change title and category respectively. To get Broadcasters OAuth go to [link](https://twitchtokengenerator.com/) but instead of bot's log into the broadcaster's account.

### Services
For any service to work you should switch it's State to Enabled.

- ### Core
  - #### Chat Commands
    Here you can add new chat commands and chat ads and also change some of their info like name, description, aliases, state, etc.
  - #### Chat Logs
    Stores the entire chat history except the bot's and filtrated messages.
  - #### Message Filter
    Here you can add new filters or change the existing ones. Needed for Moderation Service.
  - #### Moderation
    A service to choose punishments for the set filters in Message Filter Service. Level Requests pattern should always be turned off unless you know what you are doing.
- ### Useful
  - #### Ai
    Firstly, decide which provider you want to use and then choose an AI Mode accordingly. <br>
    To set up the desired AI provider follow the instructions:
    - [Ollama](https://docs.ollama.com/quickstart)
    - [HuggingFace](https://www.youtube.com/watch?v=uBSbgQ1qPHI)
    - [Vertex AI](https://docs.cloud.google.com/vertex-ai/generative-ai/docs/start/quickstart)
    - [DeepSeek](https://rahulranjan.org/2025/02/01/how-to-get-your-deepseek-api-key-testing-and-troubleshooting/)
    <br>
    "Remove Chat In" option removes the started conversation from memory once certain amount of seconds have passed.
  - #### Translator
    To set up the desired translation service follow the instructions:
    - [Google](https://cloud.google.com/translate/docs/overview)
    - [Vk](https://dev.vk.com/ru/api/access-token/getting-started)
  - #### Demon List
    Provides such commands as !place, !top, !pplace, !ptop, etc.
  - #### Level Requests
    Basically, you don't want to touch any of given options but Permissions. <br>
    Requests State is manipulated via !req command and Reward Id is set via !set-req-reward command (should be used within a reward redeem).
  - #### Game Requests
    Used to handle the queue of game requests. Games are added either via !add-game command or set reward.
  - #### Notifications
    Sends telegram notifications on stream started. <br>
    [Full Guide](https://www.youtube.com/watch?v=l5YDtSLGhqk) to obtain the Chat Id and Bot's Token. <br>
    Cooldown is given in seconds. For new line in Notification Prompt use \n.
- ### Fun
  - #### Message Randomizer
    Sends a random message from logs into the chat once counter hits the given max or random value or !carrot command is used.
  - #### Text Generator
    A Markov chain trained on the chat logs.
  - #### Points
    - #### Bank
      Stores the points.
    - #### Shop
      By default, has Ai and Mute lots. New slots can be added via !add-lot command.
    - #### Casino
      An alternative way to spend the points by gambling.
  - ### Debug
    - #### Stream State Checker
      Allows to change the Check Cooldown and shows some debug information.
    - #### Logger
      Logs whatever internally happened.
