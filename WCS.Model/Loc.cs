using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WCS.Entity
{
    public class Loc: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性更改通知事件
        /// </summary>
        public void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public Dictionary<string, OpcItem> opcItemDic = null;

        public Loc()
        {
            BizStep = BizStatus.None;
            opcItemDic = new Dictionary<string, OpcItem>();
        }

        private string locNo;
        /// <summary>
        /// 站台编号
        /// </summary>
        public string LocNo
        {
            get
            {
                return this.locNo;
            }
            set
            {
                this.locNo = value;
                NotifyPropertyChanged(nameof(LocNo));
            }
        }

        private string locPlcNo;
        /// <summary>
        /// PLC编号
        /// </summary>
        public string LocPlcNo
        {
            get
            {
                return this.locPlcNo;
            }
            set
            {
                this.locPlcNo = value;
                NotifyPropertyChanged(nameof(LocPlcNo));
            }
        }

        /// <summary>
        /// PLC读取项value
        /// </summary>
        public ILocStatusRead PlcStatusRead
        {
            get;
            set;
        }

        /// <summary>
        /// PLC写入项value
        /// </summary>
        public ILocStatusWrite PlcStatusWrite
        {
            get;
            set;
        }

        private int locStatus = 0;
        /// <summary>
        /// 站台状态:用于界面展示
        /// </summary>
        public int LocStatus
        {
            get
            {
                return this.locStatus;
            }
            set
            {
                this.locStatus = value;
                NotifyPropertyChanged(nameof(LocStatus));
            }
        }

        private string locBizStep;
        /// <summary>
        /// 业务步骤
        /// </summary>
        public string LocBizStep
        {
            get
            {
                return this.locBizStep;
            }
            set
            {
                this.locBizStep = value;
                NotifyPropertyChanged(nameof(LocBizStep));
            }
        }

        private string locBizDesc;
        /// <summary>
        /// 业务描述
        /// </summary>
        public string LocBizDesc
        {
            get
            {
                return this.locBizDesc;
            }
            set
            {
                this.locBizDesc = value;
                NotifyPropertyChanged(nameof(LocBizDesc));
            }
        }

        /// <summary>
        /// 站台类型编号
        /// </summary>
        public string LocType
        {
            get; 
            set;
        }
        /// <summary>
        /// 站台类型描述
        /// </summary>
        public string LocTypeDesc
        {
            get; 
            set;
        }
        /// <summary>
        /// OPC Group配置类型
        /// </summary>
        public string OpcKind
        {
            get; 
            set;
        }
        /// <summary>
        /// 业务类型
        /// </summary>
        public string BizType
        {
            get; 
            set;
        }

        private BizStatus bizStep;
        /// <summary>
        /// 业务状态
        /// </summary>
        public BizStatus BizStep
        {
            get
            {
                return bizStep;
            }
            set
            {
                bizStep = value;
                setLocBizStep();
            }
        }

        public IBiz Biz
        {
            get;
            set;
        }

        private void setLocBizStep()
        {
            LocBizStep = BizStep.ToString();
        }

        /// <summary>
        /// 上位机处理故障码
        /// </summary>
        public long FaultNo
        {
            get;
            set;
        }

        private string execLog = string.Empty;

        /// <summary>
        /// 运行日志记录
        /// </summary>
        public string ExecLog
        {
            get
            {
                return execLog;
            }
            set
            {
                if (execLog.Length > 10000)
                {
                    execLog = string.Empty;
                }
                if (value == LastExecLog)
                {
                    return;
                }
                LastExecLog = value;

                execLog += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                execLog += Environment.NewLine;
                execLog += value;
                execLog += Environment.NewLine;
                execLog += "------------------------------------------";
                execLog += Environment.NewLine;
            }
        }

        /// <summary>
        /// 上次运行日志
        /// </summary>
        public string LastExecLog
        {
            get; 
            set;
        }

        /// <summary>
        /// 站台信息复位
        /// </summary>
        public virtual void InitLoc()
        {
            FaultNo = 0;
            LocStatus = 0;
            LocBizDesc = string.Empty;
        }

        /// <summary>
        /// DataChange
        /// </summary>
        public virtual void SetPlcChangeValue(string busIdentity, object value)
        {

        }

        /// <summary>
        /// 写信息
        /// </summary>
        public virtual List<KeyValuePair<string, object>> SendInfos(ref string errmsg)
        {
            return null;
        }

        /// <summary>
        /// 清空写信息
        /// </summary>
        public virtual List<KeyValuePair<string, object>> ClearInfos(ref string errmsg)
        {
            return null;
        }

        /// <summary>
        /// 写故障码
        /// </summary>
        public virtual List<KeyValuePair<string, object>> SendFaultNo(ref string errmsg)
        {
            var result = new List<KeyValuePair<string, object>>();
            try
            {
                var item = opcItemDic.Values.FirstOrDefault(p => p.BizIdentity.Equals("Write.FaultNo"));
                if (item != null)
                {
                    result.Add(new KeyValuePair<string, object>(item.TagLongName, Convert.ToUInt16(FaultNo)));
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                result = null;
            }
            return result;
        }
    }
}
