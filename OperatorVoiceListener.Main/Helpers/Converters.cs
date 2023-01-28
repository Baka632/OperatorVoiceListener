using Windows.ApplicationModel.Resources;

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
}
