# AccountDupFinder
This plugin helps administrators identify duplicate accounts by monitoring player connections and sending notifications via Discord webhook.

## Config
The config is located at the same place that dll is. 

```
{
  "DatabaseHost": "localhost",
  "DatabasePort": "3306",
  "DatabaseUser": "your_username",
  "DatabasePassword": "your_password",
  "DatabaseName": "your_database_name",
  "DiscordWebhookUrl": "https://discord.com/api/webhooks/xxxxxxxxxxxxxxx/xxxxxxxxxxxxxxxxx",
  "EmbedTitle": "Duplicate Account Detected!",
  "EmbedColor": 16711680,
  "PlayerFieldName": "Player Name",
  "IPAddressFieldName": "IP Address",
  "CurrentAccountFieldName": "Current Account",
  "AltAccountFieldName": "Alternate Account"
}
```

## Preview
![image](https://github.com/NaathySz/AccountDupFinder/assets/97997774/b4e9a0e5-9e56-4c2f-9938-de9a66a7d739)
