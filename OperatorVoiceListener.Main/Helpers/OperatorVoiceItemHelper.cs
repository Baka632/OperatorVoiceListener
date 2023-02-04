using System.Collections.Immutable;
using System.Globalization;

namespace OperatorVoiceListener.Main.Helpers
{
    public readonly struct OperatorVoiceItemHelper
    {
        private readonly ImmutableDictionary<string, string> OpCodenameToNameMapping;
        private readonly ImmutableDictionary<string, OperatorVoiceInfo[]> OpCodenameToVoiceMapping;

        public OperatorVoiceItemHelper(ImmutableDictionary<string, string> opCodenameToNameMapping, ImmutableDictionary<string, OperatorVoiceInfo[]> opCodenameToVoiceMapping)
        {
            OpCodenameToNameMapping = opCodenameToNameMapping;
            OpCodenameToVoiceMapping = opCodenameToVoiceMapping;
        }

        public (OperatorVoiceInfo?, OperatorVoiceLine?) GetFullVoiceDetail(OperatorVoiceLine item, bool useAnyAvailableInfo = false)
        {
            if (OpCodenameToVoiceMapping.TryGetValue(item.CharactorCodename, out OperatorVoiceInfo[]? voiceInfos))
            {
                IEnumerable<OperatorVoiceInfo> infos = from info in voiceInfos where info.Type == item.VoiceType select info;

                if (infos.Any())
                {
                    OperatorVoiceInfo voiceInfo = infos.First();
                    OperatorVoiceLine? voiceItem = (from OperatorVoiceLine? vi in voiceInfo.Voices where vi.Value.VoiceId == item.VoiceId select vi).FirstOrDefault();
                    return (voiceInfo, voiceItem);
                }
                else
                {
                    if (useAnyAvailableInfo)
                    {
                        OperatorVoiceInfo voiceInfo = voiceInfos.FirstOrDefault();
                        OperatorVoiceLine? voiceItem = (from OperatorVoiceLine? vi in voiceInfo.Voices where vi.Value.VoiceId == item.VoiceId select vi).FirstOrDefault();
                        return (voiceInfo, voiceItem);
                    }

                    return (null, null);
                }
            }
            else
            {
                return (null, null);
            }
        }

        public bool TryGetOperatorName(OperatorVoiceLine item, out string? opName)
        {
            string codename = item.CharactorCodename.Split('_')[0];
            if (OpCodenameToNameMapping.TryGetValue(codename, out string? name))
            {
                opName = name;
                return true;
            }
            else
            {
                opName = null;
                return false;
            }
        }
    }
}
