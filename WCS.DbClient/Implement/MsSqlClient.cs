using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;
using DapperExtensions;
using WCS.Entity;
using SRBL.DbClient.Dapper;

namespace WCS.DbClient
{
    public class MsSqlClient : IDbClient
    {
        private IDatabase Database = null;

        public MsSqlClient()
        {
            this.Database = DatabaseFactory.Create();
        }

        public Dictionary<string, Loc> GetLocData(string areaNo, ref string errmsg)
        {
            try
            {
                var dt = selectLocData(areaNo, ref errmsg);
                if (dt == null || dt.Rows.Count == 0)
                {
                    errmsg = "No Data " + errmsg;
                    return null;
                }
                var locDic = new Dictionary<string, Loc>();
                foreach (DataRow row in dt.Rows)
                {
                    var locKind = row["kind"].ToString();

                    switch (locKind)
                    {
                        case "Trans":
                        case "Stack":
                        case "TransSwitch":
                            var trans = new Trans();
                            trans.LocNo = row["loc_no"].ToString();
                            trans.LocPlcNo = row["loc_plc_no"].ToString();
                            trans.LocType = row["loc_type"].ToString();
                            trans.BizType = row["biz_type"].ToString();
                            trans.OpcKind = row["kind"].ToString();
                            locDic.Add(trans.LocNo, trans);
                            break;
                    }
                }
                return locDic;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return null;
            }
        }

        public Dictionary<string, OpcItem> GetOpcItemsData(string locNo, ref string errmsg)
        {
            try
            {
                var dt = selectOpcItems(locNo, ref errmsg);
                if (dt == null || dt.Rows.Count == 0)
                {
                    errmsg = "No Data " + errmsg;
                    return null;
                }
                var itemDic = new Dictionary<string, OpcItem>();
                foreach (DataRow row in dt.Rows)
                {
                    var opcItem = new OpcItem();
                    opcItem.LocNo = row["loc_no"].ToString();
                    opcItem.LocPlcNo = row["loc_plc_no"].ToString();
                    opcItem.TagLongName = row["taglongname"].ToString();
                    opcItem.BizIdentity = row["busidentity"].ToString();
                    opcItem.Kind = row["kind"].ToString();
                    itemDic.Add(opcItem.TagLongName, opcItem);
                }
                return itemDic;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return null;
            }
        }

        public TaskCmd GetTaskCmd(TaskCmd taskCmd, ref string errmsg)
        {
            var dt = selectTaskCmd(taskCmd, ref errmsg);
            if (dt == null || dt.Rows.Count == 0)
            {
                errmsg = "No Data " + errmsg;
                return null;
            }
            if (dt.Rows.Count > 1)
            {
                errmsg = "数据异常：大于1";
                return null;
            }
            var row = dt.Rows[0];
            var task = new TaskCmd();
            task.Objid = Convert.ToInt64(row["objid"]);
            task.TaskNo = Convert.ToInt64(row["task_no"]);
            task.SlocNo = row["sloc_no"].ToString();
            task.ElocNo = row["eloc_no"].ToString();
            task.SlocPlcNo = row["sloc_plc_no"].ToString();
            task.ElocPlcNo = row["eloc_plc_no"].ToString();
            task.CmdType = row["cmd_type"].ToString();
            task.CmdStep = row["cmd_step"].ToString();
            task.SlocType = row["sloc_type"].ToString();
            task.ElocType = row["eloc_type"].ToString();
            task.PalletNo = row["pallet_no"].ToString();
            task.CreateTime = row["creation_date"].ToString();
            return task;
        }

        public List<TaskCmd> GetTaskCmds(TaskCmd taskCmd, ref string errmsg)
        {
            var dt = selectTaskCmd(taskCmd, ref errmsg);
            if (dt == null || dt.Rows.Count == 0)
            {
                errmsg = "No Data " + errmsg;
                return null;
            }
            var taskCmds = new List<TaskCmd>();
            foreach (DataRow row in dt.Rows)
            {
                var task = new TaskCmd();
                task.Objid = Convert.ToInt64(row["objid"]);
                task.TaskNo = Convert.ToInt64(row["task_no"]);
                task.SlocNo = row["sloc_no"].ToString();
                task.ElocNo = row["eloc_no"].ToString();
                task.SlocPlcNo = row["sloc_plc_no"].ToString();
                task.ElocPlcNo = row["eloc_plc_no"].ToString();
                task.CmdType = row["cmd_type"].ToString();
                task.CmdStep = row["cmd_step"].ToString();
                task.SlocType = row["sloc_type"].ToString();
                task.ElocType = row["eloc_type"].ToString();
                task.PalletNo = row["pallet_no"].ToString();
                task.CreateTime = row["creation_date"].ToString();
                taskCmds.Add(task);
            }
            return taskCmds;
        }

        public bool GetDbTime(ref string errmsg)
        {
            try
            {
                Database.Connection.ExecuteScalar<DateTime>("select getdate()");
                if (Database.Connection.State == ConnectionState.Closed)
                {
                    Database.Connection.Open();
                }
                return true;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 空盘收集工位 控盘托盘垛数据绑定
        /// </summary>
        /// <param name="palletNo"></param>
        /// <param name="palletQty"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        public bool CreateBPalletProduct(string palletNo, int palletQty, ref string errmsg)
        {
            try
            {
                DynamicParameters para = new DynamicParameters();
                para.Add("i_mater_type", "BPALLET");
                para.Add("i_pallet_type", "JPTP");
                para.Add("i_mater_no", "BPALLET");
                para.Add("i_pallet_no", palletNo);
                para.Add("i_product_date", DateTime.Now);
                para.Add("i_product_qty", palletQty);
                para.Add("i_creation_by", "WCS");
                para.Add("o_product_guid", null, DbType.String, ParameterDirection.Output, size: 50);
                para.Add("o_err_code", null, DbType.String, ParameterDirection.Output, size: 50);
                para.Add("o_err_desc", null, DbType.String, ParameterDirection.Output, size: 50);
                Database.Connection.Execute("proc_1000_bind_product_pda", para, commandType: CommandType.StoredProcedure);
                var errNo = para.Get<string>("o_err_code");
                errmsg = para.Get<string>("o_err_desc");
                return errNo == "0";
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
        }

        public long CreateTask(ParameterForReqTask parameter, ref string errmsg)
        {
            try
            {
                var parameterQty = getParameterCountForReqTask(parameter.Objid, ref errmsg);
                var result = false;
                if (parameterQty == 0)
                {
                    result = saveParameterforReqTask(parameter, ref errmsg);
                }
                else if (parameterQty > 0)
                {
                    result = updatParameterForReqTask(parameter, ref errmsg);
                }
                long oResult = 0;
                if (result)
                {
                    oResult = ExecProcForReqTask(parameter.Objid, ref errmsg);
                }
                return oResult;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return 0;
            }
        }

        public long CreateCmd(ParameterForReqCmd parameter, ref string errmsg)
        {
            try
            {
                var parameterQty = getParameterCountForReqCmd(parameter.Objid, ref errmsg);
                var result = false;
                if (parameterQty == 0)
                {
                    result = saveParameterforReqCmd(parameter, ref errmsg);
                }
                else if (parameterQty > 0)
                {
                    result = updatParameterForReqCmd(parameter, ref errmsg);
                }
                long oResult = 0;
                if (result)
                {
                    oResult = ExecProcForReqCmd(parameter.Objid, ref errmsg);
                }
                return oResult;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return 0;
            }
        }

        public bool FinishTaskCmd(ParameterForFinishCmd parameter, ref string errmsg)
        {
            try
            {
                var parameterQty = getParameterCountForFinishCmd(parameter.Objid, ref errmsg);
                var result = false;
                if (parameterQty == 0)
                {
                    result = saveParameterforFinishCmd(parameter, ref errmsg);
                }
                else if (parameterQty > 0)
                {
                    result = updatParameterForFinishCmd(parameter, ref errmsg);
                }
                var oResult = false;
                if (result)
                {
                    oResult = ExecProcForFinishCmd(parameter.Objid, ref errmsg);
                }
                return oResult;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
        }

        private DataTable selectLocData(string areaNo, ref string errmsg)
        {
            try
            {
                var sql = @"select * 
                            from psb_opc_loc_group 
                            where biz_area = @areaNo";
                var dp = new DynamicParameters();
                dp.Add("areaNo", areaNo);
                return Database.Connection.QueryTable(sql, dp);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return null;
            }
        }

        private DataTable selectOpcItems(string locNo, ref string errmsg)
        {
            try
            {
                var sql = @"select t.*, t1.busidentity,
                            t.taggroup + t1.busidentity tagLongName
                            from psb_opc_loc_group t
                            left join psb_opc_loc_items t1 
                            on t1.kind = t.kind
                            where t.isenable = 1
                            and t1.isenable = 1
                            and t.loc_no = @locNo
                            order by t1.tagindex";
                var dp = new DynamicParameters();
                dp.Add("locNo", locNo);
                return Database.Connection.QueryTable(sql, dp);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return null;
            }
        }

        private DataTable selectTaskCmd(TaskCmd taskCmd, ref string errmsg)
        {
            try
            {
                var sb = new StringBuilder();
                var dp = new DynamicParameters();
                sb.Append("select t.* from wbs_task_cmd t");
                sb.Append(" where 1=1");
                if (!string.IsNullOrEmpty(taskCmd.SlocNo))
                {
                    sb.Append(" and t.sloc_no = @slocNo");
                    dp.Add("slocNo", taskCmd.SlocNo);
                }
                if (!string.IsNullOrEmpty(taskCmd.ElocNo))
                {
                    sb.Append(" and t.eloc_no = @elocNo");
                    dp.Add("elocNo", taskCmd.ElocNo);
                }
                if (!string.IsNullOrEmpty(taskCmd.PalletNo))
                {
                    sb.Append(" and t.pallet_no = @palletNo");
                    dp.Add("palletNo", taskCmd.PalletNo);
                }
                if (taskCmd.TaskNo > 0)
                {
                    sb.Append(" and t.task_no = @taskNo");
                    dp.Add("taskNo", taskCmd.TaskNo);
                }
                if (!string.IsNullOrEmpty(taskCmd.CmdStep))
                {
                    sb.Append(" and t.cmd_step = @cmdStep");
                    dp.Add("cmdStep", taskCmd.CmdStep);
                }
                sb.Append(" order by t.objid desc");

                return Database.Connection.QueryTable(sb.ToString(), dp);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return null;
            }
        }

        public bool UpdateCmdStep(long cmdObjid, string cmdStep, ref string errmsg)
        {
            try
            {
                var sql = @"update wbs_task_cmd
                            set cmd_step = @cmdStep,
                            excute_date = getdate()
                            where objid = @objid";
                var dp = new DynamicParameters();
                dp.Add("objid", cmdObjid);
                dp.Add("cmdstep", cmdStep);
                return Database.Connection.Execute(sql, dp) > 0;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
        }

        public long GetObjidForReqTask(ref string errmsg)
        {
            try
            {
                var sql = "select next value for seq_tproc_0100_task_request";
                return Database.Connection.ExecuteScalar<int>(sql);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return -1;
            }
        }

        private int getParameterCountForReqTask(long objid, ref string errmsg)
        {
            try
            {
                var sql = @"select count(*) 
                            from tproc_0100_task_request 
                            where objid = @objid";
                var dp = new DynamicParameters();
                dp.Add("objid", objid);
                return Database.Connection.ExecuteScalar<int>(sql, dp);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return -1;
            }
        }

        private bool saveParameterforReqTask(ParameterForReqTask parameter, ref string errmsg)
        {
            try
            {
                var sql = @"insert into tproc_0100_task_request
                            (objid, order_type_no, sloc_no, eloc_no, pallet_no, source_type)
                            values
                            (@reqObjid, @orderTypeNo, @slocNo, @elocNo, @palletNo, @sourceType)";
                var dp = new DynamicParameters();
                dp.Add("reqObjid", parameter.Objid);
                dp.Add("orderTypeNo", parameter.OrderTypeNo);
                dp.Add("slocNo", parameter.SlocNo);
                dp.Add("elocNo", parameter.ElocNo);
                dp.Add("palletNo", parameter.PalletNo);
                dp.Add("sourceType", parameter.SourceType);
                return Database.Connection.Execute(sql, dp) > 0;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
        }

        private bool updatParameterForReqTask(ParameterForReqTask parameter, ref string errmsg)
        {
            try
            {
                var sql = @"update tproc_0100_task_request
                            set pallet_no = @palletNo,
                            sloc_no = @slocNo,
                            eloc_no = @elocNo,
                            order_type_no = @orderTypeNo,
                            source_type = @sourceType
                            where objid = @reqObjid";
                var dp = new DynamicParameters();
                dp.Add("reqObjid", parameter.Objid);
                dp.Add("orderTypeNo", parameter.OrderTypeNo);
                dp.Add("slocNo", parameter.SlocNo);
                dp.Add("elocNo", parameter.ElocNo);
                dp.Add("palletNo", parameter.PalletNo);
                dp.Add("sourceType", parameter.SourceType);
                return Database.Connection.Execute(sql, dp) > 0;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
        }

        private long ExecProcForReqTask(long reqObjid, ref string errmsg)
        {
            try
            {
                var dp = new DynamicParameters();
                dp.Add("i_param_objid", reqObjid);
                dp.Add("o_task_no", 0, DbType.Int64, ParameterDirection.Output);
                dp.Add("o_err_code", 0, DbType.Int32, ParameterDirection.Output);
                dp.Add("o_err_desc", "", DbType.String, ParameterDirection.Output, size: 80);
                Database.Connection.Execute("proc_0100_task_request", param: dp, commandType: CommandType.StoredProcedure);
                errmsg = dp.Get<string>("o_err_desc");
                return dp.Get<long>("o_task_no");
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return 0;
            }
        }
        
        public long GetObjidForReqCmd(ref string errmsg)
        {
            try
            {
                var sql = "select next value for seq_tproc_0200_cmd_request";
                return Database.Connection.ExecuteScalar<int>(sql);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return -1;
            }
        }

        private int getParameterCountForReqCmd(long objid, ref string errmsg)
        {
            try
            {
                var sql = @"select count(*) 
                            from tproc_0200_cmd_request 
                            where objid = @objid";
                var dp = new DynamicParameters();
                dp.Add("objid", objid);
                return Database.Connection.ExecuteScalar<int>(sql, dp);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return -1;
            }
        }

        private bool saveParameterforReqCmd(ParameterForReqCmd parameter, ref string errmsg)
        {
            try
            {
                var sql = @"insert into tproc_0200_cmd_request
                            (objid, task_no, curr_loc_no)
                            values
                            (@reqObjid, @taskNo, @currLocNo)";
                var dp = new DynamicParameters();
                dp.Add("reqObjid", parameter.Objid);
                dp.Add("taskNo", parameter.TaskNo);
                dp.Add("currLocNo", parameter.CurrLocNo);
                return Database.Connection.Execute(sql, dp) > 0;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
        }

        private bool updatParameterForReqCmd(ParameterForReqCmd parameter, ref string errmsg)
        {
            try
            {
                var sql = @"update tproc_0200_cmd_request
                            set task_no = @taskNo,
                            curr_loc_no = @currLocNo
                            where objid = @reqObjid";
                var dp = new DynamicParameters();
                dp.Add("reqObjid", parameter.Objid);
                dp.Add("taskNo", parameter.TaskNo);
                dp.Add("currLocNo", parameter.CurrLocNo);
                return Database.Connection.Execute(sql, dp) > 0;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
        }

        private long ExecProcForReqCmd(long reqObjid, ref string errmsg)
        {
            try
            {
                var dp = new DynamicParameters();
                dp.Add("i_param_objid", reqObjid);
                dp.Add("o_cmd_objid", 0, DbType.Int64, ParameterDirection.Output);
                dp.Add("o_err_code", 0, DbType.Int32, ParameterDirection.Output);
                dp.Add("o_err_desc", "", DbType.String, ParameterDirection.Output, size: 80);
                Database.Connection.Execute("proc_0200_cmd_request", param: dp, commandType: CommandType.StoredProcedure);
                errmsg = dp.Get<string>("o_err_desc");
                return dp.Get<long>("o_cmd_objid");
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return 0;
            }
        }

        public long GetObjidForFinishCmd(ref string errmsg)
        {
            try
            {
                var sql = "select next value for seq_tproc_0300_cmd_finish";
                return Database.Connection.ExecuteScalar<int>(sql);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return -1;
            }
        }

        private int getParameterCountForFinishCmd(long objid, ref string errmsg)
        {
            try
            {
                var sql = @"select count(*) 
                            from tproc_0300_cmd_finish 
                            where objid = @objid";
                var dp = new DynamicParameters();
                dp.Add("objid", objid);
                return Database.Connection.ExecuteScalar<int>(sql, dp);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return -1;
            }
        }

        private bool saveParameterforFinishCmd(ParameterForFinishCmd parameter, ref string errmsg)
        {
            try
            {
                var sql = @"insert into tproc_0300_cmd_finish
                            (objid, task_no, curr_loc_no, finish_flag)
                            values
                            (@reqObjid, @taskNo, @currLocNo, @finishFlag)";
                var dp = new DynamicParameters();
                dp.Add("reqObjid", parameter.Objid);
                dp.Add("taskNo", parameter.TaskNo);
                dp.Add("currLocNo", parameter.CurrLocNo);
                dp.Add("finishFlag", parameter.FinishFlag);
                return Database.Connection.Execute(sql, dp) > 0;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
        }

        private bool updatParameterForFinishCmd(ParameterForFinishCmd parameter, ref string errmsg)
        {
            try
            {
                var sql = @"update tproc_0300_cmd_finish
                            set task_no = @taskNo,
                            curr_loc_no = @currLocNo,
                            finish_flag = @finishFlag
                            where objid = @reqObjid";
                var dp = new DynamicParameters();
                dp.Add("reqObjid", parameter.Objid);
                dp.Add("taskNo", parameter.TaskNo);
                dp.Add("currLocNo", parameter.CurrLocNo);
                dp.Add("finishFlag", parameter.FinishFlag);
                return Database.Connection.Execute(sql, dp) > 0;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
        }

        private bool ExecProcForFinishCmd(long reqObjid, ref string errmsg)
        {
            try
            {
                var dp = new DynamicParameters();
                dp.Add("i_param_objid", reqObjid);
                dp.Add("o_err_code", 0, DbType.Int32, ParameterDirection.Output);
                dp.Add("o_err_desc", "", DbType.String, ParameterDirection.Output, size: 80);
                Database.Connection.Execute("proc_0300_cmd_finish", param: dp, commandType: CommandType.StoredProcedure);
                errmsg = dp.Get<string>("o_err_desc");
                return string.IsNullOrEmpty(errmsg);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 更新站台状态
        /// </summary>
        public bool RecordLocStatus(string locNo, TransStatusRead locStatus, ref string errmsg)
        {
            try
            {
                var sql = @"update pem_loc_status
                            set task_no = @taskno,
                            pallet_no = @palletno,
                            sloc_no = @slocno,
                            eloc_no = @elocno,
                            status_auto = @auto,
                            status_load = @load,
                            status_fault = @falut,
                            status_request = @request,
                            status_free = @free,
                            status_needtoput = @needtoput,
                            record_time = getdate()
                            where loc_plc_no = @locno";               
                var dp = new DynamicParameters();
                dp.Add("locno", locNo);
                dp.Add("taskno", locStatus.TaskNo);
                dp.Add("palletno", locStatus.PalletNo);
                dp.Add("slocno", locStatus.SlocPlcNo);
                dp.Add("elocno", locStatus.ElocPlcNo);
                dp.Add("auto", locStatus.StatusAuto);
                dp.Add("load", locStatus.StatusLoad);
                dp.Add("falut", locStatus.StatusFault);
                dp.Add("request", locStatus.StatusRequest);
                dp.Add("free", locStatus.StatusFree);
                dp.Add("needtoput", locStatus.StatusNeedToPut);
                var result = Database.Connection.Execute(sql, dp);
                return result > 0;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 记录空盘收集工位站台状态（不记录托盘号）
        /// </summary>
        public bool RecordBPalletLocStatus(string locNo, TransStatusRead locStatus, ref string errmsg)
        {
            try
            {
                var sql = @"update pem_loc_status
                            set task_no = @taskno,                           
                            sloc_no = @slocno,
                            eloc_no = @elocno,
                            status_auto = @auto,
                            status_load = @load,
                            status_fault = @falut,
                            status_request = @request,
                            status_free = @free,
                            status_needtoput = @needtoput,
                            record_time = getdate(),
                            pallet_amount = @palletamount
                            where loc_no = @locno";
                var dp = new DynamicParameters();
                dp.Add("locno", locNo);
                dp.Add("taskno", locStatus.TaskNo);                
                dp.Add("slocno", locStatus.SlocPlcNo);
                dp.Add("elocno", locStatus.ElocPlcNo);
                dp.Add("auto", locStatus.StatusAuto);
                dp.Add("load", locStatus.StatusLoad);
                dp.Add("falut", locStatus.StatusFault);
                dp.Add("request", locStatus.StatusRequest);
                dp.Add("free", locStatus.StatusFree);
                dp.Add("needtoput", locStatus.StatusNeedToPut);
                dp.Add("palletamount", locStatus.PalletAmount);
                return Database.Connection.Execute(sql, dp) > 0;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 获取站台记录的托盘号
        /// </summary>
        /// <param name="locNo"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        public string GetRecordBPalletLoc(string locNo, ref string errmsg)
        {
            try
            {
                var sql = @"select pallet_no 
                            from pem_loc_status 
                            where loc_no = @locNo";
                var dp = new DynamicParameters();
                dp.Add("locNo", locNo);

                var dt = Database.Connection.QueryTable(sql, dp);
                return dt.Rows[0]["pallet_no"].ToString();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return string.Empty;
            }
        }

        /// <summary>
        /// [按键]任务处理
        /// </summary>
        public bool HandleTaskCmd(long taskNo, int finishflag, ref string errmsg)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("i_task_no", taskNo);
                param.Add("i_state", finishflag);
                param.Add("i_last_modified_by", "WCS");
                param.Add("o_err_desc", "", DbType.String, ParameterDirection.Output, size: 80);
                Database.Connection.Execute("proc_0401_process_task", param, commandType: CommandType.StoredProcedure);
                errmsg = param.Get<string>("o_err_desc");
                return string.IsNullOrEmpty(errmsg);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 获取ScanRid
        /// </summary>
        public string GetScanRfid(string locNo, ref string errmsg)
        {
            try
            {
                var sql = @"select isnull(min(scan_rfid_no),'') 
                            from pem_loc_status 
                            where loc_no = @locNo";
                var param = new DynamicParameters();
                param.Add("locNo", locNo);
                return Database.Connection.ExecuteScalar<string>(sql, param);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 2020.11.06 新增 获取WMS设置的站台出入库模式   //记录空值时状态
        /// </summary>
        /// <param name="locNo"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        public string GetLocInOutSwitch(string locNo, ref string errmsg)
        {
            try
            {
                var sql = $"select limit_IO_type from psb_loc where loc_no = '{locNo}'";
                return Database.Connection.ExecuteScalar<string>(sql);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 2020.11.09 新增 获取站台对应堆垛机的最近任务类型
        /// </summary>
        /// <param name="locNo"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        public string GetElocSrmInfo(string locPlcNo, ref string errmsg)
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append(" select last_task_type from pem_srm_status p ");
                sb.Append(" left join psb_loc l on p.srm_no = l.transfer_no ");
                sb.Append($" where l.loc_plc_no = '{locPlcNo}' ");
                return Database.Connection.ExecuteScalar<string>(sb.ToString());
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 2021.06.28 新增 获取站台对应堆垛机的当前任务类型
        /// </summary>
        /// <param name="locNo"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        public string GetSrmTaskType(string locPlcNo, ref string errmsg)
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append(" select top 1 task_type from pem_srm_status p ");
                sb.Append(" left join psb_loc l on p.srm_no = l.transfer_no ");
                sb.Append($" where l.loc_plc_no = '{locPlcNo}' ");
                return Database.Connection.ExecuteScalar<string>(sb.ToString());
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return "";
            }
        }

        /// <summary>
        /// 2020.11.09 新增 获取目的站台对应堆垛机的当前出库任务信息
        /// </summary>
        /// <param name="locNo"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        public DataTable GetElocSrmTaskInfo(string locPlcNo, ref string errmsg)
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append(" select t.task_no,t.task_step,c.objid,c.eloc_no,c.cmd_step from wbs_task t ");
                sb.Append(" left join wbs_task_cmd c on t.task_no = c.task_no ");
                sb.Append(" left join psb_loc l on t.transfer_no = l.transfer_no ");
                sb.Append($" where t.io_type = 'O' and l.loc_plc_no = '{locPlcNo}' ");
                sb.Append(" order by t.task_no asc ");
                return Database.Connection.QueryTable(sb.ToString());
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 2020.11.10 站台状态更新 额外新增托盘数量
        /// </summary>
        public bool RecordOtherAdded(string locNo, string palletAmount)
        {
            try
            {
                var sql = $"update pem_loc_status set pallet_amount = '{palletAmount}' where loc_no = '{locNo}'";
                Database.Connection.Execute(sql);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
