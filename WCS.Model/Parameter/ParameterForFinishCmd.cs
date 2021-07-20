using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCS.Entity
{
    public class ParameterForFinishCmd
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long Objid
        {
            get; set;
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
        /// <summary>
        /// 结束标记
        /// </summary>
        public int FinishFlag
        {
            get; set;
        }
    }
}
