using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using EdGps.Core;

namespace EdGps
{
    public static class VoicePlayer
    {
        private static SemaphoreSlim _lock = new SemaphoreSlim(1);

        public static async Task Play(VoiceType response) {
            await _lock.WaitAsync();

            var _wmp = new MediaPlayer();
            _wmp.Open(new Uri(Path.GetFullPath(Path.Combine(Directories.VoiceDir, $"{response.ToString()}.mp3"))));
            _wmp.Volume = 150;

            var duration = _wmp.NaturalDuration.HasTimeSpan ? _wmp.NaturalDuration.TimeSpan : TimeSpan.Zero;

            _wmp.Play();
            
            await Task.Delay((int)duration.TotalMilliseconds);
            _lock.Release();
        }
    }
}