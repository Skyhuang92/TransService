using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCS.Entity;

namespace WCS.Biz.TransOut
{
    public class FinishTaskAndInOutSwitch : IBiz
    {
        private BizHandle bizHandle
        {
            get;
            set;
        }

        public FinishTaskAndInOutSwitch()
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

            InOutSwitchHandle(currLoc);

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

        //2020.11.06 新增 适应宝通原材料库一层站台正反转切换
        private void InOutSwitchHandle(Trans loc)
        {
            //获取WMS设置的当前站台出入库状态
            var wmsInOutStatus = bizHandle.GetLocInOutSwitch(loc);

            if (wmsInOutStatus == -1)
            {
                return;
            }

            var plcStatus = loc.PlcStatusRead as TransStatusRead;

            //判断是否与机台当前模式一致
            if (plcStatus.StatusInOutSwitch != wmsInOutStatus)
            {
                //切换出入库模式
                loc.InOutSwitch = wmsInOutStatus;
                bizHandle.SendInOutSwitchFlagToPlc(loc);

                var msg = "出入库模式切换为:入库";
                if (wmsInOutStatus == 1)
                    msg = "出入库模式切换为:出库";
                bizHandle.ShowExecLog(loc, msg);
            }
        }
    }
}
