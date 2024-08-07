# AccountDupFinder
This plugin helps administrators identify duplicate accounts by monitoring player connections and sending notifications via Discord webhook.

## Config
The config is located at the same place that dll is. 

```json
{
  "DatabaseHost": "localhost",
  "DatabasePort": "3306",
  "DatabaseUser": "your_username",
  "DatabasePassword": "your_password",
  "DatabaseName": "your_database_name",
  "DatabaseTableName": "player_data",
  "DiscordWebhookUrl": "https://discord.com/api/webhooks/xxxxxxxxxxxxxxx/xxxxxxxxxxxxxxxxx",
  "EmbedTitle": "Duplicate Account Detected!",
  "EmbedColor": 16711680,
  "PlayerFieldName": "Player Name",
  "IPAddressFieldName": "IP Address",
  "CurrentAccountFieldName": "Current Account",
  "AltAccountFieldName": "Alternate Account",
  "VpnEmbedTitle": "VPN Suspect!",
  "VpnEmbedColor": 16776960,
  "VpnAccountFieldName": "Account URL",
  "VpnNoteFieldName": "Warning",
  "VpnNoteFieldValue": "This player is suspected of using a VPN!"
  "VPNEnabled": true,
  "VpnNotificationMessage": "{blue}[AccountDupFinder] {white}Suspected VPN activity by player {red}{playerName} {white}(SteamID: {red}{steamId}{white}, IP: {red}{ipAddress})",
  "DuplicateAccountNotificationMessage": "{blue}[AccountDupFinder] {white}Player {red}{playerName} {white}(SteamID: {red}{steamId}{white}, IP: {red}{ipAddress}{white}) has connected with a duplicate account! Existing SteamID: {red}{DupSteamId}",
  "FlagToNotify": "@css/generic",
  "CacheTimeInMinutes": 5,
  "WhitelistIPs": [
    "192.168.0.1",
    "192.168.1.1"
    ],
  "WhitelistSteamIDs": [
    "76561198939064419"
  ]
}
```

## Preview
Discord messages:

![image](https://github.com/NaathySz/AccountDupFinder/assets/97997774/bd451b27-d6d3-49c8-9a96-a11849502b75)

![image](https://github.com/NaathySz/AccountDupFinder/assets/97997774/886bd9e5-4f72-4742-b1bc-8d36cf8f95f8)

In-game messages:

![image](https://github.com/NaathySz/AccountDupFinder/assets/97997774/ae56c736-ff17-4b42-9ab7-c07030c54869)

![image](https://github.com/NaathySz/AccountDupFinder/assets/97997774/13984cfd-dbea-4ca9-b40f-7de189b34250)

## Available colors
```
{default}
{white}
{darkred}
{green}
{lightyellow}
{lightblue}
{olive}
{lime}
{red}
{lightpurple}
{purple}
{grey}
{yellow}
{gold}
{silver}
{blue}
{darkblue}
{bluegrey}
{magenta}
{lightred}
{orange}
```

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/L4L611665R)
