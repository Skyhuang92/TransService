using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCS.Entity
{
    public class McConfig
    {
        #region 单例
        private static McConfig _this = null;
        public static McConfig Instance
        {
            get
            {
                if (_this == null)
                {
                    lock (typeof(McConfig))
                    {
                        if (_this == null)
                        {
                            _this = new McConfig();
                        }
                    }
                }
                return _this;
            }
        }
        private McConfig()
        {
            DbType = GetConfig("DbType");
            LocArea = GetConfig("LocArea");
            LocHost = GetConfig("LocHost");
            OpcType = GetConfig("OpcType");
            OpcServerName = GetConfig("OpcServerName");
            OpcServerHost = GetConfig("OpcServerHost");
            OpcServerPort = GetConfig("OpcServerPort");
            OpcServerGroup = GetConfig("OpcServerGroup");
        }
        #endregion

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType
        {
            get;
            set;
        }

        public string OpcType
        {
            get;
            set;
        }

        public string OpcServerName
        {
            get;
            set;
        }

        public string OpcServerHost
        {
            get;
            set;
        }

        public string OpcServerPort
        {
            get;
            set;
        }

        public string OpcServerGroup
        {
            get;
            set;
        }

        /// <summary>
        /// 站台区域
        /// </summary>
        public string LocArea
        {
            get;
            set;
        }

        /// <summary>
        /// 站台IP
        /// </summary>
        public string LocHost
        {
            get;
            set;
        }

        /// <summary>
        /// 读配置文档
        /// </summary>
        public string GetConfig(string key)
        {
            var value = string.Empty;
            try
            {
                value = ConfigurationManager.AppSettings[key];
            }
            catch
            {
                value = string.Empty;
            }
            return value;
        }
    }
}
