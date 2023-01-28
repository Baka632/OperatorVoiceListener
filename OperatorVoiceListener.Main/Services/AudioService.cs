using Windows.Foundation;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;

namespace OperatorVoiceListener.Main.Services
{
    public sealed class AudioService
    {
        public MediaPlayer Player { get; }

        public AudioService()
        {
            Player = new MediaPlayer
            {
                AudioCategory = MediaPlayerAudioCategory.Media
            };
            Player.MediaEnded += OnPlayerMediaEnded;
        }

        private void OnPlayerMediaEnded(MediaPlayer sender, object args)
        {
            sender.Pause();
            sender.PlaybackSession.Position = TimeSpan.Zero;
        }

        public async Task PlayOperatorVoice(byte[] voice, string title, string subtitle, string cv)
        {
            Player.Pause();
            MediaPlaybackSession playbackSession = Player.PlaybackSession;
            playbackSession.Position = TimeSpan.Zero;

            InMemoryRandomAccessStream stream = new();
            IBuffer buffer = WindowsRuntimeBuffer.Create(voice, 0, voice.Length, voice.Length);
            await stream.WriteAsync(buffer);

            MediaSource source = MediaSource.CreateFromStream(stream, "audio/ogg");
            MediaPlaybackItem media = new(source);

            MediaItemDisplayProperties props = media.GetDisplayProperties();
            props.Type = Windows.Media.MediaPlaybackType.Video;
            props.MusicProperties.Artist = cv;
            props.VideoProperties.Subtitle = subtitle;
            props.MusicProperties.Title = title;
            props.VideoProperties.Title = title;
            media.ApplyDisplayProperties(props);

            Player.Source = media;
            Player.Play();
        }
    }
}
