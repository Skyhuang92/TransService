using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using SRBL.McLanguage;
using SRBL.OpcClient;
using WCS.Entity;

namespace WCS.OpcHelper
{
    public class OpcDaHelper : IOpcHelper
    {
        private OpcClient _Opc;
        private string OpcServerName = McConfig.Instance.OpcServerName;
        private string OpcServerHost = McConfig.Instance.OpcServerHost;
        private string OpcServerGroup = McConfig.Instance.OpcServerGroup;
        
        public OpcDaHelper()
        {
            this._Opc = new OpcClient();
        }

        public bool OpcConnection(string[] opcStringItems, ref string errmsg)
        {
            try
            {
                if (_Opc.ConnectOpcServer(OpcServerHost, OpcServerName, ref errmsg))
                {
                    if(_Opc.AddOpcGroup(OpcServerGroup, ref errmsg))
                    {
                        return _Opc.AddOpcItems(OpcServerGroup, opcStringItems, ref errmsg);
                    }
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            return false;
        }

        public bool GetLocPlcStatus(Loc loc, ref string errmsg)
        {
            try
            {
                var itemNames = loc.opcItemDic.Values.Select(p => p.TagLongName);
                if(itemNames == null)
                {
                    errmsg = Lan.Info("ReadOPCItemsFail");
                    return false;
                }
                var opcItemValues = _Opc.ReadValues(OpcServerGroup, itemNames.ToArray(), ref errmsg);
                if (opcItemValues == null || opcItemValues.Length == 0)
                {
                    errmsg = Lan.Info("GetOPCItemsValueFail") + errmsg;
                    return false;
                }
                foreach(var opcItemValue in opcItemValues)
                {
                    if (opcItemValue == null)
                    {
                        string[] msg =
                        {
                            opcItemValue.TagLongName,
                            errmsg
                        };
                        errmsg = Lan.Info("GetOPCTagLongNameFail", msg);
                        return false;
                    }
                    if (opcItemValue.Quality.Equals(Opc.Da.Quality.Bad))
                    {
                        string[] msg =
                        {
                            opcItemValue.TagLongName, 
                            "BAD"
                        };
                        errmsg = Lan.Info("GetOPCTagLongNameConnectBad", msg);
                        return false;
                    }
                    var tagValue = opcItemValue.TagValue == null ? 0 : opcItemValue.TagValue;
                    var bizIdentity = loc.opcItemDic[opcItemValue.TagLongName].BizIdentity;
                    loc.SetPlcChangeValue(bizIdentity, tagValue);
                }
                return true;
            }
            catch(Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
        }

        public bool SendInfoToPlc(Loc loc, ref string errmsg)
        {
            var result = loc.SendInfos(ref errmsg);
            if (result.Count <= 0)
            {
                errmsg = Lan.Info("GetOPCWriteItemsFail");
                return false;
            }
            return _Opc.WriteValues(OpcServerGroup, result.ToArray(), ref errmsg);
        }

        public bool ClearInfoToPlc(Loc loc, ref string errmsg)
        {
            var result = loc.ClearInfos(ref errmsg);
            if (result.Count <= 0)
            {
                errmsg = Lan.Info("GetOPCWriteItemsFail");
                return false;
            }
            return _Opc.WriteValues(OpcServerGroup, result.ToArray(), ref errmsg);
        }

        public bool SendFaultNoToPlc(Loc loc, ref string errmsg)
        {
            var result = loc.SendFaultNo(ref errmsg);
            if (result.Count <= 0)
            {
                errmsg = Lan.Info("GetOPCWriteItemsFail");
                return false;
            }
            return _Opc.WriteValues(OpcServerGroup, result.ToArray(), ref errmsg);
        }

        public bool SendTaskNoToPlc(Loc loc, ref string errmsg)
        {
            var trans = loc as Trans;
            var result = trans.SendTaskNo(ref errmsg);
            if (result.Count <= 0)
            {
                errmsg = Lan.Info("GetOPCWriteItemsFail");
                return false;
            }
            return _Opc.WriteValues(OpcServerGroup, result.ToArray(), ref errmsg);
        }

        public bool SendHandleRequestFlagToPlc(Loc loc, ref string errmsg)
        {
            var trans = loc as Trans;
            var result = trans.SendHandleRequestFlag(ref errmsg);
            if (result.Count <= 0)
            {
                errmsg = Lan.Info("GetOPCWriteItemsFail");
                return false;
            }
            return _Opc.WriteValues(OpcServerGroup, result.ToArray(), ref errmsg);
        }

        public bool SendHandlePickFlagToPlc(Loc loc, ref string errmsg)
        {
            var trans = loc as Trans;
            var result = trans.SendHandleNeedToPutFlag(ref errmsg);
            if (result.Count <= 0)
            {
                errmsg = Lan.Info("GetOPCWriteItemsFail");
                return false;
            }
            return _Opc.WriteValues(OpcServerGroup, result.ToArray(), ref errmsg);
        }

        /// <summary>
        /// 写状态切换信号
        /// </summary>
        public bool SendInOutSwitchFlagToPlc(Loc loc, ref string errmsg)
        {
            var trans = loc as Trans;
            var result = trans.SendInOutSwitchFlag(ref errmsg);
            if (result.Count <= 0)
            {
                errmsg = Lan.Info("GetOPCWriteItemsFail");
                return false;
            }
            return _Opc.WriteValues(OpcServerGroup, result.ToArray(), ref errmsg);           
        }

        public bool SendSpareToPlc(Loc loc, ref string errmsg, int? status = 0)
        {
            var trans = loc as Trans;
            var result = trans.SendSpareFlag(ref errmsg, status);
            if (result.Count <= 0)
            {
                errmsg = Lan.Info("GetOPCWriteItemsFail");
                return false;
            }
            return _Opc.WriteValues(OpcServerGroup, result.ToArray(), ref errmsg);
        }

    }
}
