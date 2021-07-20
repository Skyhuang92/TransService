using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCS.Entity
{
    public class Trans : Loc
    {
        public Trans() : base()
        {
            this.TaskCmd = new TaskCmd();
            this.TaskCmdList = new List<TaskCmd>();
            PlcStatusRead = new TransStatusRead();
            PlcStatusWrite = new TransStatusWrite();
        }

        /// <summary>
        /// 当前指令信息
        /// </summary>
        public TaskCmd TaskCmd
        {
            get; 
            set;
        }

        /// <summary>
        /// 以当前站台为起点的所有指令信息
        /// 用于界面指令信息刷新
        /// </summary>
        public List<TaskCmd> TaskCmdList
        {
            get; 
            set;
        }

        /// <summary>
        /// 请求任务Objid
        /// </summary>
        public long ReqTaskObjid
        {
            get; set;
        }

        /// <summary>
        /// 请求指令Objid
        /// </summary>
        public long ReqCmdObjid
        {
            get; set;
        }

        /// <summary>
        /// 结束任务Objid
        /// </summary>
        public long FinishObjid
        {
            get; 
          
            set;
        }
        /// <summary>
        /// 请求单据类型编号
        /// </summary>
        public string ReqOrderTypeNo
        {
            get;
            set;
        }

        /// <summary>
        /// 任务处理标识
        /// </summary>
        public int HandleRequetFlag
        {
            get;
            set;
        }

        /// <summary>
        /// 取货处理标识
        /// </summary>
        public int HandlePickFlag
        {
            get;
            set;
        }

        /// <summary>
        /// 状态切换标志
        /// </summary>
        public int InOutSwitch
        {
            get;
            set;
        }

        /// <summary>
        /// 工装编号
        /// </summary>
        public string ScanRfidNo
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        /// 上位机生成的任务号
        /// </summary>
        public long TaskNo
        {
            get;
            set;
        }

        /// <summary>
        /// 托盘数量
        /// </summary>
        public int PalletAmount
        {
            get;
            set;
        }


        /// <summary>
        /// 站台信息复位
        /// </summary>
        public override void InitLoc()
        {
            base.LocStatus = 0;
            base.LocBizDesc = string.Empty;
            base.FaultNo = 0;

            this.ReqTaskObjid = 0;
            this.ReqCmdObjid = 0;
            this.FinishObjid = 0;
            this.TaskNo = 0;
            this.ReqOrderTypeNo = string.Empty;
            this.HandleRequetFlag = 0;
            this.HandlePickFlag = 0;
            this.TaskCmd = new TaskCmd();
        }

        public override void SetPlcChangeValue(string busIdentity, object value)
        {
            var readstatus = PlcStatusRead as TransStatusRead;
            var writestatus = PlcStatusWrite as TransStatusWrite;
            switch (busIdentity)
            {
                #region 绑定读取值
                case "Read.TaskNo":
                    readstatus.TaskNo = Convert.ToInt32(value);
                    break;
                case "Read.PalletNo":
                    readstatus.PalletNo = value.ToString().Trim();
                    break;
                case "Read.SlocArea":
                    readstatus.SlocArea = value.ToString().Trim();
                    break;
                case "Read.SlocCode":
                    readstatus.SlocCode = value.ToString().Trim();
                    break;
                case "Read.ElocArea":
                    readstatus.ElocArea = value.ToString().Trim();
                    break;
                case "Read.ElocCode":
                    readstatus.ElocCode = value.ToString().Trim();
                    break;
                case "Read.StatusAuto":
                    readstatus.StatusAuto = Convert.ToInt32(value);
                    break;
                case "Read.StatusFault":
                    readstatus.StatusFault = Convert.ToInt32(value);
                    break;
                case "Read.StatusLoad":
                    readstatus.StatusLoad = Convert.ToInt32(value);
                    break;
                case "Read.StatusRequest":
                    readstatus.StatusRequest = Convert.ToInt32(value);
                    break;
                case "Read.StatusFree":
                    readstatus.StatusFree = Convert.ToInt32(value);
                    break;
                case "Read.StatusNeedToPut":
                    readstatus.StatusNeedToPut = Convert.ToInt32(value);
                    break;
                case "Read.StatusInOutSwitch":
                    readstatus.StatusInOutSwitch = Convert.ToInt32(value);
                    break;
                case "Read.PalletAmount":
                    readstatus.PalletAmount = Convert.ToInt32(value);
                    break;
                #endregion

                #region 绑定写入项
                case "Write.TaskNo":
                    writestatus.TaskNo = Convert.ToInt32(value);
                    break;
                case "Write.PalletNo":
                    writestatus.PalletNo = value.ToString().Trim();
                    break;
                case "Write.SlocArea":
                    writestatus.SlocArea = value.ToString().Trim();
                    break;
                case "Write.SlocCode":
                    writestatus.SlocCode = value.ToString().Trim();
                    break;
                case "Write.ElocArea":
                    writestatus.ElocArea = value.ToString().Trim();
                    break;
                case "Write.ElocCode":
                    writestatus.ElocCode = value.ToString().Trim();
                    break;
                case "Write.HandleRequest":
                    writestatus.HandleRequest = Convert.ToInt32(value);
                    break;
                case "Write.HandleNeedToPut":
                    writestatus.HandleNeedToPut = Convert.ToInt32(value);
                    break;
                case "Write.FaultNo":
                    writestatus.FaultNo = value.ToString().Trim();
                    break;
                default:
                    break;
                #endregion
            }
        }

        /// <summary>
        /// 写信息
        /// </summary>
        public override List<KeyValuePair<string, object>> SendInfos(ref string errmsg)
        {
            var result = new List<KeyValuePair<string, object>>();
            try
            {
                foreach (var item in opcItemDic.Values.Where(p => p.BizIdentity.StartsWith("Write")))
                {
                    switch (item.BizIdentity)
                    {
                        case "Write.TaskNo":
                            result.Add(new KeyValuePair<string, object>(item.TagLongName, Convert.ToUInt32(this.TaskCmd.TaskNo)));
                            break;
                        case "Write.PalletNo":
                            result.Add(new KeyValuePair<string, object>(item.TagLongName, this.TaskCmd.PalletNo ?? string.Empty));
                            break;
                        case "Write.SlocArea":
                            result.Add(new KeyValuePair<string, object>(item.TagLongName, Convert.ToUInt16(this.TaskCmd.SlocArea)));
                            break;
                        case "Write.SlocCode":
                            result.Add(new KeyValuePair<string, object>(item.TagLongName, Convert.ToUInt16(this.TaskCmd.SlocCode)));
                            break;
                        case "Write.ElocArea":
                            result.Add(new KeyValuePair<string, object>(item.TagLongName, Convert.ToUInt16(this.TaskCmd.ElocArea)));
                            break;
                        case "Write.ElocCode":
                            result.Add(new KeyValuePair<string, object>(item.TagLongName, Convert.ToUInt16(this.TaskCmd.ElocCode)));
                            break;
                    }
                }
                
            }
            catch(Exception ex)
            {
                errmsg = ex.Message;
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 写状态切换信号
        /// </summary>
        public List<KeyValuePair<string, object>> SendInOutSwitchFlag(ref string errmsg)
        {
            var result = new List<KeyValuePair<string, object>>();
            var item = opcItemDic.Values.FirstOrDefault(p => p.BizIdentity.Equals("Write.InOutSwitch"));
            if (item != null)
            {
                result.Add(new KeyValuePair<string, object>(item.TagLongName, InOutSwitch));
            }
            return result;
        }

        /// <summary>
        /// 写任务已处理确认信号
        /// </summary>
        public List<KeyValuePair<string, object>> SendHandleRequestFlag(ref string errmsg)
        {
            var result = new List<KeyValuePair<string, object>>();
            try
            {
                var item = opcItemDic.Values.FirstOrDefault(p => p.BizIdentity.Equals("Write.HandleRequest"));
                if (item != null)
                {
                    result.Add(new KeyValuePair<string, object>(item.TagLongName, HandleRequetFlag));
                }
            }
            catch(Exception ex)
            {
                errmsg = ex.Message;
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 写任务已处理确认信号
        /// </summary>
        public List<KeyValuePair<string, object>> SendHandleNeedToPutFlag(ref string errmsg)
        {
            var result = new List<KeyValuePair<string, object>>();
            try
            {
                var item = opcItemDic.Values.FirstOrDefault(p => p.BizIdentity.Equals("Write.HandleNeedToPut"));
                if (item != null)
                {
                    result.Add(new KeyValuePair<string, object>(item.TagLongName, HandlePickFlag));
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                result = null;
            }
            return result;
        }

      

        /// <summary>
        /// 写任务号
        /// </summary>
        public List<KeyValuePair<string, object>> SendTaskNo(ref string errmsg)
        {
            var result = new List<KeyValuePair<string, object>>();
            try
            {
                var item = opcItemDic.Values.FirstOrDefault(p => p.BizIdentity.Equals("Write.TaskNo"));
                if (item != null)
                {
                    result.Add(new KeyValuePair<string, object>(item.TagLongName, Convert.ToUInt32(TaskNo)));
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 写信息
        /// </summary>
        public override List<KeyValuePair<string, object>> ClearInfos(ref string errmsg)
        {
            var result = new List<KeyValuePair<string, object>>();
            try
            {
                foreach (var item in opcItemDic.Values.Where(p => p.BizIdentity.StartsWith("Write")))
                {
                    switch (item.BizIdentity)
                    {
                        case "Write.TaskNo":
                            result.Add(new KeyValuePair<string, object>(item.TagLongName, 0));
                            break;
                        case "Write.PalletNo":
                            result.Add(new KeyValuePair<string, object>(item.TagLongName, string.Empty));
                            break;
                        case "Write.SlocArea":
                            result.Add(new KeyValuePair<string, object>(item.TagLongName, 0));
                            break;
                        case "Write.SlocCode":
                            result.Add(new KeyValuePair<string, object>(item.TagLongName, 0));
                            break;
                        case "Write.ElocArea":
                            result.Add(new KeyValuePair<string, object>(item.TagLongName, 0));
                            break;
                        case "Write.ElocCode":
                            result.Add(new KeyValuePair<string, object>(item.TagLongName, 0));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 写信号给站台最近堆垛机的当前出库任务类型
        /// </summary>
        public List<KeyValuePair<string, object>> SendSpareFlag(ref string errmsg,int? status = 0)
        {
            var result = new List<KeyValuePair<string, object>>();
            try
            {
                var item = opcItemDic.Values.FirstOrDefault(p => p.BizIdentity.Equals("Read.Spare"));
                if (item != null)
                {
                    result.Add(new KeyValuePair<string, object>(item.TagLongName, status));
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                result = null;
            }
            return result;
        }

    }
}
