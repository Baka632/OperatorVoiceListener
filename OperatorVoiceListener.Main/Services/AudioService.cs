using System.Collections.Immutable;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;

namespace OperatorVoiceListener.Main.Services
{
    public sealed class AudioService
    {
        public MediaPlayer Player { get; }
        private readonly ImmutableDictionary<string, string> OpCodenameToNameMapping;
        private readonly ImmutableDictionary<string, OperatorVoiceInfo[]> OpCodenameToVoiceMapping;

        public AudioService()
        {
            Player = new MediaPlayer
            {
                AudioCategory = MediaPlayerAudioCategory.Media
            };
            Player.MediaEnded += OnPlayerMediaEnded;
            OperatorTextResourceHelper textResourceHelper = new(ArknightsResources.Operators.TextResources.Properties.Resources.ResourceManager);

            OpCodenameToNameMapping = textResourceHelper.GetOperatorCodenameMapping(AvailableCultureInfos.ChineseSimplifiedCultureInfo);
            OpCodenameToVoiceMapping = textResourceHelper.GetAllOperatorVoiceInfos(AvailableCultureInfos.ChineseSimplifiedCultureInfo);
        }

        private void OnPlayerMediaEnded(MediaPlayer sender, object args)
        {
            sender.Pause();
            sender.PlaybackSession.Position = TimeSpan.Zero;
        }

        public void PlayOperatorVoice(byte[] voice, OperatorVoiceItem voiceItem)
        {
            Player.Pause();
            MediaPlaybackSession playbackSession = Player.PlaybackSession;
            playbackSession.Position = TimeSpan.Zero;

            MediaSource source = MediaSource.CreateFromStream(new MemoryStream(voice).AsRandomAccessStream(), "audio/ogg");
            MediaPlaybackItem media = new(source);

            string lang = voiceItem.VoiceType switch
            {
                OperatorVoiceType.ChineseMandarin => "中文-普通话",
                OperatorVoiceType.ChineseRegional => "中文-方言",
                OperatorVoiceType.Japanese => "日语",
                OperatorVoiceType.English => "英语",
                OperatorVoiceType.Korean => "韩语",
                OperatorVoiceType.Italian => "意大利语",
                _ => string.Empty,
            };
            MediaItemDisplayProperties props = media.GetDisplayProperties();
            props.Type = Windows.Media.MediaPlaybackType.Video;
            string codename = voiceItem.CharactorCodename.Split('_')[0];
            
            if (OpCodenameToVoiceMapping.TryGetValue(codename, out var voiceInfos))
            {
                OperatorVoiceInfo voiceInfo = voiceInfos.Where(info => info.Type == voiceItem.VoiceType).First();
                props.MusicProperties.Artist = voiceInfo.CV;

                OperatorVoiceItem voiceItemContainsFullData = voiceInfo.Voices.Where(voice => voice.VoiceId == voiceItem.VoiceId).First();
                props.VideoProperties.Subtitle = voiceItemContainsFullData.VoiceText;
                props.MusicProperties.Title = props.VideoProperties.Title = OpCodenameToNameMapping.TryGetValue(codename, out string? opName)
                ? $"{opName} - {voiceItemContainsFullData.VoiceTitle} [{lang}]"
                : $"{voiceItem.CharactorCodename} - {voiceItemContainsFullData.VoiceTitle} [{lang}]";
            }
            else
            {
                props.MusicProperties.Title = OpCodenameToNameMapping.TryGetValue(codename, out string? opName)
                    ? $"{opName} [{lang}]"
                    : $"{voiceItem.CharactorCodename} [{lang}]";
            }
            media.ApplyDisplayProperties(props);

            Player.Source = media;
            Player.Play();
        }
    }
}
