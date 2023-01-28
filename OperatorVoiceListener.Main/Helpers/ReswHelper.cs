using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Windows.ApplicationModel.Resources;

namespace OperatorVoiceListener.Main.Helpers
{
    internal static class ReswHelper
    {
        public static string GetReswString(string name)
        {
            ResourceLoader loader= new();
            return loader.GetString(name);
        }
    }
}
