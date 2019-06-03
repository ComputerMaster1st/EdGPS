using System;
using System.IO;
using System.Windows.Media;
using EdGps.Core;

namespace EdGps
{
    public static class VoicePlayer
    {
        private static readonly MediaPlayer _wmp = new MediaPlayer();

        public static void Play(VoiceType response) {
            _wmp.Open(new Uri(Path.GetFullPath(Path.Combine(Directories.VoiceDir, $"{response.ToString()}.mp3"))));
            _wmp.Play();
        }
    }
}