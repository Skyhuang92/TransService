using System;
using System.Collections.Generic;
using System.Text;

namespace WCS.Entity
{
    public class OpcItem
    {
        /// <summary>
        /// 站台编号
        /// </summary>
        public string LocNo
        {
            get; 
            set;
        }
        /// <summary>
        /// 站台PLC编号
        /// </summary>
        public string LocPlcNo
        {
            get; 
            set;
        }
        /// <summary>
        /// 站台类型
        /// </summary>
        public string Kind
        {
            get; 
            set;
        }
        /// <summary>
        /// 测点长名
        /// </summary>
        public string TagLongName
        {
            get; 
            set;
        }
        /// <summary>
        /// 业务唯一标示
        /// </summary>
        public string BizIdentity
        {
            get; 
            set;
        }
    }
}
