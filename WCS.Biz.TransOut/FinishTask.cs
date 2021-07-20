using System;
using System.Collections.Generic;
using System.Text;
using WCS.Entity;
using WCS.Biz;

namespace WCS.Biz.TransOut
{
    public class FinishTask : IBiz
    {
        private BizHandle bizHandle
        {
            get;
            set;
        }

        public FinishTask()
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
            if (!bizHandle.CheckPlcStatusPreRequest(currLoc))
            {
                return;
            }

            var plcStatus = currLoc.PlcStatusRead as TransStatusRead;
            if (plcStatus.StatusNeedToPut == 0)
            {
                if (currLoc.BizStep != BizStatus.None)
                {
                    bizHandle.ClearInfoToPlc(currLoc);                    
                    currLoc.BizStep = BizStatus.None;
                }
                currLoc.InitLoc();
                return;
            }
            ExecuteReceiveData(currLoc);
        }

        private void ExecuteReceiveData(Trans loc)
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
                var result = bizHandle.SendHandlePickFlagToPlc(loc);
                if (!result)
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
