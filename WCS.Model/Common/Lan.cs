using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRBL.McLanguage;

namespace WCS.Entity
{
    public static class Lan
    {
        private static string language;
        public static string Language
        {
            get
            {
                if (string.IsNullOrEmpty(language))
                {
                    language = "zh_CN";
                }
                return language;
            }
            set
            {
                language = value;
            }
        }

        public static string Info(string key)
        {
            var oResult = string.Empty;
            try
            {
                oResult = Manager.Instance[Language][key].ToString();
            }
            catch
            {
                oResult = $"{key}：No Data";
            }
            return oResult;
        }

        public static string Info(string key, object msg)
        {
            var oResult = string.Empty;
            try
            {
                oResult = string.Format(Manager.Instance[Language][key].ToString(), msg);
            }
            catch
            {
                oResult = $"{key}：No Data";
            }
            return oResult;
        }

        public static string Info(string key, string[] msg)
        {
            var oResult = string.Empty;
            try
            {
                oResult = string.Format(Manager.Instance[Language][key].ToString(), msg);
            }
            catch
            {
                oResult = $"{key}：No Data";
            }
            return oResult;
        }
    }
}
