using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using SRBL.McLanguage;
using WCS.DbClient;
using WCS.Entity;
using WCS.OpcHelper;

namespace WCS.Biz
{
    public class BizHandle
    {
        private IDbClient dbHelper;
        private IOpcHelper opcHelper;

        #region 单例
        private static BizHandle _this = null;
        public static BizHandle Instance
        {                 
            get
            {
                if (_this == null)
                {
                    lock (typeof(BizHandle))
                    {
                        if (_this == null)
                        {
                            _this = new BizHandle();
                        }
                    }
                }
                return _this;
            }
        }
        private BizHandle()
        {
            dbHelper = DbClientFactory.Create();
            opcHelper = OpcClientFactory.Create();
        }
        #endregion

        public bool CheckDbConnection()
        {
            var errmsg = string.Empty;
            bool result;
            if (dbHelper != null && dbHelper.GetDbTime(ref errmsg))
            {
                ShowInitLog("Y", InfoType.dbConn);
                result = true;
            }
            else
            {
                ShowInitLog("N", InfoType.dbConn);
                return false;
            }
            return result;
        }

        public Dictionary<string, Loc> GetLocDic(string bizArea)
        {
            var errmsg = string.Empty;
            var locDic = dbHelper.GetLocData(bizArea, ref errmsg);
            if (locDic == null || locDic.Count <= 0)
            {
                ShowInitLog(Lan.Info("GetLocFail") + errmsg);
                locDic = null;
            }
            return locDic;
        }

        public Dictionary<string, OpcItem> GetOpcItemDic(Loc loc)
        {
            var errmsg = string.Empty;
            var opcItemDic = dbHelper.GetOpcItemsData(loc.LocNo, ref errmsg);
            if (opcItemDic == null || opcItemDic.Count <= 0)
            {
                string[] msg = 
                { 
                    loc.LocPlcNo, 
                    errmsg 
                };
                ShowInitLog(Lan.Info("GetOPCItemsFail", msg));
                opcItemDic = null;
            }
            return opcItemDic;
        }
        
        /// <summary>
        /// 记录站台状态
        /// </summary>
        public bool RecordLocStatus(Loc loc)
        {
            try
            {
                var plcStatus = loc.PlcStatusRead as TransStatusRead;
                var errmsg = string.Empty;
                var result = dbHelper.RecordLocStatus(loc.LocPlcNo, plcStatus, ref errmsg);
                if(!result)
                    ShowErrorLog(loc, Lan.Info("SavePLCStatusFail", errmsg));
                return result;
            }
            catch(Exception ex)
            {
                ShowErrorLog(loc, Lan.Info("SavePLCStatusAbnormal", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 记录站台状态
        /// </summary>
        public bool RecordBPalletLocStatus(Loc loc)
        {
            try
            {
                var plcStatus = loc.PlcStatusRead as TransStatusRead;
                var errmsg = string.Empty;
                var result = dbHelper.RecordBPalletLocStatus(loc.LocNo, plcStatus, ref errmsg);
                if (!result)
                    ShowErrorLog(loc, Lan.Info("SavePLCStatusFail", errmsg));
                return result;
            }
            catch (Exception ex)
            {
                ShowErrorLog(loc, Lan.Info("SavePLCStatusAbnormal", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 记录站台状态
        /// </summary>
        public string GetRecordBPalletLoc(Loc loc)
        {
            try
            {              
                var errmsg = string.Empty;
                var result = dbHelper.GetRecordBPalletLoc(loc.LocNo, ref errmsg);
                if (string.IsNullOrEmpty(result))
                    ShowErrorLog(loc, Lan.Info("GetLocRecordPalletFail", errmsg));
                return result;
            }
            catch (Exception ex)
            {
                ShowErrorLog(loc, Lan.Info("GetLocRecordPalletAbnormal", ex.Message));
                return string.Empty;
            }
        }

        /// <summary>
        /// 校验站台状态是否可以发起请求
        /// </summary>
        public bool CheckPlcStatusPreRequest(Loc loc)
        {
            var plcStatus = loc.PlcStatusRead as TransStatusRead;
            if (plcStatus.StatusFault == 1)
            {
                ShowErrorLog(loc, Lan.Info("EquipFault"));
                return false;
            }
            if (plcStatus.StatusAuto == 0)
            {
                ShowErrorLog(loc, Lan.Info("EquipManual"), 3);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 通过站台号获取指令信息
        /// </summary>
        public bool GetTaskCmdBySlocNo(Trans loc)
        {
            try
            {
                var parameter = new TaskCmd();
                parameter.SlocNo = loc.LocNo;
                parameter.CmdStep = "00";
                var errmsg = string.Empty;
                var taskcmd = dbHelper.GetTaskCmd(parameter, ref errmsg);
                if (taskcmd == null)
                {
                    ShowErrorLog(loc, Lan.Info("GetTaskCmdFail") + errmsg);
                    return false;
                }
                loc.TaskCmd = taskcmd;
                var msg = taskCmdString(loc.TaskCmd);
                ShowLocCmd(loc, msg);
                return true;
            }
            catch (Exception ex)
            {
                ShowErrorLog(loc, Lan.Info("GetTaskCmdAbnormal") + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 通过工装号获取指令
        /// </summary>
        public bool GetTaskCmdBySlocNoAndPalletNo(Trans loc)
        {
            try
            {
                var parameter = new TaskCmd();
                parameter.SlocNo = loc.LocNo;
                parameter.PalletNo = loc.ScanRfidNo;
                parameter.CmdStep = "00";
                var errmsg = string.Empty;
                var taskcmd = dbHelper.GetTaskCmd(parameter, ref errmsg);
                if (taskcmd == null)
                {
                    ShowErrorLog(loc, Lan.Info("GetTaskCmdFail") + errmsg);
                    return false;
                }
                loc.TaskCmd = taskcmd;
                var msg = taskCmdString(loc.TaskCmd);
                ShowLocCmd(loc, msg);
                return true;
            }
            catch (Exception ex)
            {
                ShowErrorLog(loc, Lan.Info("GetTaskCmdAbnormal") + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 通过工装号获取指令
        /// </summary>
        public bool GetTaskCmdBySlocNoAndTaskNo(Trans loc)
        {
            try
            {
                var parameter = new TaskCmd();
                parameter.SlocNo = loc.LocNo;
                parameter.TaskNo = loc.TaskNo;
                parameter.CmdStep = "00";
                var errmsg = string.Empty;
                var taskcmd = dbHelper.GetTaskCmd(parameter, ref errmsg);
                if (taskcmd == null)
                {
                    ShowErrorLog(loc, Lan.Info("GetTaskCmdFail") + errmsg);
                    return false;
                }
                loc.TaskCmd = taskcmd;
                var msg = taskCmdString(loc.TaskCmd);
                ShowLocCmd(loc, msg);
                return true;
            }
            catch (Exception ex)
            {
                ShowErrorLog(loc, Lan.Info("GetTaskCmdAbnormal") + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 通过目的位置获取指令
        /// </summary>
        public bool GetTaskCmdByByElocNoAndTaskNo(Trans loc)
        {
            try
            {
                var parameter = new TaskCmd();
                parameter.ElocNo = loc.LocNo;
                parameter.TaskNo = loc.TaskNo;
                var errmsg = string.Empty;
                var taskcmd = dbHelper.GetTaskCmd(parameter, ref errmsg);
                if (taskcmd == null)
                {
                    ShowErrorLog(loc, Lan.Info("GetTaskCmdFail") + errmsg);
                    return false;
                }
                loc.TaskCmd = taskcmd;
                var msg = taskCmdString(loc.TaskCmd);
                ShowLocCmd(loc, msg);
                return true;
            }
            catch (Exception ex)
            {
                ShowErrorLog(loc, Lan.Info("GetTaskCmdAbnormal") + ex.Message);
                return false;
            }
        }

        private string taskCmdString(TaskCmd taskCmd)
        {
            var msg = Lan.Info("TaskCmd");
            msg += Environment.NewLine;
            msg += Lan.Info("TaskNo") + taskCmd.TaskNo;
            msg += Environment.NewLine;
            msg += Lan.Info("RfidNo") + taskCmd.PalletNo;
            msg += Environment.NewLine;
            msg += Lan.Info("SlocPlcNo") + taskCmd.SlocPlcNo;
            msg += Environment.NewLine;
            msg += Lan.Info("ElocPlcNo") + taskCmd.ElocPlcNo;
            return msg;
        }

        /// <summary>
        /// 修改指令信息步骤
        /// </summary>
        public bool UpdateCmdStep(Trans loc)
        {
            try
            {
                var errmsg = string.Empty;
                var result = dbHelper.UpdateCmdStep(loc.TaskCmd.Objid, "02", ref errmsg);
                if (result)
                {
                    ShowLocCmd(loc, Lan.Info("UpdateCmdStepSuccess", loc.TaskCmd.Objid));
                }
                else
                {
                    string[] msg =
                    {
                        loc.TaskCmd.Objid.ToString(),
                        errmsg
                    };
                    ShowErrorLog(loc, Lan.Info("UpdateCmdStepFail", msg));
                }
                return result;
            }
            catch (Exception ex)
            {
                ShowErrorLog(loc, Lan.Info("UpdateCmdStepAbnormal") + ex.Message);
                return false;
            }
        }

        private long getObjidForRequestTask(Trans loc)
        {
            var objid = loc.ReqTaskObjid;
            if (objid <= 0)
            {
                var errmsg = string.Empty;
                objid = dbHelper.GetObjidForReqTask(ref errmsg);
                if (objid > 0)
                {
                    ShowExecLog(loc, Lan.Info("GetRequestTaskObjidSuccess", objid));
                }
                else
                {
                    ShowErrorLog(loc, Lan.Info("GetRequestTaskObjidFail") + errmsg);
                }
            }
            return objid;
        }

        /// <summary>
        /// 绑定空盘信息
        /// </summary>
        public bool CreateProduct(Trans loc)
        {
            var errmsg = string.Empty;
            var result = dbHelper.CreateBPalletProduct(loc.ScanRfidNo, loc.PalletAmount, ref errmsg);
            if (result)
            {
                ShowExecLog(loc, Lan.Info("CreateBPalletProductSuccess"));
            }
            else
            {
                ShowErrorLog(loc, Lan.Info("CreateBPalletProductFail") + errmsg);
            }

            return result;
        }


        /// <summary>
        /// 请求生成任务
        /// </summary>
        public bool CreateTask(Trans loc)
        {
            var result = false;
            loc.ReqTaskObjid = getObjidForRequestTask(loc);
            if (loc.ReqTaskObjid > 0)
            {
                var parameter = new ParameterForReqTask();
                parameter.Objid = loc.ReqTaskObjid;
                parameter.OrderTypeNo = loc.ReqOrderTypeNo;
                parameter.SlocNo = loc.LocNo;
                parameter.ElocNo = "00000000";
                parameter.PalletNo = loc.ScanRfidNo;
                parameter.SourceType = 3;
                var errmsg = string.Empty;
                var taskNo = dbHelper.CreateTask(parameter, ref errmsg);
                if (taskNo > 0)
                {
                    loc.TaskNo = taskNo;
                    result = true;
                    ShowExecLog(loc, Lan.Info("RequestTaskSuccess", taskNo));
                }
                else
                {
                    ShowErrorLog(loc, Lan.Info("RequestTaskFail") + errmsg);
                }
            }
            return result;
        }

        private long getObjidForRequestCmd(Trans loc)
        {
            var objid = loc.ReqCmdObjid;
            if (objid <= 0)
            {
                var errmsg = string.Empty;
                objid = dbHelper.GetObjidForReqCmd(ref errmsg);
                if (objid > 0)
                {
                    ShowExecLog(loc, Lan.Info("GetRequestCmdObjidSuccess", objid));
                }
                else
                {
                    ShowErrorLog(loc, Lan.Info("GetRequestCmdObjidFail") + errmsg);
                }
            }
            return objid;
        }

        /// <summary>
        /// 请求生成指令
        /// </summary>
        public bool CreateCmd(Trans loc)
        {
            var result = false;
            loc.ReqCmdObjid = getObjidForRequestCmd(loc);
            if (loc.ReqCmdObjid > 0)
            {
                var parameter = new ParameterForReqCmd();
                parameter.Objid = loc.ReqCmdObjid;
                parameter.CurrLocNo = loc.LocNo;
                parameter.TaskNo = loc.TaskNo;
                var errmsg = string.Empty;
                var cmdObjid = dbHelper.CreateCmd(parameter, ref errmsg);
                if (cmdObjid > 0)
                {
                    result = true;
                    ShowExecLog(loc, Lan.Info("RequestCmdSuccess", cmdObjid));
                }
                else
                {
                    ShowErrorLog(loc, Lan.Info("RequestCmdFail") + errmsg);
                }
            }
            return result;
        }

        private long getObjidForFinishCmd(Trans loc)
        {
            var objid = loc.FinishObjid;
            if (objid <= 0)
            {
                var errmsg = string.Empty;
                objid = dbHelper.GetObjidForFinishCmd(ref errmsg);
                if (objid > 0)
                {
                    ShowExecLog(loc, Lan.Info("GetFinishCmdObjidSuccess", objid));
                }
                else
                {
                    ShowErrorLog(loc, Lan.Info("GetFinishCmdObjidFail") + errmsg);
                }
            }
            return objid;
        }

        /// <summary>
        /// 结束指令
        /// </summary>
        public bool FinishTaskCmd(Trans loc)
        {
            var result = false;
            loc.FinishObjid = getObjidForFinishCmd(loc);
            if (loc.FinishObjid > 0)
            {
                var parameter = new ParameterForFinishCmd();
                parameter.Objid = loc.FinishObjid;
                parameter.CurrLocNo = loc.LocNo;
                parameter.TaskNo = loc.TaskNo;
                parameter.FinishFlag = 1;
                var errmsg = string.Empty;
                result = dbHelper.FinishTaskCmd(parameter, ref errmsg);
                if (result)
                {
                    ShowLocCmd(loc, Lan.Info("FinishCmdSuccess"));
                }
                else
                {
                    ShowErrorLog(loc, Lan.Info("FinishCmdFail") + errmsg);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取ScanRfid
        /// </summary>
        public bool GetScanRfid(Trans loc)
        {
            var errmsg = string.Empty;
            var scanRfid = dbHelper.GetScanRfid(loc.LocNo, ref errmsg);
            if (string.IsNullOrEmpty(scanRfid))
            {
                ShowErrorLog(loc, Lan.Info("GetRfidNoFail") + errmsg);
                return false;
            }
            ShowExecLog(loc, Lan.Info("GetRfidNoSuccess", scanRfid));
            loc.ScanRfidNo = scanRfid;
            return true;
        }

        public List<TaskCmd> GetTaskCmds(string locNo)
        {
            var taskCmd = new TaskCmd();
            taskCmd.SlocNo = locNo;
            var errmsg = string.Empty;
            return dbHelper.GetTaskCmds(taskCmd, ref errmsg);
        }

        /// <summary>
        /// 处理任务信息
        /// </summary>
        public bool HandleTaskCmd(long taskNo, int dealFlag, ref string errmsg)
        {
            return dbHelper.HandleTaskCmd(taskNo, dealFlag, ref errmsg);
        }

        /// <summary>
        /// 2020.11.06 新增 获取WMS设置的站台出入库模式
        /// </summary>
        public int GetLocInOutSwitch(Trans loc)
        {
            try
            {
                var errmsg = string.Empty;

                var inOutStatus = dbHelper.GetLocInOutSwitch(loc.LocNo, ref errmsg);
                if (string.IsNullOrEmpty(inOutStatus))
                {
                    ShowErrorLog(loc, "获取当前站台的出入库模式异常:" + errmsg);
                    return -1;
                }

                //判断机台业务状态
                if (inOutStatus == "I" || inOutStatus == "O")
                {
                    if (inOutStatus == "I")
                        return 0;
                    else
                        return 1;
                }
                else
                {
                    ShowErrorLog(loc, "获取未识别的出入库模式" + errmsg);
                    return -1;
                }
            }
            catch (Exception ex)
            {
                ShowErrorLog(loc, "获取WMS设置的站台出入库模式时发生异常：" + ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// 2020.11.09 判断是否可以放行
        /// </summary>
        public bool PalletPassCheck(Trans loc)
        {
            try
            {
                var errmsg = string.Empty;

                var type = dbHelper.GetElocSrmInfo(loc.TaskCmd.ElocPlcNo, ref errmsg);
                if (type == null)
                {
                    ShowErrorLog(loc, "获取站台【" + loc.TaskCmd.ElocPlcNo + "】对应堆垛机的最近任务类型失败：" + errmsg);
                    return false;
                }

                errmsg = string.Empty;

                //获取堆垛机当前出库任务的执行状态
                var dt = dbHelper.GetElocSrmTaskInfo(loc.TaskCmd.ElocPlcNo, ref errmsg);
                if (!string.IsNullOrEmpty(errmsg))
                {
                    ShowErrorLog(loc, "获取站台【" + loc.TaskCmd.ElocPlcNo + "】对应堆垛机当前出库任务的执行状态时发生异常：" + errmsg);
                    return false;
                }

                //堆垛机状态分析
                var taskAmount = dt.Rows.Count;
                var taskStep = "";
                var taskCmd = "";
                var elocNo = "";

                if (taskAmount > 0)
                {
                    taskStep = dt.Rows[0]["task_step"].ToString();
                    taskCmd = dt.Rows[0]["objid"].ToString();
                    elocNo = dt.Rows[0]["eloc_no"].ToString();
                }

                //判断站台是否应该放行
                if (type == "I")
                {
                    //上一任务为入库，未生成指令和正在执行目的站台放货指令时，不允许放行
                    if (taskAmount > 0)
                    {
                        //存在未生成指令的出库任务
                        if (taskStep == "0000")
                        {
                            return false;
                        }
                        //存在往此站台放货的指令
                        if (!string.IsNullOrEmpty(taskCmd) && elocNo == loc.TaskCmd.ElocPlcNo)
                        {
                            return false;
                        }
                    }
                }
                if (type == "O")
                {
                    //存在往此站台放货的指令
                    if (!string.IsNullOrEmpty(taskCmd) && elocNo == loc.TaskCmd.ElocPlcNo)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowErrorLog(loc, "获取站台【" + loc.TaskCmd.ElocPlcNo + "】对应堆垛机当前出库任务的执行状态时发生异常：" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 2020.11.10 新增 H11、H12区 获取空盘收集工位托盘数量
        /// </summary>
        public bool RecordOtherAdded(Trans loc)
        {
            try
            {
                var errmsg = string.Empty;

                var result = dbHelper.RecordOtherAdded(loc.LocNo, loc.PalletAmount.ToString());

                if (!result)
                {
                    ShowErrorLog(loc, $"记录站台状态失败：{errmsg}");
                }

                return result;
            }
            catch (Exception ex)
            {
                ShowErrorLog(loc, "记录站台状态时发生异常：" + ex.Message);
                return false;
            }
        }


        public bool OpcConnection(string[] opcStringItems)
        {
            var errmsg = string.Empty;
            var result = opcHelper.OpcConnection(opcStringItems, ref errmsg);
            if (result)
            {
                ShowInitLog(Lan.Info("ConnectLocOPCServerSuccess"));
            }
            else
            {
                ShowInitLog(Lan.Info("ConnectLocOPCServerFail") + errmsg);
            }
            return result;
        }

        public bool GetOpcStatus(Loc loc)
        {
            var errmsg = string.Empty;
            var result = true;
            if (!opcHelper.GetLocPlcStatus(loc, ref errmsg))
            {
                loc.LocStatus = 2;
                loc.LocBizDesc = Lan.Info("GetOPCItemsValueFail") + errmsg;
                result = false;
            }
            ShowLocStatus(loc, "Fresh");
            return result;
        }

        /// <summary>
        /// 发送指令信息至PLC
        /// </summary>
        public bool SendInfoToPlc(Loc loc)
        {
            var errmsg = string.Empty;
            var result = opcHelper.SendInfoToPlc(loc, ref errmsg);
            if (result)
            {
                ShowExecLog(loc, Lan.Info("DownTaskCmdSuccess"));
            }
            else
            {
                ShowErrorLog(loc, Lan.Info("DownTaskCmdFail") + errmsg);
            }
            return result;
        }

        /// <summary>
        /// 清除发送至PLC的指令信息
        /// </summary>
        public bool ClearInfoToPlc(Loc loc)
        {
            var errmsg = string.Empty;
            var result = opcHelper.ClearInfoToPlc(loc, ref errmsg);
            if (result)
            {
                ShowExecLog(loc, Lan.Info("ClearTaskCmdSuccess"));
            }
            else
            {
                ShowErrorLog(loc, Lan.Info("ClearTaskCmdFail") + errmsg);
            }
            return result;
        }

        /// <summary>
        /// 发送故障号至PLC
        /// </summary>
        public bool SendFaultNoToPlc(Loc loc)
        {
            var errmsg = string.Empty;
            var result = opcHelper.SendFaultNoToPlc(loc, ref errmsg);
            if (result)
            {
                ShowExecLog(loc, Lan.Info("DownFaultNoSuccess", loc.FaultNo));
            }
            else
            {
                string[] msg =
                {
                    loc.FaultNo.ToString(), 
                    errmsg
                };
                ShowErrorLog(loc, Lan.Info("DownFaultNoFail", msg));
            }
            return result;
        }

        /// <summary>
        /// 发送任务号至PLC
        /// </summary>
        public bool SendTaskNoToPlc(Trans loc)
        {
            var errmsg = string.Empty;
            var result = opcHelper.SendTaskNoToPlc(loc, ref errmsg);
            if (result)
            {
                ShowExecLog(loc, Lan.Info("DownTaskNoSuccess", loc.TaskNo));
            }
            else
            {
                string[] msg =
                {
                    loc.TaskNo.ToString(), 
                    errmsg
                };
                ShowErrorLog(loc, Lan.Info("DownTaskNoFail",msg));
            }
            return result;
        }

        /// <summary>
        /// 发送任务已处理至PLC
        /// </summary>
        public bool SendHandleRequestFlagToPlc(Loc loc)
        {
            var errmsg = string.Empty;
            var result = opcHelper.SendHandleRequestFlagToPlc(loc, ref errmsg);
            if (result)
            {
                ShowExecLog(loc, Lan.Info("DownHandleFlagSuccess"));
            }
            else
            {
                ShowErrorLog(loc, Lan.Info("DownHandleFlagFail") + errmsg);
            }
            return result;
        }

        /// <summary>
        /// 发送取货已处理至PLC
        /// </summary>
        public bool SendHandlePickFlagToPlc(Loc loc)
        {
            var errmsg = string.Empty;
            var result = opcHelper.SendHandlePickFlagToPlc(loc, ref errmsg);
            if (result)
            {
                ShowExecLog(loc, Lan.Info("DownPickFlagSuccess"));
            }
            else
            {
                ShowErrorLog(loc, Lan.Info("DownPickFlagFail") + errmsg);
            }
            return result;
        }

        /// <summary>
        /// 发送状态切换信号至PLC
        /// </summary>
        public bool SendInOutSwitchFlagToPlc(Loc loc)
        {
            var errmsg = string.Empty;
            var result = opcHelper.SendInOutSwitchFlagToPlc(loc, ref errmsg);
            if (result)
            {
                ShowExecLog(loc, "写入PLC信号切换标志成功！");
            }
            else
            {
                ShowErrorLog(loc, "写入PLC状态切换标志失败:" + errmsg);
            }
            return result;
        }

        /// <summary>
        /// 初始化日志展示
        /// </summary>
        public void ShowInitLog(string msg, InfoType infoType = InfoType.logInfo)
        {
            var showInfoData = new ShowInfoData(msg, "", infoType);
            ShowFormData.Instance.ShowFormInfo(showInfoData);
        }

        /// <summary>
        /// 站台状态显示
        /// </summary>
        public void ShowLocStatus(Loc loc, string msg)
        {
            msg = "[" + loc.LocPlcNo + "]" + msg;
            var showInfoData = new ShowInfoData(msg, loc.LocNo, InfoType.locStatus);
            ShowFormData.Instance.ShowFormInfo(showInfoData);
        }

        /// <summary>
        /// 站台运行日志显示
        /// </summary>
        public void ShowExecLog(Loc loc, string msg)
        {
            loc.LocStatus = 1;
            loc.LocBizDesc = string.Empty;
            msg = "[" + loc.LocPlcNo + "]" + msg;
            var showInfoData = new ShowInfoData(msg, loc.LocNo, InfoType.logInfo);
            ShowFormData.Instance.ShowFormInfo(showInfoData);
        }

        /// <summary>
        /// 站台异常日志显示
        /// </summary>
        public void ShowErrorLog(Loc loc, string msg, int locStatus = 2)
        {
            loc.LocStatus = locStatus;
            loc.LocBizDesc = msg;
            msg = "[" + loc.LocPlcNo + "]" + msg;
            var showInfoData = new ShowInfoData(msg, loc.LocNo, InfoType.logInfo);
            ShowFormData.Instance.ShowFormInfo(showInfoData);
        }

        /// <summary>
        /// 站台指令显示
        /// </summary>
        public void ShowLocCmd(Loc loc, string msg)
        {
            loc.LocStatus = 1;
            loc.LocBizDesc = string.Empty;
            msg = "[" + loc.LocPlcNo + "]" + msg;
            var showInfoData = new ShowInfoData(msg, loc.LocNo, InfoType.taskCmd);
            ShowFormData.Instance.ShowFormInfo(showInfoData);
        }

        /// <summary>
        /// 从XML读取多语言配置
        /// </summary>
        /// <returns></returns>
        public bool CreateLanguage()
        {
            try
            {
                var xe = XElement.Load(@"Resource\AllLanguageList.xml");
                var elements = from ele in xe.Elements()
                               select ele;
                var xmlNameList = new List<string>();
                foreach (var element in elements)
                {
                    var xmlName = element.Value;
                    if (!string.IsNullOrEmpty(xmlName))
                    {
                        xmlNameList.Add(xmlName);
                    }
                }
                foreach(var xmlName in xmlNameList)
                {
                    var xElement = XElement.Load(@"Resource\" + xmlName + ".xml");
                    var logInfos = from el in xElement.Elements()
                                   select el;
                    var language = xmlName;
                    foreach (var logInfo in logInfos)
                    {
                        var key = logInfo.Attribute("name").Value;
                        var value = logInfo.Value;
                        Manager.Instance.AddLanguage(language, key, value);
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                ShowInitLog("获取多语言资源失败：" + ex.Message);
                return false;
            }
        }


        /// <summary>
        /// 获取站台最近堆垛机当前任务类型
        /// </summary>
        public string GetSrmTaskType(string locNo)
        {
            try
            {
                var errmsg = string.Empty;
                var result = dbHelper.GetSrmTaskType(locNo, ref errmsg);
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 发送堆垛机状态至PLC（使用备用字段）
        /// </summary>
        public bool SendSpareToPlc(Trans loc, int? status = 0)
        {
            var errmsg = string.Empty;
            var result = opcHelper.SendSpareToPlc(loc, ref errmsg, status);
            return result;
        }

    }
}
