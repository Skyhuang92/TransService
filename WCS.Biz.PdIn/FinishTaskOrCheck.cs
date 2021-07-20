using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCS.Entity;

namespace WCS.Biz.PdIn
{
    public class FinishTaskOrCheck : IBiz
    {
        private BizHandle bizHandle
        {
            get;
            set;
        }

        public FinishTaskOrCheck()
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
            if (plcStatus.StatusNeedToPut == 0 && plcStatus.StatusRequest == 0)
            {
                if (currLoc.BizStep != BizStatus.None)
                {
                    bizHandle.ClearInfoToPlc(currLoc);
                    currLoc.BizStep = BizStatus.None;
                }
                currLoc.InitLoc();
                return;
            }

            //到位信号处理2#堆垛机取货站台业务
            if (plcStatus.StatusNeedToPut == 1)
                ExecuteReceiveData(currLoc);

            //请求信号处理通道放行业务
            if (plcStatus.StatusRequest == 1)
                PalletPassCheck(currLoc);
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

        /// <summary>
        /// 2020.11.09 特殊业务处理 经过此站台的物料放行判断处理
        /// </summary>
        /// <param name="loc"></param>
        private void PalletPassCheck(Trans loc)
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
                loc.BizStep = BizStatus.Check;
            }

            if (loc.BizStep == BizStatus.Check)
            {
                //2021.03.03 设定站台读取任务信息
                loc.TaskCmd = new TaskCmd();
                loc.TaskCmd.TaskNo = plcStatus.TaskNo;
                loc.TaskCmd.ElocPlcNo = plcStatus.ElocPlcNo;

                if (!bizHandle.PalletPassCheck(loc))
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
