using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCS.Entity;

namespace WCS.Biz.BPallet
{
    public class RequestAndSendTask : IBiz
    {
        private BizHandle bizHandle
        {
            get;
            set;
        }

        public RequestAndSendTask()
        {
            bizHandle = BizHandle.Instance;
        }

        public void HandleLoc(Loc loc)
        {
            var currLoc = loc as Trans;
            //更新站台PLC状态 排除托盘号 空盘收集工位无法获取托盘号 手持创建任务时更新
            if (!bizHandle.RecordBPalletLocStatus(currLoc))
            {
                return;
            }
            
            if (!bizHandle.CheckPlcStatusPreRequest(currLoc))
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
                loc.ScanRfidNo = bizHandle.GetRecordBPalletLoc(loc);
                loc.PalletAmount = plcStatus.PalletAmount;
                if (string.IsNullOrEmpty(plcStatus.PalletNo))
                {                   
                    return;
                }
              
                if (bizHandle.GetTaskCmdBySlocNoAndPalletNo(loc))
                {
                    loc.BizStep = BizStatus.WriteTaskCmd;
                }
                else
                {
                    loc.BizStep = BizStatus.CreateProduct;
                }
            }

            if(loc.BizStep == BizStatus.CreateProduct)
            {
                if (!bizHandle.CreateProduct(loc))
                {
                    return;
                }
                loc.BizStep = BizStatus.RequestTask;
            }

            if (loc.BizStep == BizStatus.RequestTask)
            {
                loc.ReqOrderTypeNo = "100062";
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
                if (!bizHandle.GetTaskCmdBySlocNoAndPalletNo(loc))
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
