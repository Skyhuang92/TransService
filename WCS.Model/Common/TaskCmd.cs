using System;
using System.Collections.Generic;
using System.Text;

namespace WCS.Entity
{
    public class TaskCmd
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long Objid
        {
            get;
            set;
        }
        /// <summary>
        /// 任务流水号
        /// </summary>
        public long TaskNo
        {
            get;
            set;
        }
        /// <summary>
        /// 指令类型
        /// </summary>
        public string CmdType
        {
            get; 
            set;
        }
        /// <summary>
        /// 指令步骤
        /// </summary>
        public string CmdStep
        {
            get; 
            set;
        }
        /// <summary>
        /// 起始地址类型
        /// </summary>
        public string SlocType
        {
            get; 
            set;
        }
        /// <summary>
        /// 起始地址
        /// </summary>
        public string SlocNo
        {
            get; 
            set;
        }
        /// 起始PLC地址
        /// </summary>
        public string SlocPlcNo
        {
            get; 
            set;
        }
        /// <summary>
        /// 目的地址类型
        /// </summary>
        public string ElocType
        {
            get; 
            set;
        }
        /// <summary>
        /// 目的地址
        /// </summary>
        public string ElocNo
        {
            get; 
            set;
        }
        /// <summary>
        /// 目的PLC地址
        /// </summary>
        public string ElocPlcNo
        {
            get; 
            set;
        }
        /// <summary>
        /// RFID号码
        /// </summary>
        public string PalletNo
        {
            get; 
            set;
        }
        /// <summary>
        /// 工装类型
        /// </summary>
        public string PalletType
        {
            get; 
            set;
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime
        {
            get; 
            set;
        }

        /// <summary>
        /// 起始站台区域
        /// </summary>
        public int SlocArea
        {
            get
            {
                var result = 0;
                if (!string.IsNullOrEmpty(SlocPlcNo))
                {
                    result = (int)Encoding.ASCII.GetBytes(SlocPlcNo.Substring(0, 1))[0];
                }
                return result;
            }
        }

        /// <summary>
        /// 起始站台编号
        /// </summary>
        public string SlocCode
        {
            get
            {
                var result = string.Empty;
                if (!string.IsNullOrEmpty(SlocPlcNo))
                {
                    result = SlocPlcNo.Substring(1);
                }
                return result;
            }
        }

        /// <summary>
        /// 目的站台区域
        /// </summary>
        public int ElocArea
        {
            get
            {
                var result = 0;
                if (!string.IsNullOrEmpty(ElocPlcNo))
                {
                    result = (int)Encoding.ASCII.GetBytes(ElocPlcNo.Substring(0, 1))[0];
                }
                return result;
            }
        }
        /// <summary>
        /// 目的站台编号
        /// </summary>
        public string ElocCode
        {
            get
            {
                var result = string.Empty;
                if (!string.IsNullOrEmpty(ElocPlcNo))
                {
                    result = ElocPlcNo.Substring(1);
                }
                return result;
            }
        }
    }
}
