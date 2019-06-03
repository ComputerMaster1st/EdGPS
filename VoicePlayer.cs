using EdGps.Core;
using NAudio.Wave;

namespace EdGps
{
    public static class VoicePlayer
    {
        public static void Play(VoiceType response) {
            using (var reader = new Mp3FileReader($"{Directories.VoiceDir}/{response.ToString()}.mp3")) 
            using (var waveOut = new WaveOutEvent()) {
                waveOut.Init(reader);
                waveOut.Play();
            }
        }
    }
}