using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCS.Entity
{
    public class ParameterForReqTask
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long Objid
        {
            get;set;
        }
        /// <summary>
        /// 订单类型编号
        /// </summary>
        public string OrderTypeNo
        {
            get; set;
        }
        /// <summary>
        /// 起始位置
        /// </summary>
        public string SlocNo
        {
            get; set;
        }
        /// <summary>
        /// 目的位置
        /// </summary>
        public string ElocNo
        {
            get; set;
        }
        /// <summary>
        /// 工装编号
        /// </summary>
        public string PalletNo
        {
            get; set;
        }
        /// <summary>
        /// 数据来源
        /// </summary>
        public int SourceType
        {
            get; set;
        }
    }
}
