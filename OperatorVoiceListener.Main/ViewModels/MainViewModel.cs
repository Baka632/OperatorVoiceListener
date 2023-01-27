using OperatorVoiceListener.Main.Services;
using CustomVoiceRes = ArknightsResources.Operators.VoiceResources.Custom.Properties.Resources;
using CNVoiceRes = ArknightsResources.Operators.VoiceResources.CN.Properties.Resources;
using ENVoiceRes = ArknightsResources.Operators.VoiceResources.EN.Properties.Resources;
using JPVoiceRes = ArknightsResources.Operators.VoiceResources.JP.Properties.Resources;
using KRVoiceRes = ArknightsResources.Operators.VoiceResources.KR.Properties.Resources;
using Windows.Media.Playback;
using System.Diagnostics;
using Microsoft.UI.Dispatching;

namespace OperatorVoiceListener.Main.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string operatorCodename = string.Empty;
        [ObservableProperty]
        private string voiceID = string.Empty;
        [ObservableProperty]
        private OperatorVoiceType voiceType;
        [ObservableProperty]
        private int voiceTypeIndex;

        [ObservableProperty]
        private string infoBarMessage = string.Empty;
        [ObservableProperty]
        private string infoBarTitle = string.Empty;
        [ObservableProperty]
        private bool infoBarOpen;
        [ObservableProperty]
        private InfoBarSeverity infoBarSeverity;

        [ObservableProperty]
        private bool isLoadingAudio;

        private DispatcherQueue MainDispatcherQueue;

        internal OperatorVoiceType[] OperatorVoiceTypes = new OperatorVoiceType[]
        {
            OperatorVoiceType.ChineseMandarin,
            OperatorVoiceType.ChineseRegional,
            OperatorVoiceType.Japanese,
            OperatorVoiceType.English,
            OperatorVoiceType.Korean,
            OperatorVoiceType.Italian,
        };

        public AudioService AudioService { get; }

        public MainViewModel(DispatcherQueue queue)
        {
            AudioService = new AudioService();
            AudioService.Player.MediaFailed += OnMediaPlayFailed;
            MainDispatcherQueue = queue;
        }

        private void OnMediaPlayFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
#if DEBUG
            Debug.WriteLine("When will WinUI 3 fix this bug?\nSee:https://github.com/microsoft/WindowsAppSDK/issues/3305");
#endif
            MainDispatcherQueue.TryEnqueue(() => SetInfoBar(true,
                "注意",
                "由于WinUI 3的一个bug，音频解码出错，请重新开始播放并将播放进度调节到自己需要的位置。",
                InfoBarSeverity.Error));
        }

        partial void OnVoiceTypeIndexChanged(int value)
        {
            VoiceType = OperatorVoiceTypes.ElementAtOrDefault(value);
        }

        public async Task StartVoicePlay()
        {
            ResetInfoBar();
            IsLoadingAudio = true;
            OperatorVoiceItem voiceItem = new(OperatorCodename, VoiceID, string.Empty, string.Empty, VoiceType);

            OperatorVoiceResourceHelper resourceHelper = VoiceType switch
            {
                OperatorVoiceType.ChineseRegional or OperatorVoiceType.Italian => new OperatorVoiceResourceHelper(CustomVoiceRes.ResourceManager),
                OperatorVoiceType.English => new OperatorVoiceResourceHelper(ENVoiceRes.ResourceManager),
                OperatorVoiceType.ChineseMandarin => new OperatorVoiceResourceHelper(CNVoiceRes.ResourceManager),
                OperatorVoiceType.Japanese => new OperatorVoiceResourceHelper(JPVoiceRes.ResourceManager),
                _ => new OperatorVoiceResourceHelper(KRVoiceRes.ResourceManager),
            };

            try
            {
                byte[] voice = await resourceHelper.GetOperatorVoiceAsync(voiceItem);
                AudioService.PlayOperatorVoice(voice, voiceItem);
            }
            catch
            {
                SetInfoBar(true, "无法找到语音文件", "请检查你输入的语音ID是否正确", InfoBarSeverity.Error);
            }
            IsLoadingAudio = false;
        }

        private void SetInfoBar(bool isOpen, string title, string message, InfoBarSeverity severity)
        {
            InfoBarOpen = isOpen;
            InfoBarTitle = title;
            InfoBarMessage = message;
            InfoBarSeverity = severity;
        }

        private void ResetInfoBar()
        {
            InfoBarMessage = string.Empty;
            InfoBarOpen = false;
            InfoBarTitle = string.Empty;
            InfoBarSeverity = InfoBarSeverity.Informational;
        }

        public static bool ReverseBoolean(bool value)
        {
            return !value;
        }
    }
}
