using OperatorVoiceListener.Main.Models;
using System.Globalization;

namespace OperatorVoiceListener.Main.Helpers
{
    public class OperatorVoiceTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value switch
            {
                OperatorVoiceType OperatorVoiceType => OperatorVoiceType switch
                {
                    OperatorVoiceType.ChineseMandarin => ReswHelper.GetReswString("ChineseMandarin"),
                    OperatorVoiceType.ChineseRegional => ReswHelper.GetReswString("ChineseRegional"),
                    OperatorVoiceType.Japanese => ReswHelper.GetReswString("Japanese"),
                    OperatorVoiceType.English => ReswHelper.GetReswString("English"),
                    OperatorVoiceType.Korean => ReswHelper.GetReswString("Korean"),
                    OperatorVoiceType.Italian => ReswHelper.GetReswString("Italian"),
                    OperatorVoiceType.None => ReswHelper.GetReswString("NoneVoice"),
                    _ => string.Empty,
                },
                _ => DependencyProperty.UnsetValue,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class CultureInfoToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
                case CultureInfo cultureInfo:
                    if (cultureInfo.Name == AvailableCultureInfos.ChineseSimplifiedCultureInfo.Name)
                    {
                        return ReswHelper.GetReswString("ChineseSimplified");
                    }
                    else if (cultureInfo.Name == AvailableCultureInfos.EnglishCultureInfo.Name)
                    {
                        return ReswHelper.GetReswString("English");
                    }
                    else
                    {
                        goto default;
                    }
                default:
                    return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class LanguageTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value switch
            {
                LanguageType type => type switch
                {
                    LanguageType.ChineseTraditional => ReswHelper.GetReswString("ChineseTraditional"),
                    LanguageType.ChineseSimplified => ReswHelper.GetReswString("ChineseSimplified"),
                    LanguageType.Japanese => ReswHelper.GetReswString("Japanese"),
                    LanguageType.English => ReswHelper.GetReswString("English"),
                    LanguageType.Korean => ReswHelper.GetReswString("Korean"),
                    _ => string.Empty,
                },
                _ => DependencyProperty.UnsetValue
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
