using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using SRBL.McLanguage;
using WCS.Entity;

namespace WCS.Biz
{
    public class BizInit
    {
        public Dictionary<string, Loc> locDic;
        private BizHandle bizHandle;
        private string locArea = McConfig.Instance.LocArea;
        private string locHost = McConfig.Instance.LocHost;
        
        public BizInit()
        {
            locDic = new Dictionary<string, Loc>();
            bizHandle = BizHandle.Instance;
        }

        public bool InitLocData()
        {
            if (!bizHandle.CreateLanguage())
            {
                return false;
            }
            if (!bizHandle.CheckDbConnection())
            {
                return false;
            }
            locDic = bizHandle.GetLocDic(locArea);
            if (locDic == null)
            {
                return false;
            }
            foreach(Loc loc in locDic.Values)
            {
                loc.opcItemDic = bizHandle.GetOpcItemDic(loc);
                if(loc.opcItemDic == null)
                {
                    return false;
                }
                loc.Biz = CreateBiz(loc.BizType);
                if(loc.Biz == null)
                {
                    bizHandle.ShowErrorLog(loc, Lan.Info("CreateBizClassFail"));
                    return false;
                }
            }
            bizHandle.ShowInitLog(Lan.Info("CreateLocObjectSuccess"));
            return true;
        }

        public bool InitLocOpc()
        {
            var opcStringItems = new List<string>();
            foreach (Loc loc in locDic.Values)
            {
                foreach(string item in loc.opcItemDic.Keys)
                {
                    opcStringItems.Add(item);
                }
            }
            bizHandle.ShowInitLog(string.Format(Lan.Info("OPCItemsCount"), opcStringItems.Count));
            return bizHandle.OpcConnection(opcStringItems.ToArray());
        }

        private IBiz CreateBiz(string bizName)
        {
            try
            {
                var type = System.Type.GetType(bizName);
                if (type == null)
                {
                    return null;
                }
                var biz = Activator.CreateInstance(type) as IBiz;
                return biz;
            }
            catch
            {
                return null;
            }
        }

        public bool PingIP(string locIP)
        {
            try
            {
                var pr = new Ping().Send(locIP, 3000);
                if (pr == null)
                {
                    return false;
                }
                return pr.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }

        public string GetSrmTaskType(string locplcno)
        {
            try
            {
                var type = bizHandle.GetSrmTaskType(locplcno);
                return type;
            }
            catch
            {
                return "";
            }
        }

        public void CheckOpcConnection()
        {
            while (true)
            {
                if (PingIP(locHost))
                {
                    bizHandle.ShowInitLog("Y", InfoType.plcConn);
                }
                else
                {
                    bizHandle.ShowInitLog("N",InfoType.plcConn);
                }
                Thread.Sleep(5000);
            }
        }

        public void Run()
        {
            while(true)
            {
                Thread.Sleep(1000);
                if (!bizHandle.CheckDbConnection())
                {
                    continue;
                }
                foreach (Loc loc in locDic.Values)
                {
                    // 将4号堆垛机出库状态反馈给G1134
                    if (loc.LocPlcNo == "G1134")
                    {
                        try
                        {
                            var currLoc = loc as Trans;
                            var type = bizHandle.GetSrmTaskType("G1141");
                            if (type == "O")
                            {
                                bizHandle.SendSpareToPlc(currLoc,1);
                            }
                            else bizHandle.SendSpareToPlc(currLoc, 0);
                        }
                        catch { }
                    }
                    if (!bizHandle.GetOpcStatus(loc))
                    {
                        continue;
                    }
                    try
                    {
                        loc.Biz?.HandleLoc(loc);
                    }
                    catch (Exception ex)
                    {
                        bizHandle.ShowLocStatus(loc, Lan.Info("HandleBizAbnormal") + ex.Message);
                    }
                }
            }
        }
    }
}


