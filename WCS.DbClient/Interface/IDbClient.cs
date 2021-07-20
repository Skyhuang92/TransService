using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DapperExtensions;
using WCS.Entity;

namespace WCS.DbClient
{
    public interface IDbClient
    {
        /// <summary>
        /// 获取数据库时间
        /// </summary>
        bool GetDbTime(ref string errmsg);

        /// <summary>
        /// 获取站台数据
        /// </summary>
        /// <param name="areaNo">站台区域</param>
        /// <returns></returns>
        Dictionary<string, Loc> GetLocData(string areaNo, ref string errmsg);

        /// <summary>
        /// 获取OPC项
        /// </summary>
        /// <param name="locNo">站台号</param>
        Dictionary<string, OpcItem> GetOpcItemsData(string locNo, ref string errmsg);

        /// <summary>
        /// 获取指令信息
        /// </summary>
        TaskCmd GetTaskCmd(TaskCmd taskCmd, ref string errmsg);

        /// <summary>
        /// 获取指令集合
        /// </summary>
        List<TaskCmd> GetTaskCmds(TaskCmd taskCmd, ref string errmsg);

        /// <summary>
        /// 修改指令步骤
        /// </summary>
        /// <param name="cmdObjid">指令主键</param>
        /// <param name="cmdStep">指令步骤</param>
        /// <returns></returns>
        bool UpdateCmdStep(long cmdObjid, string cmdStep, ref string errmsg);

        /// <summary>
        /// 获取请求生成任务参数表主键
        /// </summary>
        long GetObjidForReqTask(ref string errmsg);

        /// <summary>
        /// 空盘收集工位 控盘托盘垛数据绑定
        /// </summary>
        /// <param name="palletNo"></param>
        /// <param name="palletQty"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        bool CreateBPalletProduct(string palletNo, int palletQty, ref string errmsg);

        /// <summary>
        /// 请求生成任务
        /// </summary>
        long CreateTask(ParameterForReqTask parameter, ref string errmsg);

        /// <summary>
        /// 获取请求生成指令参数表主键
        /// </summary>
        long GetObjidForReqCmd(ref string errmsg);

        /// <summary>
        /// 请求生成指令
        /// </summary>
        long CreateCmd(ParameterForReqCmd parameter, ref string errmsg);

        /// <summary>
        /// 获取请求结束任务参数表主键
        /// </summary>
        long GetObjidForFinishCmd(ref string errmsg);

        /// <summary>
        /// 请求结束指令
        /// </summary>
        bool FinishTaskCmd(ParameterForFinishCmd parameter, ref string errmsg);

        /// <summary>
        /// 记录站台状态
        /// </summary>
        /// <param name="locNo">站台号</param>
        /// <param name="locStatus">PLC读取值</param>
        /// <returns></returns>
        bool RecordLocStatus(string locNo, TransStatusRead locStatus, ref string errmsg);

        /// <summary>
        /// 记录空盘收集工位站台状态（不记录托盘号）
        /// </summary>
        /// <param name="locNo">站台号</param>
        /// <param name="locStatus">PLC读取值</param>
        /// <returns></returns>
        bool RecordBPalletLocStatus(string locNo, TransStatusRead locStatus, ref string errmsg);

        /// <summary>
        /// 获取站台记录的托盘号
        /// </summary>
        /// <param name="locNo"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        string GetRecordBPalletLoc(string locNo, ref string errmsg);

        /// <summary>
        /// 任务处理
        /// </summary>
        /// <param name="taskNo">任务编号</param>
        /// <param name="finishflag">处理标记</param>
        /// <returns></returns>
        bool HandleTaskCmd(long taskNo, int finishflag, ref string errmsg);

        /// <summary>
        /// 获取上位机读取RFID
        /// </summary>
        /// <param name="locNo">站台号</param>
        /// <returns></returns>
        string GetScanRfid(string locNo, ref string errmsg);

        /// <summary>
        /// 获取站台出入库模式状态
        /// </summary>
        /// <param name="locNo">站台号</param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        string GetLocInOutSwitch(string locNo, ref string errmsg);

        /// <summary>
        /// 2020.11.09 新增 获取站台对应堆垛机的最近任务类型
        /// </summary>
        /// <param name="locPlcNo"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        string GetElocSrmInfo(string locPlcNo, ref string errmsg);

        /// <summary>
        /// 2021.06.28 新增 获取站台对应堆垛机的当前任务类型
        /// </summary>
        /// <param name="locPlcNo"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        string GetSrmTaskType(string locPlcNo, ref string errmsg);

        /// <summary>
        /// 获取入库站台对应堆垛机的任务信息
        /// </summary>
        /// <param name="locPlcNo"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        DataTable GetElocSrmTaskInfo(string locPlcNo, ref string errmsg);

        /// <summary>
        /// 记录站台托盘数量
        /// </summary>
        /// <param name="locNo"></param>
        /// <param name="palletAmount"></param>
        /// <returns></returns>
        bool RecordOtherAdded(string locNo, string palletAmount);
    }
}
