using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCS.Trans
{
    public static class Tool
    {
        /// <summary>
        /// 读配置文档
        /// </summary>
        public static string GetConfig(string key)
        {
            var values = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrWhiteSpace(values))
            {
                return values;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 写配置文档
        /// </summary>
        public static void SetConfig(string key, object value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] != null)
            {
                config.AppSettings.Settings[key].Value = value.ToString();
            }
            else
            {
                config.AppSettings.Settings.Add(key, value.ToString());
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// 获取资源文件内容
        /// </summary>
        public static string GetLog(string key)
        {
            var oResult = string.Empty;
            try
            {
                oResult = App.Current.FindResource(key).ToString();
            }
            catch
            {
                oResult = $"No Data:{key} ";
            }
            return oResult;
        }

        /// <summary>
        /// 打开指定目录下文件
        /// </summary>
        public static void OpenFile(string NewFileName)
        {
            var process = new Process();
            try
            {
                var processStartInfo = new ProcessStartInfo(NewFileName);
                process.StartInfo = processStartInfo;
                process.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                process.Close();
                process = null;
            }
        }
    }
}
