using System;
using System.Collections.Generic;
using System.Text;
using WCS.Entity;

namespace WCS.Biz.TransOut
{
    public class FinishOrSendTaskNoRfid : IBiz
    {
        private BizHandle bizHandle
        {
            get;
            set;
        }

        public FinishOrSendTaskNoRfid()
        {
            bizHandle = BizHandle.Instance;
        }

        public void HandleLoc(Loc loc)
        {
            var currLoc = loc as Trans;
            if (!bizHandle.RecordLocStatus(currLoc))
            {
                return;
            }
            if(!bizHandle.CheckPlcStatusPreRequest(currLoc))
            {
                return;
            }

            var plcStatus = currLoc.PlcStatusRead as TransStatusRead;
            if (plcStatus.StatusRequest == 0 && plcStatus.StatusNeedToPut == 0)
            {
                if (currLoc.BizStep != BizStatus.None)
                {
                    bizHandle.ClearInfoToPlc(currLoc);                  
                    currLoc.BizStep = BizStatus.None;
                }
                currLoc.InitLoc();
                return;
            }
            if (plcStatus.StatusRequest == 1 && plcStatus.StatusNeedToPut == 1)
            {
                bizHandle.ShowErrorLog(currLoc, "下位机信号异常,WCS无法确定业务类型！");
                return;
            }

            if (plcStatus.StatusRequest == 1)
            {
                ExecuteRequestData(currLoc);
            }
            else if (plcStatus.StatusNeedToPut == 1)
            {
                ExecuteFinishData(currLoc);
            }
        }

        private void ExecuteFinishData(Trans loc)
        {
            var plcStatus = loc.PlcStatusRead as TransStatusRead;
            if (loc.BizStep == BizStatus.None)
            {
                if (plcStatus.TaskNo == 0)
                {
                    bizHandle.ShowErrorLog(loc, "下位机未传递任务流水号");
                    return;
                }
                loc.TaskNo = plcStatus.TaskNo;
                var msg = "下位机传递信息：";
                msg += Environment.NewLine;
                msg += "任务编号 = " + plcStatus.TaskNo;
                msg += Environment.NewLine;
                msg += "工装编号 = " + plcStatus.PalletNo;
                bizHandle.ShowExecLog(loc, msg);
                loc.BizStep = BizStatus.FinishTaskCmd;
            }

            if (loc.BizStep == BizStatus.FinishTaskCmd)
            {
                if (!bizHandle.FinishTaskCmd(loc))
                {
                    return;
                }
                loc.HandlePickFlag = 1;
                loc.BizStep = BizStatus.WritePickDeal;
            }

            if (loc.BizStep == BizStatus.WritePickDeal)
            {
                if (!bizHandle.SendHandlePickFlagToPlc(loc))
                {
                    return;
                }
                loc.BizStep = BizStatus.ResetLocStatus;
            }

            if (loc.BizStep == BizStatus.ResetLocStatus)
            {
                loc.InitLoc();
            }
        }

        private void ExecuteRequestData(Trans loc)
        {
            var plcStatus = loc.PlcStatusRead as TransStatusRead;
            if (loc.BizStep == BizStatus.None)
            {
                if (bizHandle.GetTaskCmdBySlocNo(loc))
                {
                    loc.BizStep = BizStatus.UpdateCmdStep;
                }
                else
                {
                    bizHandle.ShowExecLog(loc, "未找到站台的指令信息");
                    return;
                }
            }

            if (loc.BizStep == BizStatus.UpdateCmdStep)
            {
                if (!bizHandle.UpdateCmdStep(loc))
                {
                    return;
                }
                loc.BizStep = BizStatus.WriteTaskCmd;
            }

            if (loc.BizStep == BizStatus.WriteTaskCmd)
            {
                if (!bizHandle.SendInfoToPlc(loc))
                {
                    return;
                }
                loc.HandleRequetFlag = 1;
                loc.BizStep = BizStatus.WriteTaskDeal;
            }

            if (loc.BizStep == BizStatus.WriteTaskDeal)
            {
                if (!bizHandle.SendHandleRequestFlagToPlc(loc))
                {
                    return;
                }
                loc.BizStep = BizStatus.ResetLocStatus;
            }

            if (loc.BizStep == BizStatus.ResetLocStatus)
            {
                loc.InitLoc();
            }
        }
    }
}
