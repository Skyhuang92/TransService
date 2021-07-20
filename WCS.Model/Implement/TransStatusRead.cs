using System;
using System.Collections.Generic;
using System.Text;

namespace WCS.Entity
{ 
    public class TransStatusRead :ILocStatusRead
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
        /// 源地址区域编号
        /// </summary>
        public string SlocArea
        {
            get;
            set;
        }
        /// <summary>
        /// 源地址设备Id
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
        /// 状态字：自动
        /// </summary>
        public int StatusAuto
        {
            get;
            set;
        }
        /// <summary>
        /// 状态字：故障
        /// </summary>
        public int StatusFault
        {
            get;
            set;
        }
        /// <summary>
        /// 状态字：有载
        /// </summary>
        public int StatusLoad
        {
            get;
            set;
        }
        /// <summary>
        /// 状态字：请求上位机下发任务
        /// </summary>
        public int StatusRequest
        {
            get;
            set;
        }
        /// <summary>
        /// 状态字：站点空闲可放货
        /// </summary>
        public int StatusFree
        {
            get;
            set;
        }
        /// <summary>
        /// 状态字：站点有货需取货
        /// </summary>
        public int StatusNeedToPut
        {
            get;
            set;
        }

        /// <summary>
        /// 状态字：出入站台切换
        /// </summary>
        public int StatusInOutSwitch
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

        /// <summary>
        /// 托盘数量
        /// </summary>
        public int PalletAmount
        {
            get;
            set;
        }
    }
}
