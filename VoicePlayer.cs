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

        public async Task Play(VoiceType response, int volume) {
            await _lock.WaitAsync();

            var localFilePath = Path.Combine(Directories.VoiceDir, $"{response.ToString()}.mp3");

            if (!_mp3Times.ContainsKey(response))
            {
                using (var mp3File = new Mp3FileReader(Path.Combine(Directories.VoiceDir, $"{response.ToString()}.mp3")))
                {
                   _mp3Times.Add(response, mp3File.TotalTime);
                }
                
            }
            
            using (var mainOutputStream = new Mp3FileReader(Path.GetFullPath(localFilePath)))
            using (var volumeStream = new WaveChannel32(mainOutputStream))
            using (var waveOut = new WaveOutEvent { DeviceNumber = -1, Volume = (float)volume / 100 })
            {
                waveOut.Init(volumeStream);
                waveOut.Play();

                await Task.Delay(_mp3Times[response]);
            }

            _lock.Release();
        }
    }
}