using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCS.Entity;

namespace WCS.Biz.Cache
{
    public class UndateFreeFlag : IBiz
    {
        private BizHandle bizHandle;

        public UndateFreeFlag()
        {
            bizHandle = BizHandle.Instance; ;
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

            /// 2020.11.10 新增 H11、H12区 获取空盘收集工位托盘数量
            currLoc.PalletAmount = plcStatus.PalletAmount;
            bizHandle.RecordOtherAdded(currLoc);

            currLoc.InitLoc();
            currLoc.BizStep = BizStatus.None;
        }
    }
}
