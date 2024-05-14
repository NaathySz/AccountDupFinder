using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using Dapper;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AccountDupFinder
{
    public class AccountDupFinder : BasePlugin
    {
        public override string ModuleName => "Account Dup Finder";
        public override string ModuleVersion => "1.0.0";
        public override string ModuleAuthor => "Nathy";
        public override string ModuleDescription => "Help admins to find duplicate accounts";

        private PluginConfig _config;
        private static readonly HttpClient _httpClient = new HttpClient();

        public override void Load(bool hotReload)
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            string configFilePath = Path.Combine(ModuleDirectory, "Config.json");
            if (!File.Exists(configFilePath))
            {
                throw new FileNotFoundException("Config file not found.");
            }

            _config = PluginConfig.LoadFromJson(configFilePath);

            using (var connection = new MySqlConnection(GetConnectionString()))
            {
                connection.Open();

                string createTableQuery = "CREATE TABLE IF NOT EXISTS player_data (id INT AUTO_INCREMENT PRIMARY KEY, SteamID VARCHAR(255), PlayerName VARCHAR(255), IPAddress VARCHAR(255))";

                using (var command = new MySqlCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static async Task<string> GetProfilePictureAsync(string steamId64, string defaultImage)
        {
            try
            {
                string apiUrl = $"https://steamcommunity.com/profiles/{steamId64}/?xml=1";

                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string xmlResponse = await response.Content.ReadAsStringAsync();
                    int startIndex = xmlResponse.IndexOf("<avatarFull><![CDATA[") + "<avatarFull><![CDATA[".Length;
                    int endIndex = xmlResponse.IndexOf("]]></avatarFull>", startIndex);

                    if (endIndex >= 0)
                    {
                        string profilePictureUrl = xmlResponse.Substring(startIndex, endIndex - startIndex);
                        return profilePictureUrl;
                    }
                    else
                    {
                        return defaultImage;
                    }
                }
                else
                {
                    return null!;
                }
            }
            catch
            {
                return null!;
            }
        }

        private async Task SendDiscordWebhookNotification(string playerName, string currentAccountLink, string altAccountLink, string ipAddress, string steamId64)
        {
            var profilePictureUrl = await GetProfilePictureAsync(steamId64, "https://steamuserimages-a.akamaihd.net/ugc/885384897182110030/F095539864AC9E94AE5236E04C8CA7C2725BCEFF/");

            var embed = new
            {
                title = _config.EmbedTitle,
                color = _config.EmbedColor,
                fields = new[]
                {
                    new
                    {
                        name = _config.PlayerFieldName,
                        value = playerName,
                        inline = true
                    },
                    new
                    {
                        name = _config.IPAddressFieldName,
                        value = ipAddress,
                        inline = true
                    },
                    new
                    {
                        name = _config.CurrentAccountFieldName,
                        value = currentAccountLink,
                        inline = false
                    },
                    new
                    {
                        name = _config.AltAccountFieldName,
                        value = altAccountLink,
                        inline = true
                    }
                },
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                footer = new
                {
                    text = "AccountDupFinder, by Nathy"
                },
                thumbnail = new
                {
                    url = profilePictureUrl
                }
            };

            var payload = new { embeds = new[] { embed } };
            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            await _httpClient.PostAsync(_config.DiscordWebhookUrl, content);
        }


        [GameEventHandler]
        public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
        {
            if (@event.Userid != null && @event.Userid.IsValid)
            {
                string playerName = @event.Userid.PlayerName;
                string steamId = @event.Userid.SteamID.ToString();
                string ipAddress = @event.Userid.IpAddress?.Split(':')[0] ?? "Unknown";

                Task.Run(() => HandlePlayerConnectAsync(playerName, steamId, ipAddress));
            }

            return HookResult.Continue;
        }

        private async Task HandlePlayerConnectAsync(string playerName, string steamId, string ipAddress)
        {
            Console.WriteLine($"Player Connected: {playerName} (SteamID: {steamId}, IP: {ipAddress})");

            try
            {
                using (var connection = new MySqlConnection(GetConnectionString()))
                {
                    await connection.OpenAsync();

                    Console.WriteLine("Connection opened successfully.");

                    string query = "SELECT SteamID FROM player_data WHERE IPAddress = @IPAddress";
                    var existingSteamId = await connection.ExecuteScalarAsync<string>(query, new { IPAddress = ipAddress });

                    if (existingSteamId != null)
                    {
                        if (existingSteamId != steamId)
                        {
                            Console.WriteLine($"WARNING: Duplicate account detected! Player: {playerName}, Existing SteamID: {existingSteamId}, Connected SteamID: {steamId}");
                            await SendDiscordWebhookNotification(playerName, $"https://steamcommunity.com/profiles/{steamId}", $"https://steamcommunity.com/profiles/{existingSteamId}", ipAddress, steamId);
                        }
                    }
                    else
                    {
                        string insertQuery = "INSERT INTO player_data (SteamID, PlayerName, IPAddress) VALUES (@SteamID, @PlayerName, @IPAddress)";
                        await connection.ExecuteAsync(insertQuery, new { SteamID = steamId, PlayerName = playerName, IPAddress = ipAddress });

                        Console.WriteLine("Data inserted successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private string GetConnectionString()
        {
            return $"Server={_config.DatabaseHost};Port={_config.DatabasePort};Database={_config.DatabaseName};Uid={_config.DatabaseUser};Pwd={_config.DatabasePassword};";
        }
    }

    public class PluginConfig
    {
        public string DatabaseHost { get; set; }
        public string DatabasePort { get; set; }
        public string DatabaseUser { get; set; }
        public string DatabasePassword { get; set; }
        public string DatabaseName { get; set; }
        public string DiscordWebhookUrl { get; set; }

        public string EmbedTitle { get; set; } = "Duplicate Account Detected - Smurf Alert!";
        public int EmbedColor { get; set; } = 16711680;
        public string PlayerFieldName { get; set; } = "Player";
        public string IPAddressFieldName { get; set; } = "IP Address";
        public string CurrentAccountFieldName { get; set; } = "Current Account";
        public string AltAccountFieldName { get; set; } = "Alt Account";

        public static PluginConfig LoadFromJson(string filePath)
        {
            string jsonData = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<PluginConfig>(jsonData);
        }
    }

}
