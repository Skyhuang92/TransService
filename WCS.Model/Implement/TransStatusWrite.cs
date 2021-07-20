using System;
using System.Collections.Generic;
using System.Text;

namespace WCS.Entity
{
    public class TransStatusWrite : ILocStatusWrite
    {
        /// <summary>
        /// 任务号
        /// </summary>
        public long TaskNo
        {
            get;
            set;
        }
        /// <summary>
        /// 托盘号
        /// </summary>
        public string PalletNo
        {
            get;
            set;
        }
        /// <summary>
        /// 源地址区域符号
        /// </summary>
        public string SlocArea
        {
            get;
            set;
        }
        /// <summary>
        /// 源地址设备编号
        /// </summary>
        public string SlocCode
        {
            get;
            set;
        }
        /// <summary>
        /// 目的地址区域符号
        /// </summary>
        public string ElocArea
        {
            get;
            set;
        }
        /// <summary>
        /// 目的地址设备编号
        /// </summary>
        public string ElocCode
        {
            get;
            set;
        }
        /// <summary>
        /// “请求任务”已处理信号
        /// </summary>
        public int HandleRequest
        {
            get;
            set;
        }
        /// <summary>
        /// “请求取货”已处理信号
        /// </summary>
        public int HandleNeedToPut
        {
            get;
            set;
        }
        /// <summary>
        /// 故障编号
        /// </summary>
        public string FaultNo
        {
            get;
            set;
        }

        /// <summary>
        /// 源地址
        /// </summary>
        public string SlocPlcNo
        {
            get
            {
                return getLocPlcNo(SlocArea, SlocCode);
            }
        }

        /// <summary>
        /// 目的地址
        /// </summary>
        public string ElocPlcNo
        {
            get
            {
                return getLocPlcNo(ElocArea, ElocCode);
            }
        }

        private string getLocPlcNo(string locArea, string locCode)
        {
            var result = string.Empty;
            try
            {
                result = ((char)int.Parse(locArea)).ToString() + locCode;
            }
            catch
            {
                result = null;
            }
            return result;
        }
    }
}
