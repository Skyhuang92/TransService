using System;
using System.Collections.Generic;
using System.Text;
using WCS.Entity;

namespace WCS.Biz.PdIn
{
    public class RequestTask : IBiz
    {
        private BizHandle bizHandle
        {
            get;
            set;
        }

        public RequestTask()
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
                if (string.IsNullOrEmpty(plcStatus.PalletNo))
                {
                    bizHandle.ShowErrorLog(loc, "下位机未传递工装编号");
                    return;
                }
                loc.ScanRfidNo = plcStatus.PalletNo;
                if (bizHandle.GetTaskCmdBySlocNoAndPalletNo(loc))
                {
                    return;
                }
                loc.BizStep = BizStatus.RequestTask;
            }

            if (loc.BizStep == BizStatus.RequestTask)
            {
                loc.ReqOrderTypeNo = "100064";
                if (!bizHandle.CreateTask(loc))
                {
                    return;
                }
                loc.BizStep = BizStatus.WriteTaskCmd;
            }

            if (loc.BizStep == BizStatus.WriteTaskCmd)
            {
                if (!bizHandle.SendTaskNoToPlc(loc))
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
    }
}
