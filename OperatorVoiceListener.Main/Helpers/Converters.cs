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
                    OperatorVoiceType.ChineseMandarin => "中文-普通话",
                    OperatorVoiceType.ChineseRegional => "中文-方言",
                    OperatorVoiceType.Japanese => "日语",
                    OperatorVoiceType.English => "英语",
                    OperatorVoiceType.Korean => "韩语",
                    OperatorVoiceType.Italian => "意大利语",
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
