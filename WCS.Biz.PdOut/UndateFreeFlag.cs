using System;
using System.Collections.Generic;
using System.Text;
using WCS.Entity;

namespace WCS.Biz.PdOut
{
    public class UndateFreeFlag : IBiz
    {
        private BizHandle bizHandle
        {
            get;
            set;
        }

        public UndateFreeFlag()
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
            loc.LocStatus = 0;
        }
    }
}
