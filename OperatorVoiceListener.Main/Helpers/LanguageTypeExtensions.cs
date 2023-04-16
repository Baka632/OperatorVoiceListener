using OperatorVoiceListener.Main.Models;
using System.Globalization;

namespace OperatorVoiceListener.Main.Helpers
{
    public static class LanguageTypeExtensions
    {
        public static CultureInfo AsCultureInfo(this LanguageType type)
        {
            return type switch
            {
                LanguageType.English => AvailableCultureInfos.EnglishCultureInfo,
                LanguageType.Japanese => AvailableCultureInfos.JapaneseCultureInfo,
                LanguageType.Korean => AvailableCultureInfos.KoreanCultureInfo,
                LanguageType.ChineseTraditional => AvailableCultureInfos.ChineseTraditionalCultureInfo,
                _ => AvailableCultureInfos.ChineseSimplifiedCultureInfo,
            };
        }
    }
}
