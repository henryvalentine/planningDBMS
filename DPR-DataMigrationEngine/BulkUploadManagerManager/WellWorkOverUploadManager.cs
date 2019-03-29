using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Text;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;

namespace DPR_DataMigrationEngine.BulkUploadManagerManager
{
    public class WellWorkOverUploadManager
    {
        public bool Import(string filePath, string sheetName, ref List<WellWorkover> mList, ref string msg)
        {
            if (filePath.Length < 3 || new FileInfo(filePath).Exists == false || (Path.GetExtension(filePath) != ".xls" && Path.GetExtension(filePath) != ".xlsx"))
            {
                msg = "Invalid Excel File Format";
                return false;
            }

            if (sheetName.Length < 1)
            {
                msg = "Invalid Excel Sheet Name";
                return false;
            }

            var connectionstring = string.Empty;
            switch (Path.GetExtension(filePath))
            {
                case ".xls":
                    connectionstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES;'";
                    break;
                case ".xlsx":
                    connectionstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;ImportMixedTypes=Text'";
                    break;
            }

            if (connectionstring == "")
            {
                msg = "Process Error! Please try again later";
                return false;
            }

            var selectString = @"SELECT [well_name], [equipment_used], [workover_reason], [workover_Completion_month(MM)], [workover_Completion_year(yyyy)] FROM [" + sheetName + "$]";
            var myCon = new OleDbConnection(connectionstring); 
            try
            {
                if (myCon.State == ConnectionState.Closed)
                {
                    myCon.Open();
                }
                var cmd = new OleDbCommand(selectString, myCon);
                var adap = new OleDbDataAdapter(cmd);
                var ds = new DataSet();
                adap.Fill(ds);
                if (ds.Tables.Count < 1)
                {
                    msg = "Invalid WellWorkover Template!";
                    return false;
                }
                var dv = new DataView(ds.Tables[0]);
                if (dv.Count < 1)
                {
                    msg = "Invalid WellWorkover Template!";
                    return false;
                }

                msg = string.Empty;
                var sb = new StringBuilder();
                sb.AppendLine("<table width=\"98%\" cellspacing=\"1px\" border=\"1\" cellpadding=\"2px\">");
                sb.AppendLine(string.Format("<tr><th width=\"45%\">WellWorkover Name</th><th width=\"55%\">Error</th></tr>"));
                var errorExist = false;
                for (var i = 0; i < dv.Count; i++)
                {
                    var mymsg = string.Empty;
                    var wellName = dv[i].Row["well_name"].ToString().Trim();
                    if (wellName.Trim().Length < 2) { continue; }
                    var mInfo = ProcessRecord(dv[i], ref mymsg);
                    if (mInfo == null)
                    {
                        errorExist = true;
                        sb.AppendLine(mymsg.Length > 0
                                          ? string.Format(
                                              "<tr border=\"1\"><td width=\"45%\">{0}</td><td width=\"55%\">{1}</td></tr>", wellName,
                                              mymsg)
                                          : string.Format(
                                              "<tr border=\"1\"><td width=\"45%\">{0}</td><td width=\"55%\">Unknown Error</td></tr>",
                                              wellName));
                        continue;
                    }
                    mList.Add(mInfo);
                }
                sb.AppendLine("</table>");  
                if (errorExist)
                {
                    var sbb = new StringBuilder();
                    sbb.AppendLine("Following error occurred while loading your data template:");
                    sbb.AppendLine(sb.ToString());
                    msg = sbb.ToString();
                }
                myCon.Close();
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                myCon.Close();
                return false;
            }
        }
        private WellWorkover ProcessRecord(DataRowView dv, ref string msg)
        {
            if (dv == null) { return null; }
            try
            {
                var wellName = dv.Row["well_name"].ToString().Replace(" ", "").Replace(",", "").Trim();

                if (string.IsNullOrEmpty(wellName.Trim()))
                {
                    return null;
                }

                var mInfo = new WellWorkover
                {
                    WellName = wellName,
                };

                var ttcg = new WellServices().GetWellIdByName(wellName);

                if (ttcg < 1)
                {
                    msg = "Well Information could not be processed";
                    return null;
                }

                mInfo.WellId = ttcg;

                var year = dv.Row["workover_Completion_year(yyyy)"].ToString().Trim();
               long outYear;
               var yrResult = long.TryParse(year, out outYear);
                if (!yrResult && outYear < 1)
                {
                    msg = "Invalid Workover Completion Year.";
                    return null;
                }
                else
                {
                    mInfo.Year = outYear;
                }
                //workover_Completion_year(yyyy)

                var month = dv.Row["workover_Completion_month(MM)"].ToString().Trim();
                int outMonth;
                var mnResult = int.TryParse(month, out outMonth);
                if (!mnResult && outMonth < 1)
                {
                    msg = "Invalid Workover Completion Month.";
                    return null;
                }
                else
                {
                    mInfo.Month = outMonth;
                }

               
                mInfo.DateCompleted = outYear + "/"  + outMonth + "/01";

                if (!string.IsNullOrEmpty(dv.Row["equipment_used"].ToString().Trim()))
                {
                    var dfv = dv.Row["equipment_used"].ToString().Trim();
                    var equipmentId = new EquipmentServices().GetEquipmentId( dfv);
                    if (equipmentId < 1)
                    {
                        msg = "Equipment Information could not be processed";
                        return null;
                    }

                    mInfo.EquipmentId = equipmentId;
                }
                else
                {
                    msg = "Equipment used is empty.";
                    return null;
                }

                var reason = dv.Row["workover_reason"].ToString().Trim();
                if (!string.IsNullOrEmpty(reason))
                {
                    var dfv = reason;
                    var reasonId = new WellWorkOverReasonServices().GetWellWorkOverReasonIdByName(dfv);
                    if (reasonId < 1)
                    {
                        msg = "Workover Reason Information could not be processed";
                        return null;
                    }
                    mInfo.WellWorkOverReasonId = reasonId;
                }
                else
                {
                    msg = "Workover Reason is empty";
                    return null;
                }
                var status = new WellWorkoverServices().AddWellWorkoverCheckDuplicates(mInfo, mInfo.WellId, mInfo.WellWorkOverReasonId, mInfo.Month, mInfo.Year);
                if (status < 1)
                {
                    if (status == -3)
                    {
                        msg = "Well workover for the same Period, Reason and Equipment already exists for this well.";
                        return null;
                    }

                    msg = "An error encountered during upload.";
                    return null;
                }

                mInfo.WellWorkOverId = status;
                return mInfo;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }

        }
    }

}
