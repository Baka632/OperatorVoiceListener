using OperatorVoiceListener.Main.Services;
using CustomVoiceRes = ArknightsResources.Operators.VoiceResources.Custom.Properties.Resources;
using CNVoiceRes = ArknightsResources.Operators.VoiceResources.CN.Properties.Resources;
using ENVoiceRes = ArknightsResources.Operators.VoiceResources.EN.Properties.Resources;
using JPVoiceRes = ArknightsResources.Operators.VoiceResources.JP.Properties.Resources;
using KRVoiceRes = ArknightsResources.Operators.VoiceResources.KR.Properties.Resources;
using Windows.Media.Playback;
using System.Diagnostics;
using Microsoft.UI.Dispatching;
using System.Collections.Immutable;
using OperatorVoiceListener.Main.Models;
using OperatorVoiceListener.Main.Helpers;
using Windows.ApplicationModel.Resources;

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
        private string cv = string.Empty;
        [ObservableProperty]
        private string subtitle = string.Empty;
        [ObservableProperty]
        private string title = string.Empty;

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
        [ObservableProperty]
        private bool isSubtitleVisable = false;

        private IEnumerable<OperatorCodenameInfo>? AllOperatorCodenameInfos;
        private readonly OperatorVoiceItemHelper OperatorVoiceItemHelper;

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
        public ImmutableDictionary<string, string> OpCodenameToNameMapping { get; }
        public ImmutableDictionary<string, OperatorVoiceInfo[]> OpCodenameToVoiceMapping { get; }

        public MainViewModel()
        {
            OperatorTextResourceHelper textResourceHelper = new(ArknightsResources.Operators.TextResources.Properties.Resources.ResourceManager);
            OpCodenameToNameMapping = textResourceHelper.GetOperatorCodenameMapping(AvailableCultureInfos.ChineseSimplifiedCultureInfo);
            OpCodenameToVoiceMapping = textResourceHelper.GetAllOperatorVoiceInfos(AvailableCultureInfos.ChineseSimplifiedCultureInfo);
            AudioService = new AudioService();
            AudioService.Player.MediaFailed += OnMediaPlayFailed;
            OperatorVoiceItemHelper = new OperatorVoiceItemHelper(OpCodenameToNameMapping, OpCodenameToVoiceMapping);
        }

        private void OnMediaPlayFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
#if DEBUG
            Debug.WriteLine("When will WinUI 3 fix this bug?\nSee:https://github.com/microsoft/WindowsAppSDK/issues/3305");
#endif
        }

        partial void OnVoiceTypeIndexChanged(int value)
        {
            VoiceType = OperatorVoiceTypes.ElementAtOrDefault(value);
        }

        public async Task StartVoicePlay()
        {
            ResetInfoBar();
            IsLoadingAudio = true;
            OperatorVoiceLine voiceItem = new(OperatorCodename, VoiceID, string.Empty, string.Empty, VoiceType);

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
                string title;
                string cv;
                string subtitle;
                string lang = voiceItem.VoiceType switch
                {
                    OperatorVoiceType.ChineseMandarin => ReswHelper.GetReswString("ChineseMandarin"),
                    OperatorVoiceType.ChineseRegional => ReswHelper.GetReswString("ChineseRegional"),
                    OperatorVoiceType.Japanese => ReswHelper.GetReswString("Japanese"),
                    OperatorVoiceType.English => ReswHelper.GetReswString("English"),
                    OperatorVoiceType.Korean => ReswHelper.GetReswString("Korean"),
                    OperatorVoiceType.Italian => ReswHelper.GetReswString("Italian"),
                    _ => string.Empty,
                };

                byte[] voice = await resourceHelper.GetOperatorVoiceAsync(voiceItem);
                (OperatorVoiceInfo?, OperatorVoiceLine?) voiceDetails = OperatorVoiceItemHelper.GetFullVoiceDetail(voiceItem);
                if (voiceDetails.Item1 is not null && voiceDetails.Item2 is not null)
                {
                    OperatorVoiceInfo voiceInfo = voiceDetails.Item1.Value;
                    cv = voiceInfo.CV;

                    OperatorVoiceLine operatorVoiceItem = voiceDetails.Item2.Value;
                    Title = operatorVoiceItem.VoiceTitle;
                    subtitle = operatorVoiceItem.VoiceText;
                    title = OperatorVoiceItemHelper.TryGetOperatorName(voiceItem, out string? opName)
                    ? $"{opName} - {operatorVoiceItem.VoiceTitle} [{lang}]"
                    : $"{voiceItem.CharactorCodename} - {operatorVoiceItem.VoiceTitle} [{lang}]";
                }
                else
                {
                    title = OperatorVoiceItemHelper.TryGetOperatorName(voiceItem, out string? opName)
                        ? $"{opName} [{lang}]"
                        : $"{voiceItem.CharactorCodename} [{lang}]";
                    cv = string.Empty;
                    subtitle = string.Empty;
                    Title = string.Empty;
                }
                
                Subtitle = subtitle;
                Cv = cv;
                await AudioService.PlayOperatorVoice(voice, title, subtitle, cv);
                if (IsSubtitleVisable == false)
                {
                    IsSubtitleVisable = true;
                }
            }
            catch (ArgumentException)
            {
                SetInfoBar(true,
                           ReswHelper.GetReswString("VoiceFileNotFoundTitle"),
                           ReswHelper.GetReswString("VoiceFileNotFoundMessage"),
                           InfoBarSeverity.Error);
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

        public static bool ReverseBoolean(bool value) => !value;

        internal IEnumerable<OperatorCodenameInfo> FindOperatorCodename(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                if (AllOperatorCodenameInfos is null)
                {
                    IEnumerable<OperatorCodenameInfo> result = from codename in OpCodenameToVoiceMapping.Keys
                                                               join codenameNamePair in OpCodenameToNameMapping on codename.Split('_')[0] equals codenameNamePair.Key
                                                               select new OperatorCodenameInfo(codename, codenameNamePair.Value);
                    List<OperatorCodenameInfo> list = result.ToList();
                    list.Sort();
                    AllOperatorCodenameInfos = list;
                    return list;
                }
                else
                {
                    return AllOperatorCodenameInfos;
                }
            }

            List<OperatorCodenameInfo> target = new(20);
            foreach (var item in OpCodenameToVoiceMapping.Keys)
            {
                if (item == "aprot")
                {
                    continue;
                }

                if (item.FirstOrDefault() == text.FirstOrDefault() && item.Contains(text))
                {
                    target.Add(new OperatorCodenameInfo(item, OpCodenameToNameMapping[item.Split('_')[0]]));
                }
            }
            target.Sort();
            return target;
        }

        internal IEnumerable<OperatorIdTitleInfo> FindCurrentOperatorVoiceId()
        {
            if (OpCodenameToVoiceMapping.TryGetValue(OperatorCodename, out OperatorVoiceInfo[]? voiceInfos))
            {
                var infos = from info in voiceInfos where info.Type == VoiceType select info;

                if (infos.Any())
                {
                    OperatorVoiceInfo voiceInfo = infos.First();

                    var idTitleInfos = new List<OperatorIdTitleInfo>(40);
                    idTitleInfos.AddRange(from item in voiceInfo.Voices
                                          select new OperatorIdTitleInfo(item.VoiceId, item.VoiceTitle));
                    return idTitleInfos;
                }
                else
                {
                    return Array.Empty<OperatorIdTitleInfo>();
                }
            }
            else
            {
                return Array.Empty<OperatorIdTitleInfo>();
            }
        }
    }
}
