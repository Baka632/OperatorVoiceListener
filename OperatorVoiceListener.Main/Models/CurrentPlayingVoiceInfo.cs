namespace OperatorVoiceListener.Main.Models
{
    public readonly record struct CurrentPlayingVoiceInfo
    {
        public CurrentPlayingVoiceInfo(string operatorCodename, string voiceID, OperatorVoiceType voiceType)
        {
            OperatorCodename = operatorCodename;
            VoiceID = voiceID;
            VoiceType = voiceType;
        }

        public string OperatorCodename { get; }
        public string VoiceID { get; }
        public OperatorVoiceType VoiceType { get; }
    }
}