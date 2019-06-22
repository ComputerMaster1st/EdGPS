using System.IO;
using Newtonsoft.Json;

namespace EdGps.Core
{
    [JsonObject(MemberSerialization.Fields)]
    public class Config
    {
        [JsonIgnore]
        private const string _configName = "config.json";

        public string JournalPath { get; private set; } = string.Empty;
        public bool VoiceEnabled { get; private set; } = true;
        public int SoundVolume { get; private set; } = 50;

        [JsonConstructor]
        private Config(string journalPath, bool enableVoice) {
            JournalPath = journalPath;
            VoiceEnabled = enableVoice;
        }

        private Config() {}

        public static Config LoadOrCreate() {
            if (File.Exists(_configName)) return JsonConvert.DeserializeObject<Config>(File.ReadAllText(_configName));
            return new Config();
        }

        private void Save()
            => File.WriteAllText(_configName, JsonConvert.SerializeObject(this, Formatting.Indented));
        
        public void SetJournalPath(string path) {
            JournalPath = path;
            Save();
        }

        public void EnableVoice(bool enable = true) {
            VoiceEnabled = enable;
            Save();
        }

        public void SetVolume(int vol = 50)
        {
            SoundVolume = vol / 100;
            Save();
        }
    }
}