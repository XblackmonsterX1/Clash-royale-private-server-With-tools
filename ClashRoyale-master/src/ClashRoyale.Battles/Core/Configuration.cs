﻿using System;
using System.IO;
using Newtonsoft.Json;

namespace ClashRoyale.Battles.Core
{
    public class Configuration
    {
        [JsonIgnore] public static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ObjectCreationHandling = ObjectCreationHandling.Reuse,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.None
        };

        [JsonProperty("battle_nonce")] public string BattleNonce = "nonce";

        // Make sure to edit these on prod
        [JsonProperty("cluster_encryption_key")]
        public string ClusterKey = "15uvmi8qnyuj9tm53ipaavvytltm582yatecyjzb";

        [JsonProperty("cluster_encryption_nonce")]
        public string ClusterNonce = "nonce";

        [JsonProperty("max_sessions")] public int MaxSessions = 100;
        [JsonProperty("sentry_api")] public string SentryApiUrl = "";
        [JsonProperty("server_port")] public int ServerPort = 9449;

        public void Initialize()
        {
            if (File.Exists("config.json"))
                try
                {
                    var config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("config.json"));

                    SentryApiUrl = config.SentryApiUrl;
                    BattleNonce = config.BattleNonce;
                    ServerPort = config.ServerPort;
                    MaxSessions = config.MaxSessions;
                    ClusterKey = config.ClusterKey;
                    ClusterNonce = config.ClusterNonce;
                }
                catch (Exception)
                {
                    Console.WriteLine("Couldn't load configuration.");
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }
            else
                try
                {
                    Save();

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Server configuration has been created. Restart the server now.");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                catch (Exception)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Couldn't create config file.");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
        }

        public void Save()
        {
            File.WriteAllText("config.json", JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}