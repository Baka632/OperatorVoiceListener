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
}
