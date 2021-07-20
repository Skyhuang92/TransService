using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using WCS.MyControl;
using WCS.Biz;
using WCS.DbClient;
using System.Configuration;
using System.Windows.Controls;
using System.Windows.Input;
using SRBL.LogAgent;
using System.Diagnostics;
using WCS.Entity;

namespace WCS.Trans
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private BizInit Biz;
        private Dictionary<string, TranControl> locControlDic;
        // 站台列表
        private Dictionary<string, RGV> rgvControlDic;
        private string LimitLocNo = string.Empty;
        private string keyPassword = string.Empty;
        private string currentLanguage = string.Empty;
        /// <summary>
        /// 日志
        /// </summary>
        private ILog log
        {
            get
            {
                return Log.Store[this.GetType().FullName];
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            this.lbTime.Content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ShowFormData.Instance.OnAppDtoData += ShowInfo;
            keyPassword = "123";
            locControlDic = new Dictionary<string, TranControl>();
            rgvControlDic = new Dictionary<string, RGV>();
            Biz = new BizInit();
            if (Biz.InitLocData() && Biz.InitLocOpc())
            {
                currentLanguage = Tool.GetConfig("Language");
                SetLanguage();
                InitLocControl();
                ShowTaskCmd();
                this.dgvLoc.ItemsSource = Biz.locDic.Values.ToList();
                var thCheck = new Thread(Biz.CheckOpcConnection);
                thCheck.IsBackground = true;
                thCheck.Start();
                var thBiz = new Thread(Biz.Run);
                thBiz.IsBackground = true;
                thBiz.Start();
            }
        }

        /// <summary>
        /// 初始化站台信息
        /// </summary>
        private void InitLocControl()
        {
            foreach (UIElement element in locCanvas.Children)
            {
                var checkTrans = element is TranControl;
                if (checkTrans)
                {
                    var transControl = (element as TranControl);
                    var locInfo = Biz.locDic.Values.FirstOrDefault(p => p.LocPlcNo == transControl.LocPlcNo);
                    if (locInfo == null)
                    {
                        continue;
                    }
                    transControl.LocNo = locInfo.LocNo;
                    transControl.SetToDirection();
                    transControl.Click += LocControl_Click;
                    this.locControlDic.Add(locInfo.LocNo, transControl);
                }

                var checkRgv = element is RGV;
                if (checkRgv)
                {
                    var rgvControl = (element as RGV);
                    var locInfo = Biz.locDic.Values.FirstOrDefault(p => p.LocPlcNo == rgvControl.LocPlcNo);
                    if (locInfo == null)
                    {
                        continue;
                    }
                    rgvControl.LocNo = locInfo.LocNo;                   
                    rgvControl.Click += LocControl_Click;
                    this.rgvControlDic.Add(locInfo.LocNo, rgvControl);
                    continue;
                }
            }
        }

        /// <summary>
        /// 单机站台状态控件事件
        /// </summary>
        private void LocControl_Click(string locNo)
        {
            if (!locNo.Equals(LimitLocNo))
            {
                LimitLocNo = locNo;
                var loc = Biz.locDic[locNo];
                this.txtTaskList.Text = loc.LocPlcNo + Tool.GetLog("tabTaskList");
                this.txtPlcInfo.Text  = loc.LocPlcNo + Tool.GetLog("tabReadPLCStatus");
                this.txtLocLog.Text   = loc.LocPlcNo + Tool.GetLog("tabLocLog");
                ShowTaskCmd();
                ShowLocStatus(locNo);
            }
        }

        #region 界面展示
        public void ShowInfo(object sender, AppDataEventArgs e)
        {
            var appData = e.AppData;
            var msg = appData.StringInfo;
            var locNo = appData.LocNo;
            var infoType = appData.InfoType;
            FormShow(msg, locNo, infoType);
        }

        private void FormShow(string msg, string locNo, InfoType infoType)
        {
            this.Dispatcher.Invoke(() =>
            {
                switch (infoType)
                {
                    case InfoType.dbConn:
                        ShowDbConnStatus(msg);
                        break;
                    case InfoType.plcConn:
                        ShowPlcConnStatus(msg);
                        break;
                    case InfoType.logInfo:
                        ShowExecLog(msg, locNo);
                        break;
                    case InfoType.locStatus:
                        ShowLocStatus(locNo);
                        break;
                    case InfoType.taskCmd:
                        ShowTaskCmd();
                        ShowExecLog(msg, locNo);
                        break;
                    default:
                        break;
                }
            });
        }

        private void ShowExecLog(string msg, string locNo)
        {
            if (txtLocRecord.Text.Length > 20000)
            {
                this.txtLocRecord.Clear();
            }
            if (string.IsNullOrEmpty(locNo))
            {
                var info = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                info += Environment.NewLine;
                info += msg;
                info += Environment.NewLine;
                info += "---------------------------------------------";
                info += Environment.NewLine;
                this.txtLocRecord.AppendText(info);
                log.Debug(Environment.NewLine + info);
                return;
            }
            if (!msg.Equals(Biz.locDic[locNo].LastExecLog))
            {
                Biz.locDic[locNo].ExecLog = msg;
                var info = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                info += Environment.NewLine;
                info += msg;
                info += Environment.NewLine;
                info += "---------------------------------------------";
                info += Environment.NewLine;
                this.txtLocRecord.AppendText(info);
                log.Debug(Environment.NewLine + info);
            }
        }

        private void ShowPlcConnStatus(string msg)
        {
            if (msg.Equals("Y"))
            {
                this.recPlcConnStatus.Fill = CustomSolidBrush.Green;
            }
            else
            {
                this.recPlcConnStatus.Fill = CustomSolidBrush.Red;
            }
        }

        private void ShowDbConnStatus(string msg)
        {
            if (msg.Equals("Y"))
            {
                this.recDbConnStatus.Fill = CustomSolidBrush.Green;
            }
            else
            {
                this.recDbConnStatus.Fill = CustomSolidBrush.Red;
            }
        }

        /// <summary>
        /// 站台状态刷新
        /// </summary>
        private void ShowLocStatus(string locNo)
        {
            if (locControlDic.Keys.Contains(locNo))
            {
                var locControl = locControlDic[locNo];
                var opcStatus = Biz.locDic[locNo].PlcStatusRead as TransStatusRead;
                locControl.SetToAuto(opcStatus.StatusAuto);
                locControl.SetToLoad(opcStatus.StatusLoad);
                locControl.SetToRequest(opcStatus.StatusNeedToPut);
                locControl.SetToRequest(opcStatus.StatusRequest);
                locControl.SetToFault(opcStatus.StatusFault);
                if (string.IsNullOrEmpty(LimitLocNo) || !LimitLocNo.Equals(locNo))
                    return;
            }

            if (rgvControlDic.Keys.Contains(locNo))
            {
                var locControl = rgvControlDic[locNo];
                var opcStatus = Biz.locDic[locNo].PlcStatusRead as TransStatusRead;
                locControl.SetToAuto(opcStatus.StatusAuto);
                locControl.SetToLoad(opcStatus.StatusLoad);
                locControl.SetToRequest(opcStatus.StatusNeedToPut);
                locControl.SetToRequest(opcStatus.StatusRequest);
                locControl.SetToFault(opcStatus.StatusFault);
                if (string.IsNullOrEmpty(LimitLocNo) || !LimitLocNo.Equals(locNo))
                    return;
            }

            if (string.IsNullOrEmpty(LimitLocNo))
                return;

            var currLoc = Biz.locDic[LimitLocNo];
            this.txtRefTime.Text = DateTime.Now.ToString("yyyy-MM-dd hh24:mm:ss.fff");
            //刷新读取信息
            var currReadStatus = currLoc.PlcStatusRead as TransStatusRead;
            this.tbTaskNoR.Text = currReadStatus.TaskNo.ToString();
            this.tbPalletNoR.Text = currReadStatus.PalletNo;
            this.tbSlocNoR.Text = currReadStatus.SlocPlcNo;
            this.tbElocNoR.Text = currReadStatus.ElocPlcNo;
            this.epAuto.Fill = currReadStatus.StatusAuto == 1 ? CustomSolidBrush.LimeGreen : CustomSolidBrush.Red;
            this.epFault.Fill = currReadStatus.StatusFault == 1 ? CustomSolidBrush.Red : CustomSolidBrush.White;
            this.epLoad.Fill = currReadStatus.StatusLoad == 1 ? CustomSolidBrush.LimeGreen : CustomSolidBrush.White;
            this.epRequest.Fill = currReadStatus.StatusRequest == 1 ? CustomSolidBrush.LimeGreen : CustomSolidBrush.White;
            this.epFree.Fill = currReadStatus.StatusFree == 1 ? CustomSolidBrush.LimeGreen : CustomSolidBrush.White;
            this.epToLoad.Fill = currReadStatus.StatusNeedToPut == 1 ? CustomSolidBrush.LimeGreen : CustomSolidBrush.White;
            //刷新写入信息
            var currWriteStatus = currLoc.PlcStatusWrite as TransStatusWrite;
            this.tbTaskNoW.Text = currWriteStatus.TaskNo.ToString();
            this.tbPalletNoW.Text = currWriteStatus.PalletNo;
            this.tbSlocNoW.Text = currWriteStatus.SlocPlcNo;
            this.tbElocNoW.Text = currWriteStatus.ElocPlcNo;
            this.tbFaultNoW.Text = currWriteStatus.FaultNo;
            this.eqTaskDeal.Fill = currWriteStatus.HandleRequest == 1 ? CustomSolidBrush.LimeGreen : CustomSolidBrush.White;
            this.eqPickDeal.Fill = currWriteStatus.HandleNeedToPut == 1 ? CustomSolidBrush.LimeGreen : CustomSolidBrush.White;
            this.txtCurrLocRecord.Text = currLoc.ExecLog;
        }

        /// <summary>
        /// 指令信息刷新
        /// </summary>
        private void ShowTaskCmd()
        {
            var taskList = new List<TaskCmd>();
            if (!string.IsNullOrEmpty(LimitLocNo))
            {
                this.dgv.ItemsSource = BizHandle.Instance.GetTaskCmds(LimitLocNo);
                return;
            }
            foreach (var loc in Biz.locDic.Values)
            {
                var taskcmds = BizHandle.Instance.GetTaskCmds(loc.LocNo);
                if (taskcmds == null || taskcmds.Count == 0)
                {
                    continue;
                }
                taskList.AddRange(taskcmds);
            }
            this.dgv.ItemsSource = taskList;
        }
        #endregion

        #region 功能按键
        /// <summary>
        /// 指令重发
        /// </summary>
        private void btnRefSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv.SelectedItem == null)
                {
                    MessageBox.Show(Tool.GetLog("NotSelectTaskData"), Tool.GetLog("Resend"), MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    return;
                }
                var selectItem = (TaskCmd)dgv.SelectedItem;
                var taskNo = selectItem.TaskNo;
                if (taskNo == 0)
                {
                    MessageBox.Show(Tool.GetLog("TaskNoInvalid"), Tool.GetLog("Resend"), MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    return;
                }
                var result = MessageBox.Show( string.Format(Tool.GetLog("IsResendTask"), taskNo),
                             Tool.GetLog("Resend"), MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
                var fm = new Password(keyPassword);
                fm.ShowDialog();
                if (!(bool)fm.DialogResult)
                {
                    return;
                }
                var errmsg = string.Empty;
                if (!BizHandle.Instance.HandleTaskCmd(taskNo, 303, ref errmsg))
                {
                    MessageBox.Show(Tool.GetLog("ResendTaskFail"), errmsg);
                    return;
                }
                ShowTaskCmd();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Tool.GetLog("ResendTaskAbnormal"), ex.Message);
            }
        }
        /// <summary>
        /// 指令结束
        /// </summary>
        private void btnFinish_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv.SelectedItem == null)
                {
                    MessageBox.Show(Tool.GetLog("NotSelectTaskData"), Tool.GetLog("Finish"), MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    return;
                }
                var selectItem = (TaskCmd)dgv.SelectedItem;
                var taskNo = selectItem.TaskNo;
                if (taskNo == 0)
                {
                    MessageBox.Show(Tool.GetLog("TaskNoInvalid"), Tool.GetLog("Finish"), MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    return;
                }
                var result = MessageBox.Show(string.Format(Tool.GetLog("IsFinishTask"), taskNo),
                             Tool.GetLog("Finish"), MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
                var fm = new Password(keyPassword);
                fm.ShowDialog();
                if (!(bool)fm.DialogResult)
                {
                    return;
                }
                var errmsg = string.Empty;
                if (!BizHandle.Instance.HandleTaskCmd(taskNo, 302, ref errmsg))
                {
                    MessageBox.Show(Tool.GetLog("FinishTaskFail"), errmsg);
                    return;
                }
                ShowTaskCmd();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Tool.GetLog("FinishTaskAbnormal"), ex.Message);
            }
        }
        /// <summary>
        /// 指令删除
        /// </summary>
        private void btnDel_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv.SelectedItem == null)
                {
                    MessageBox.Show(Tool.GetLog("NotSelectTaskData"), Tool.GetLog("Delete"), MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    return;
                }
                var selectItem = (TaskCmd)dgv.SelectedItem;
                var taskNo = selectItem.TaskNo;
                if (taskNo == 0)
                {
                    MessageBox.Show(Tool.GetLog("TaskNoInvalid"), Tool.GetLog("Delete"), MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    return;
                }
                var result = MessageBox.Show(string.Format(Tool.GetLog("IsDeleteTask"), taskNo),
                             Tool.GetLog("Delete"), MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
                var fm = new Password(keyPassword);
                fm.ShowDialog();
                if (!(bool)fm.DialogResult)
                {
                    return;
                }
                var errmsg = string.Empty;
                if (!BizHandle.Instance.HandleTaskCmd(taskNo, 301, ref errmsg))
                {
                    MessageBox.Show(Tool.GetLog("DeleteTaskFail"), errmsg);
                    return;
                }
                ShowTaskCmd();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Tool.GetLog("DeleteTaskAbnormal"), ex.Message);
            }
        }

        /// <summary>
        /// 界面刷新
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.txtTaskList.Text = Tool.GetLog("tabTaskList");
            this.txtPlcInfo.Text  = Tool.GetLog("tabReadPLCStatus");
            this.txtLocLog.Text   = Tool.GetLog("tabLocLog");
            this.LimitLocNo = string.Empty;
            ShowTaskCmd();
        }

        /// <summary>
        /// 帮助按键
        /// </summary>
        private void btnHelp_Click(object sender, EventArgs e)
        {
            Tool.OpenFile($"{System.Environment.CurrentDirectory}/Help.pdf");
        }

        /// <summary>
        /// 状态复位
        /// </summary>
        private void btnRestart_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(LimitLocNo))
                {
                    MessageBox.Show(Tool.GetLog("NotSelectLoc"), Tool.GetLog("Restart"), MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    return;
                }
                var loc = Biz.locDic[LimitLocNo];
                var result = MessageBox.Show(string.Format(Tool.GetLog("IsRestartBizStep"), loc.LocPlcNo),
                             Tool.GetLog("Restart"), MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
                var fm = new Password(keyPassword);
                fm.ShowDialog();
                if (!(bool)fm.DialogResult)
                {
                    return;
                }
                loc.InitLoc();
                loc.BizStep = BizStatus.None;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Tool.GetLog("RestartBizStepAbnormal"), ex.Message);
            }
        }
        #endregion

        #region 多语言
        private void miZhCn_Click(object sender, RoutedEventArgs e)
        {
            currentLanguage = "zh_CN";
            SetLanguage();

        }

        private void miThTh_Click(object sender, RoutedEventArgs e)
        {
            currentLanguage = "th_TH";
            SetLanguage();
        }

        private void miEnGb_Click(object sender, RoutedEventArgs e)
        {
            currentLanguage = "en_GB";
            SetLanguage();
        }
        private void SetLanguage()
        {
            this.miZhCn.IsChecked = false;
            this.miEnGb.IsChecked = false;
            this.miThTh.IsChecked = false;
            LoadLanguage();
            Lan.Language = currentLanguage;
            Tool.SetConfig("Language", currentLanguage);
        }

        /// <summary>
        /// 设置语言
        /// </summary>
        private void LoadLanguage()
        {
            var dictionaryList = new List<ResourceDictionary>();
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                dictionaryList.Add(dictionary);
            }
            string requestedCulture = string.Empty;
            switch (currentLanguage)
            {
                case "th_TH":
                    this.miThTh.IsChecked = true;
                    requestedCulture = @"Language\th_TH.xaml";
                    break;
                case "en_GB":
                    this.miEnGb.IsChecked = true;
                    requestedCulture = @"Language\en_GB.xaml";
                    break;
                case "zh_CN":
                    this.miZhCn.IsChecked = true;
                    requestedCulture = @"Language\zh_CN.xaml";
                    break;
            }
            var resourceDictionary = dictionaryList.FirstOrDefault(p => p.Source.OriginalString.Equals(requestedCulture));
            Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            RefDataBoxColumnHeader();
        }

        /// <summary>
        /// 刷新DataBox列名
        /// </summary>
        private void RefDataBoxColumnHeader()
        {
            this.dgcLocNo.Header = Tool.GetLog("gridLocNo");
            this.dgcLocPlcNo.Header = Tool.GetLog("gridLocPlcNo");
            this.dgcLocBizStep.Header = Tool.GetLog("gridBizStep");
            this.dgcLocBizDesc.Header = Tool.GetLog("gridBizDesc");
            this.dgcObjid.Header = Tool.GetLog("gridObjid");
            this.dgcTaskNo.Header = Tool.GetLog("gridTaskNo");
            this.dgcPalletNo.Header = Tool.GetLog("gridPalletNo");
            this.dgcCmdType.Header = Tool.GetLog("gridTaskType");
            this.dgcCmdStep.Header = Tool.GetLog("gridCmdStep");
            this.dgcSlocPlcNo.Header = Tool.GetLog("gridSlocPlcNo");
            this.dgcElocPlcNo.Header = Tool.GetLog("gridElocPlcNo");
            this.dgcSlocType.Header = Tool.GetLog("gridSlocType");
            this.dgcElocType.Header = Tool.GetLog("gridElocType");
            this.dgcCreateTime.Header = Tool.GetLog("gridCreationTime");
            this.dgcSlocNo.Header = Tool.GetLog("gridSlocNo");
            this.dgcElocNo.Header = Tool.GetLog("gridElocNo");
        }
        #endregion
    }
}
