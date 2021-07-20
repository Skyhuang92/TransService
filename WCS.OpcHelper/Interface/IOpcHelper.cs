using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCS.Entity;

namespace WCS.OpcHelper
{
    public interface IOpcHelper
    {
        /// <summary>
        /// 连接OPC
        /// </summary>
        bool OpcConnection(string[] opcStringItems, ref string errmsg);

        /// <summary>
        /// 获取站台PLC信息
        /// </summary>
        bool GetLocPlcStatus(Loc loc, ref string errmsg);

        /// <summary>
        /// 写入信息
        /// </summary>
        bool SendInfoToPlc(Loc loc, ref string errmsg);

        /// <summary>
        /// 清空写入信息
        /// </summary>
        bool ClearInfoToPlc(Loc loc, ref string errmsg);

        /// <summary>
        /// 写入故障号
        /// </summary>
        bool SendFaultNoToPlc(Loc loc, ref string errmsg);

        /// <summary>
        /// 写入任务号
        /// </summary>
        bool SendTaskNoToPlc(Loc loc, ref string errmsg);

        /// <summary>
        /// 写入任务已处理新号
        /// </summary>
        bool SendHandleRequestFlagToPlc(Loc loc, ref string errmsg);

        /// <summary>
        /// 写入取货已处理信号
        /// </summary>
        bool SendHandlePickFlagToPlc(Loc loc, ref string errmsg);

        /// <summary>
        /// 写入取货已处理信号
        /// </summary>
        bool SendInOutSwitchFlagToPlc(Loc loc, ref string errmsg);

        /// <summary>
        /// 写入堆垛机状态（使用备用字段）
        /// </summary>
        bool SendSpareToPlc(Loc loc, ref string errmsg, int? status = 0);
    }
}
