using OperatorVoiceListener.Main.Services;
using CustomVoiceRes = ArknightsResources.Operators.VoiceResources.Custom.Properties.Resources;
using CNVoiceRes = ArknightsResources.Operators.VoiceResources.CN.Properties.Resources;
using ENVoiceRes = ArknightsResources.Operators.VoiceResources.EN.Properties.Resources;
using JPVoiceRes = ArknightsResources.Operators.VoiceResources.JP.Properties.Resources;
using KRVoiceRes = ArknightsResources.Operators.VoiceResources.KR.Properties.Resources;
using NoneVoiceRes = ArknightsResources.Operators.VoiceResources.None.Properties.Resources;
using Windows.Media.Playback;
using System.Diagnostics;
using System.Collections.Immutable;
using OperatorVoiceListener.Main.Models;
using OperatorVoiceListener.Main.Helpers;
using System.Globalization;

namespace OperatorVoiceListener.Main.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string operatorCodename = string.Empty;
        [ObservableProperty]
        private string voiceID = string.Empty;
        [ObservableProperty]
        private int voiceTypeIndex;
        [ObservableProperty]
        private LanguageType subtitleLanguageType;
        [ObservableProperty]
        private int subtitleLanguageIndex;
        [ObservableProperty]
        private string displayCv = string.Empty;
        [ObservableProperty]
        private string displaySubtitle = string.Empty;
        [ObservableProperty]
        private string displayTitle = string.Empty;
        [ObservableProperty]
        private bool isLoadingAudio;
        [ObservableProperty]
        private string doctorName = string.Empty;

        [ObservableProperty]
        private string infoBarMessage = string.Empty;
        [ObservableProperty]
        private string infoBarTitle = string.Empty;
        [ObservableProperty]
        private bool infoBarOpen;
        [ObservableProperty]
        private InfoBarSeverity infoBarSeverity;
        [ObservableProperty]
        private bool isInformationExpanderVisible = false;
        public AudioService AudioService { get; }
        public IEnumerable<OperatorIdTitleInfo> CurrentOperatorVoiceIds => FindCurrentOperatorVoiceId();
        private OperatorVoiceType VoiceType { get; set; } = OperatorVoiceType.ChineseMandarin;
        private Dictionary<LanguageType, ImmutableDictionary<string, string>> OpCodenameToNameMappingDict { get; } = new(2);
        private Dictionary<LanguageType, ImmutableDictionary<string, OperatorVoiceInfo[]>> OpCodenameToVoiceMappingDict { get; } = new(2);
        private ImmutableDictionary<string, string> OpCodenameToNameMapping => OpCodenameToNameMappingDict[SubtitleLanguageType];
        private ImmutableDictionary<string, OperatorVoiceInfo[]> OpCodenameToVoiceMapping => OpCodenameToVoiceMappingDict[SubtitleLanguageType];
        private ImmutableDictionary<string, string> OpCodenameToNameMappingInvariant => OpCodenameToNameMappingDict[InvariantLanguageType];
        private ImmutableDictionary<string, OperatorVoiceInfo[]> OpCodenameToVoiceMappingInvariant => OpCodenameToVoiceMappingDict[InvariantLanguageType];
        private Dictionary<LanguageType, IEnumerable<OperatorCodenameInfo>> AllOperatorCodenameDict { get; } = new(2);

        internal OperatorVoiceType[] OperatorVoiceTypes = new OperatorVoiceType[]
        {
            OperatorVoiceType.ChineseMandarin,
            OperatorVoiceType.ChineseRegional,
            OperatorVoiceType.Japanese,
            OperatorVoiceType.English,
            OperatorVoiceType.Korean,
            OperatorVoiceType.Italian,
            OperatorVoiceType.None,
        };

        internal LanguageType[] AvailableLanguages = new LanguageType[]
        {
            //TODO: Add uncomment language when it is ready

            LanguageType.ChineseSimplified,
            //SubtitleLanguageType.ChineseTraditional,
            LanguageType.English,
            //SubtitleLanguageType.Japanese,
            //SubtitleLanguageType.Korean,
        };

        private readonly LanguageType InvariantLanguageType;
        private CurrentPlayingVoiceInfo CurrentPlayingVoiceInfo;
        private const string DOCTOR_NAME_PLACEHOLDER = "{@nickname}";

        public MainViewModel()
        {
            OperatorTextResourceHelper textResourceHelper = new(ArknightsResources.Operators.TextResources.Properties.Resources.ResourceManager);
            InvariantLanguageType = CultureInfo.CurrentUICulture.Name.ToLowerInvariant() switch
            {
                //TODO: Uncomment if available

                "zh-hans" or "zh-cn" => LanguageType.ChineseSimplified,
                //"zh-hant" or "zh-tw" or "zh-hk" or "zh-mo" => LanguageType.ChineseTraditional,
                //"ja-jp"=> LanguageType.Japanese,
                //"ko-kr" => LanguageType.Korean,
                _ => LanguageType.English,
            };

            OpCodenameToNameMappingDict[SubtitleLanguageType] = textResourceHelper.GetOperatorCodenameMapping(SubtitleLanguageType.AsCultureInfo());
            OpCodenameToVoiceMappingDict[SubtitleLanguageType] = textResourceHelper.GetAllOperatorVoiceInfos(SubtitleLanguageType.AsCultureInfo());
            AudioService = new AudioService();
            AudioService.Player.MediaFailed += OnMediaPlayFailed;
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

        partial void OnSubtitleLanguageIndexChanged(int value)
        {
            LanguageType langType = AvailableLanguages.ElementAtOrDefault(value);
            OperatorTextResourceHelper textResourceHelper = new(ArknightsResources.Operators.TextResources.Properties.Resources.ResourceManager);
            if (!OpCodenameToNameMappingDict.TryGetValue(langType, out _))
            {
                OpCodenameToNameMappingDict[langType] = textResourceHelper.GetOperatorCodenameMapping(langType.AsCultureInfo());
            }

            if (!OpCodenameToVoiceMappingDict.TryGetValue(langType, out _))
            {
                OpCodenameToVoiceMappingDict[langType] = textResourceHelper.GetAllOperatorVoiceInfos(langType.AsCultureInfo());
            }
            SubtitleLanguageType = langType;

            OperatorVoiceLine voiceItem = new(CurrentPlayingVoiceInfo.OperatorCodename, CurrentPlayingVoiceInfo.VoiceID, string.Empty, string.Empty, CurrentPlayingVoiceInfo.VoiceType);

            OperatorVoiceItemHelper voiceItemHelper = new(OpCodenameToNameMapping, OpCodenameToVoiceMapping);
            (OperatorVoiceInfo? _, OperatorVoiceLine? voiceLine) = voiceItemHelper.GetFullVoiceDetail(voiceItem);

            DisplaySubtitle = voiceLine.HasValue ? voiceLine.Value.VoiceText.Replace(DOCTOR_NAME_PLACEHOLDER, DoctorName) : ReswHelper.GetReswString("InfoNotAvailableMessage");
        }

        private void UpdateDisplayProperties()
        {
            OperatorVoiceLine voiceItemForCv = new(OperatorCodename, VoiceID, string.Empty, string.Empty, VoiceType);
            OperatorVoiceItemHelper voiceItemHelperInvariant = new(OpCodenameToNameMappingInvariant, OpCodenameToVoiceMappingInvariant);
            (OperatorVoiceInfo? voiceInfo, OperatorVoiceLine? voiceLineInvariant) = voiceItemHelperInvariant.GetFullVoiceDetail(voiceItemForCv);
            DisplayCv = voiceInfo.HasValue ? voiceInfo.Value.CV : ReswHelper.GetReswString("InfoNotAvailableMessage");
            DisplayTitle = voiceLineInvariant.HasValue ? voiceLineInvariant.Value.VoiceTitle : "?";

            OperatorVoiceLine voiceItemForSubtitle = new(OperatorCodename, VoiceID, string.Empty, string.Empty, VoiceType);

            OperatorVoiceItemHelper voiceItemHelper = new(OpCodenameToNameMapping, OpCodenameToVoiceMapping);
            (OperatorVoiceInfo? _, OperatorVoiceLine? voiceLine) = voiceItemHelper.GetFullVoiceDetail(voiceItemForSubtitle);

            DisplaySubtitle = voiceLine.HasValue ? voiceLine.Value.VoiceText.Replace(DOCTOR_NAME_PLACEHOLDER, DoctorName) : ReswHelper.GetReswString("InfoNotAvailableMessage");
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
                OperatorVoiceType.Korean => new OperatorVoiceResourceHelper(KRVoiceRes.ResourceManager),
                _ => new OperatorVoiceResourceHelper(NoneVoiceRes.ResourceManager),
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
                    _ => ReswHelper.GetReswString("NoneVoice"),
                };

                byte[] voice = await resourceHelper.GetOperatorVoiceAsync(voiceItem);
                OperatorVoiceItemHelper voiceItemHelper = new(OpCodenameToNameMappingInvariant, OpCodenameToVoiceMappingInvariant);
                (OperatorVoiceInfo?, OperatorVoiceLine?) voiceDetails = voiceItemHelper.GetFullVoiceDetail(voiceItem);
                if (voiceDetails.Item1.HasValue && voiceDetails.Item2.HasValue)
                {
                    OperatorVoiceInfo voiceInfo = voiceDetails.Item1.Value;
                    cv = voiceInfo.CV;

                    OperatorVoiceLine operatorVoiceItem = voiceDetails.Item2.Value;
                    subtitle = operatorVoiceItem.VoiceText;
                    title = voiceItemHelper.TryGetOperatorName(voiceItem, out string? opName)
                    ? $"{opName} - {operatorVoiceItem.VoiceTitle} [{lang}]"
                    : $"{voiceItem.CharactorCodename} - {operatorVoiceItem.VoiceTitle} [{lang}]";
                }
                else
                {
                    title = voiceItemHelper.TryGetOperatorName(voiceItem, out string? opName)
                        ? $"{opName} [{lang}]"
                        : $"{voiceItem.CharactorCodename} [{lang}]";
                    cv = string.Empty;
                    subtitle = string.Empty;
                }

                await AudioService.PlayOperatorVoice(voice, title, subtitle, cv);
                CurrentPlayingVoiceInfo = new(OperatorCodename,VoiceID,VoiceType);
                UpdateDisplayProperties();

                if (IsInformationExpanderVisible == false)
                {
                    IsInformationExpanderVisible = true;
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
        public static Visibility ReverseBooleanToVisibility(bool value) => !value ? Visibility.Visible : Visibility.Collapsed;

        internal IEnumerable<OperatorCodenameInfo> FindOperatorCodename(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                if (AllOperatorCodenameDict.TryGetValue(InvariantLanguageType, out var infos))
                {
                    return infos;
                }
                else
                {
                    IEnumerable<OperatorCodenameInfo> result = from codename in OpCodenameToVoiceMappingInvariant.Keys
                                                               join codenameNamePair in OpCodenameToNameMappingInvariant on codename.Split('_')[0] equals codenameNamePair.Key
                                                               select new OperatorCodenameInfo(codename, codenameNamePair.Value);
                    List<OperatorCodenameInfo> list = result.ToList();
                    list.Sort();
                    AllOperatorCodenameDict[InvariantLanguageType] = list;
                    return list;
                }
            }

            List<OperatorCodenameInfo> target = new(20);
            foreach (var item in OpCodenameToVoiceMappingInvariant.Keys)
            {
                if (item == "aprot")
                {
                    continue;
                }

                if (item.FirstOrDefault() == text.FirstOrDefault() && item.Contains(text))
                {
                    target.Add(new OperatorCodenameInfo(item, OpCodenameToNameMappingInvariant[item.Split('_')[0]]));
                }
            }
            target.Sort();
            return target;
        }

        internal IEnumerable<OperatorIdTitleInfo> FindCurrentOperatorVoiceId()
        {
            if (OpCodenameToVoiceMappingInvariant.TryGetValue(OperatorCodename, out OperatorVoiceInfo[]? voiceInfos))
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
