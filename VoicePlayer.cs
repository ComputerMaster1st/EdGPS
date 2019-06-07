using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using EdGps.Core;

namespace EdGps
{
    public class VoicePlayer
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);

        public async Task Play(VoiceType response) {
            await _lock.WaitAsync();

            var _wmp = new MediaPlayer();
            _wmp.Open(new Uri(Path.GetFullPath(Path.Combine(Directories.VoiceDir, $"{response.ToString()}.mp3"))));

            while (!_wmp.NaturalDuration.HasTimeSpan) await Task.Delay(50);

            _wmp.Play();

            await Task.Delay(_wmp.NaturalDuration.TimeSpan);
            _lock.Release();
        }
    }
}