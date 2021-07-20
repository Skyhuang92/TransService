using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using OpcUaHelper;
using SRBL.McLanguage;
using WCS.Entity;

namespace WCS.OpcHelper
{
    public class OpcUaHelper : IOpcHelper
    {
        private OpcUaClient _Opc;
        private string OpcServerName = McConfig.Instance.OpcServerName;
        private string OpcServerHost = McConfig.Instance.OpcServerHost;
        private string OpcServerPort = McConfig.Instance.OpcServerPort;

        public OpcUaHelper()
        {
            this._Opc = new OpcUaClient();
        }

        public bool OpcConnection(string[] opcStringItems, ref string errmsg)
        {
            var result = false;
            try
            {
                var connstr = $"opc.tcp://{OpcServerHost}:{OpcServerPort}";
                var task = _Opc.ConnectServer(connstr);
                if (task.Status.ToString().ToUpper().Contains("FAULTED"))
                {
                    errmsg = task.Exception.InnerException.Message.ToString();
                }
                return true;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message + ex.StackTrace;
            }
            return result;
        }

        public bool GetLocPlcStatus(Loc loc, ref string errmsg)
        {
            try
            {
                var nodeIds = new NodeId[loc.opcItemDic.Count];
                int i = 0;
                foreach(var opcItem in loc.opcItemDic.Values)
                {
                    var text = $"ns=2;s={opcItem.TagLongName}";
                    var nodeId = new NodeId(text);
                    nodeIds[i] = nodeId;
                    i++;
                }
                var dataValues = _Opc.ReadNodes(nodeIds);
                if (dataValues == null || dataValues.Count == 0)
                {
                    errmsg = Lan.Info("GetOPCItemsValueFail");
                    return false;
                }
                i = 0;
                foreach (var dataValue in dataValues)
                {
                    if (dataValue == null)
                    {
                        string[] msg =
                        {
                            nodeIds[i].Identifier.ToString(),
                            errmsg
                        };
                        errmsg = Lan.Info("GetOPCTagLongNameFail", msg);
                        return false;
                    }
                    if (!dataValue.StatusCode.ToString().Equals("Good"))
                    {
                        string[] msg =
                        {
                            nodeIds[i].Identifier.ToString(),
                            dataValue.StatusCode.ToString()
                        };
                        errmsg = Lan.Info("GetOPCTagLongNameConnectBad", msg);
                        return false;
                    }
                    var tagValue = dataValue.Value == null ? 0 : dataValue.Value;
                    var bizIdentity = loc.opcItemDic[nodeIds[i].Identifier.ToString()].BizIdentity;
                    loc.SetPlcChangeValue(bizIdentity, tagValue);
                    i++;
                }
                return true;
            }
            catch (Exception ex)
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
            var nodes = new string[result.Count];
            var datas = new object[result.Count];
            int i = 0;
            foreach(KeyValuePair<string, object> item in result)
            {
                nodes[i] = $"ns=2;s={item.Key}";
                datas[i] = item.Value;
            }
            return _Opc.WriteNodes(nodes, datas);
        }

        public bool ClearInfoToPlc(Loc loc, ref string errmsg)
        {
            var result = loc.ClearInfos(ref errmsg);
            if (result.Count <= 0)
            {
                errmsg = Lan.Info("GetOPCWriteItemsFail");
                return false;
            }
            var nodes = new string[result.Count];
            var datas = new object[result.Count];
            int i = 0;
            foreach (KeyValuePair<string, object> item in result)
            {
                nodes[i] = $"ns=2;s={item.Key}";
                datas[i] = item.Value;
            }
            return _Opc.WriteNodes(nodes, datas);
        }

        public bool SendFaultNoToPlc(Loc loc, ref string errmsg)
        {
            var result = loc.SendFaultNo(ref errmsg);
            if (result.Count <= 0)
            {
                errmsg = Lan.Info("GetOPCWriteItemsFail");
                return false;
            }
            var nodes = new string[result.Count];
            var datas = new object[result.Count];
            int i = 0;
            foreach (KeyValuePair<string, object> item in result)
            {
                nodes[i] = $"ns=2;s={item.Key}";
                datas[i] = item.Value;
            }
            return _Opc.WriteNodes(nodes, datas);
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
            var nodes = new string[result.Count];
            var datas = new object[result.Count];
            int i = 0;
            foreach (KeyValuePair<string, object> item in result)
            {
                nodes[i] = $"ns=2;s={item.Key}";
                datas[i] = item.Value;
            }
            return _Opc.WriteNodes(nodes, datas);
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
            var nodes = new string[result.Count];
            var datas = new object[result.Count];
            int i = 0;
            foreach (KeyValuePair<string, object> item in result)
            {
                nodes[i] = $"ns=2;s={item.Key}";
                datas[i] = Convert.ToInt32(item.Value) == 1;
            }
            return _Opc.WriteNodes(nodes, datas);
        }

        public bool SendInOutSwitchFlagToPlc(Loc loc, ref string errmsg)
        {
            var trans = loc as Trans;
            var result = trans.SendInOutSwitchFlag(ref errmsg);
            if (result.Count <= 0)
            {
                errmsg = Lan.Info("GetOPCWriteItemsFail");
                return false;
            }
            var nodes = new string[result.Count];
            var datas = new object[result.Count];
            int i = 0;
            foreach (KeyValuePair<string, object> item in result)
            {
                nodes[i] = $"ns=2;s={item.Key}";
                datas[i] = Convert.ToInt32(item.Value) == 1;
            }
            return _Opc.WriteNodes(nodes, datas);
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
            var nodes = new string[result.Count];
            var datas = new object[result.Count];
            int i = 0;
            foreach (KeyValuePair<string, object> item in result)
            {
                nodes[i] = $"ns=2;s={item.Key}";
                datas[i] = Convert.ToInt32(item.Value) == 1;
            }
            return _Opc.WriteNodes(nodes, datas);
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
            var nodes = new string[result.Count];
            var datas = new object[result.Count];
            int i = 0;
            foreach (KeyValuePair<string, object> item in result)
            {
                nodes[i] = $"ns=2;s={item.Key}";
                datas[i] = item.Value;
            }
            return _Opc.WriteNodes(nodes, datas);
        }

    }
}
