using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.LanguageResource
{
    public class Localization
    {
        public static string getPropValue(HttpCookie cookie, object src, string propName)
        {
            var value = (string)src.GetType().GetProperty(propName).GetValue(src, null);
            var value_en = (string)src.GetType().GetProperty(propName + "_en").GetValue(src, null);
            
            if (cookie != null && cookie.Value != "zh-TW")
            {
                if (value_en != null && value_en != "")
                {
                    return value_en;
                }
            }
            return value;
        }
    }
}