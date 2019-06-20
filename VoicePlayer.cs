using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EdGps.Core;
using NAudio.Wave;

namespace EdGps
{
    public class VoicePlayer
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);
        private readonly Dictionary<VoiceType, TimeSpan> _mp3Times = new Dictionary<VoiceType, TimeSpan>();

        public async Task Play(VoiceType response) {
            await _lock.WaitAsync();

            var localFilePath = Path.Combine(Directories.VoiceDir, $"{response.ToString()}.mp3");

            if (!_mp3Times.ContainsKey(response))
            {
                using (var mp3File = new Mp3FileReader(Path.Combine(Directories.VoiceDir, $"{response.ToString()}.mp3")))
                {
                   _mp3Times.Add(response, mp3File.TotalTime);
                }
                
            }

            WaveStream mainOutputStream = new Mp3FileReader(Path.GetFullPath(localFilePath)); //Loading file to WaveStream
            WaveChannel32 volumeStream = new WaveChannel32(mainOutputStream); //Creating WaveChannel with default settings, set volume later
            WaveOutEvent waveOut = new WaveOutEvent //NOTE: using WaveOutEvent and NOT WaveOut - WaveOut isn't compatible with console apps
            {
                DeviceNumber = -1, //Device -1 is the system default device
                Volume = 1.0f //Volume could be set through config.json
            };
            waveOut.Init(volumeStream);
            waveOut.Play();

            await Task.Delay(_mp3Times[response]);
            _lock.Release();
        }
    }
}