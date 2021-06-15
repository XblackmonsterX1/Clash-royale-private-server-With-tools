﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClashRoyale.Extensions.Utils;
using ClashRoyale.Utilities.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpRaven.Data;

namespace ClashRoyale.Files
{
    public class Fingerprint
    {
        public const string Path = "GameAssets/fingerprint.json";

        public Fingerprint()
        {
            try
            {
                if (File.Exists(Path))
                {
                    Json = File.ReadAllText(Path);
                    Files = new List<Asset>();

                    var json = JObject.Parse(Json);
                    {
                        Sha = json["sha"].ToObject<string>();
                        Version = json["version"].ToObject<string>().Split('.').Select(int.Parse).ToArray();

                        foreach (var file in json["files"]) Files.Add(file.ToObject<Asset>());

                        Logger.Log($"Fingerprint [v{GetVersion}] loaded.",
                            GetType());
                    }
                }
                else
                {
                    Console.WriteLine("The Fingerprint cannot be loaded, the file does not exist.");
                    Program.Exit();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load the Fingerprint.");
                Program.Exit();
            }
        }

        [JsonIgnore] public string Json { get; set; }
        [JsonIgnore] public int[] Version { get; set; }

        [JsonIgnore] public int GetMajorVersion => Version?[0] ?? 3;
        [JsonIgnore] public int GetBuildVersion => Version?[1] ?? 377;
        [JsonIgnore] public int GetContentVersion => Version?[2] ?? 1;

        [JsonProperty("files")] public List<Asset> Files { get; set; }
        [JsonProperty("sha")] public string Sha { get; set; }

        [JsonProperty("version")]
        public string GetVersion => $"{GetMajorVersion}.{GetBuildVersion}.{GetContentVersion}";

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.None
            });

            Json = json.Replace("/", "\\/").TrimEnd(); // Somehow cr hates correct paths

            File.WriteAllText(Path, Json);
        }
    }

    public class Asset
    {
        [JsonProperty("defer")] public bool Defer { get; set; }
        [JsonProperty("file")] public string File { get; set; }
        [JsonProperty("sha")] public string Sha { get; set; }

        public async Task<bool> HasFileChanged()
        {
            var path = Path.Combine(UpdateManager.BaseDir, File);
            if (!System.IO.File.Exists(path)) return false;

            var expression = Path.GetExtension(File).Replace(".", string.Empty);

            switch (expression)
            {
                case "csv":
                {
                    var rawData = await System.IO.File.ReadAllBytesAsync(path);
                    var compressedData = CompressionUtils.CompressData(rawData);
                    var sha = ServerUtils.GetChecksum(compressedData);

                    return sha != Sha;
                }

                case "sc":
                {
                    var compressedData = await System.IO.File.ReadAllBytesAsync(path);
                    var sha = ServerUtils.GetChecksum(compressedData);

                    return sha != Sha;
                }

                default:
                {
                    Logger.Log($"Unknown file expression {expression}", GetType(), ErrorLevel.Warning);
                    break;
                }
            }

            return false;
        }
    }
}