using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCS.Entity
{
    public class ParameterForReqCmd
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long Objid
        {
            get;set;
        }
        /// <summary>
        /// 当前站台
        /// </summary>
        public string CurrLocNo
        {
            get; set;
        }
        /// <summary>
        /// 任务编号
        /// </summary>
        public long TaskNo
        {
            get; set;
        }
    }
}
