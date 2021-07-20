using System;
using System.Collections.Generic;
using System.Text;
using WCS.Entity;

namespace WCS.Biz.PdOut
{
    public class SendTask : IBiz
    {
        private BizHandle bizHandle
        {
            get;
            set;
        }

        public SendTask()
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
            if (plcStatus.StatusRequest != 1)
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
            if(loc.BizStep == BizStatus.None)
            {
                if (!bizHandle.GetTaskCmdBySlocNo(loc))
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
                var result = bizHandle.SendHandleRequestFlagToPlc(loc);
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
