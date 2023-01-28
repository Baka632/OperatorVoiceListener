using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OperatorVoiceListener.Main.Models;

namespace OperatorVoiceListener.Main.Helpers
{
    public class OperatorVoiceItemHelper
    {
        private readonly ImmutableDictionary<string, string> OpCodenameToNameMapping;
        private readonly ImmutableDictionary<string, OperatorVoiceInfo[]> OpCodenameToVoiceMapping;

        public OperatorVoiceItemHelper(ImmutableDictionary<string, string> opCodenameToNameMapping, ImmutableDictionary<string, OperatorVoiceInfo[]> opCodenameToVoiceMapping)
        {
            OpCodenameToNameMapping = opCodenameToNameMapping;
            OpCodenameToVoiceMapping = opCodenameToVoiceMapping;
        }

        public (OperatorVoiceInfo?, OperatorVoiceItem?) GetFullVoiceDetail(OperatorVoiceItem item)
        {
            if (OpCodenameToVoiceMapping.TryGetValue(item.CharactorCodename, out OperatorVoiceInfo[]? voiceInfos))
            {
                IEnumerable<OperatorVoiceInfo> infos = from info in voiceInfos where info.Type == item.VoiceType select info;

                if (infos.Any())
                {
                    OperatorVoiceInfo voiceInfo = infos.First();
                    OperatorVoiceItem? voiceItem = (from OperatorVoiceItem? vi in voiceInfo.Voices where vi.Value.VoiceId == item.VoiceId select vi).FirstOrDefault();
                    return (voiceInfo, voiceItem);
                }
                else
                {
                    return (null, null);
                }
            }
            else
            {
                return (null, null);
            }
        }

        public bool TryGetOperatorName(OperatorVoiceItem item, out string? opName)
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
