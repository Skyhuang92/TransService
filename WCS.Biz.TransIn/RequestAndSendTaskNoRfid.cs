﻿using System;
using System.Collections.Generic;
using System.Text;
using WCS.Entity;

namespace WCS.Biz.TransIn
{
    public class RequestAndSendTaskNoRfid : IBiz
    {
        private BizHandle bizHandle
        {
            get;
            set;
        }

        public RequestAndSendTaskNoRfid()
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
            if (plcStatus.StatusRequest == 0)
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
                if (bizHandle.GetTaskCmdBySlocNo(loc))
                {
                    loc.BizStep = BizStatus.UpdateCmdStep;
                }
                else
                {
                    loc.BizStep = BizStatus.RequestTask;
                }
            }

            if (loc.BizStep == BizStatus.RequestTask)
            {
                loc.ReqOrderTypeNo = "100064";
                if (!bizHandle.CreateTask(loc))
                {
                    return;
                }
                loc.BizStep = BizStatus.RequestCmd;
            }

            if (loc.BizStep == BizStatus.RequestCmd)
            {
                if (!bizHandle.CreateCmd(loc))
                {
                    return;
                }
                loc.BizStep = BizStatus.GetTaskCmd;
            }

            if (loc.BizStep == BizStatus.GetTaskCmd)
            {
                if (!bizHandle.GetTaskCmdBySlocNoAndTaskNo(loc))
                {
                    return;
                }
                loc.BizStep = BizStatus.UpdateCmdStep;
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
