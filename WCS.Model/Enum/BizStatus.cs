using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCS.Entity
{
    public enum BizStatus
    {
        /// <summary>
        /// 初始状态，检测信号
        /// </summary>
        None = 0,
        /// <summary>
        /// 创建产品信息
        /// </summary>
        CreateProduct,
        /// <summary>
        /// 请求任务
        /// </summary>
        RequestTask,
        /// <summary>
        /// 请求指令
        /// </summary>
        RequestCmd,
        /// <summary>
        /// 信息校验
        /// </summary>
        Check,
        /// <summary>
        /// 获取数据-指令
        /// </summary>
        GetTaskCmd,
        /// <summary>
        /// 写入指令
        /// </summary>
        WriteTaskCmd,
        /// <summary>
        /// 写入任务已处理信号
        /// </summary>
        WriteTaskDeal,
        /// <summary>
        /// 写入取货已处理信号
        /// </summary>
        WritePickDeal,
        /// <summary>
        /// 更新指令步骤
        /// </summary>
        UpdateCmdStep,
        /// <summary>
        /// 结束指令
        /// </summary>
        FinishTaskCmd,
        /// <summary>
        /// 复位
        /// </summary>
        ResetLocStatus
    }
}
