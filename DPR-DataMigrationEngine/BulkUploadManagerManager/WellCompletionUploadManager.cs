using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Text;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.Services.ServiceManager;

namespace DPR_DataMigrationEngine.BulkUploadManagerManager
{
    public class WellCompletionUploadManager
    {
        public bool Import(string filePath, string sheetName, ref List<WellCompletion> mList, ref string msg, int completionTypeId)
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

            var selectString = "";
            if (completionTypeId == 1)
            {
                selectString = @"SELECT [wellname], [equipmentused], [isinitialcompletion(YES/NO)], [lower_interval], [upper_interval], [datecompleted(yyyy/MM/dd)] FROM [" + sheetName + "$]";
            }

            if (completionTypeId == 2)
            {
                selectString = @"SELECT [wellname], [equipmentused], [isinitialcompletion(YES/NO)], [lower_interval_1], [upper_interval_1], [lower_interval_2], [upper_interval_2], [datecompleted(yyyy/MM/dd)] FROM [" + sheetName + "$]";
            }
            
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
                    msg = "Invalid WellCompletion Template!";
                    return false;
                }
                var dv = new DataView(ds.Tables[0]);
                if (dv.Count < 1)
                {
                    msg = "Invalid WellCompletion Template!";
                    return false;
                }

                msg = string.Empty;
                var sb = new StringBuilder();
                sb.AppendLine("<table width=\"98%\" cellspacing=\"1px\" border=\"1\" cellpadding=\"2px\">");
                sb.AppendLine(string.Format("<tr><th width=\"45%\">WellCompletion Name</th><th width=\"55%\">Error</th></tr>"));
                var errorExist = false;
                
                for (var i = 0; i < dv.Count; i++)
                {
                    var mymsg = string.Empty;
                    var mInfo = new WellCompletion();
                    var wellName = dv[i].Row["wellname"].ToString().Trim();
                    if (completionTypeId == 1)
                    {
                        mInfo = ProcessRecord(dv[i], ref mymsg, completionTypeId);
                    }

                    if (completionTypeId == 2)
                    {
                        mInfo = ProcessRecord2(dv[i], ref mymsg, completionTypeId);
                    }

                    if (mInfo == null)
                    {
                        errorExist = true;
                        sb.AppendLine(mymsg.Length > 0
                                          ? string.Format(
                                              "<tr border=\"1\"><td width=\"45%\">{0}</td><td width=\"55%\">{1}</td></tr>", wellName,
                                              mymsg)
                                          : string.Format(
                                              "<tr border=\"1\"><td width=\"45%\">{0}</td><td width=\"55%\">Error</td></tr>",
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
        private WellCompletion ProcessRecord(DataRowView dv, ref string msg, int completionTypeId)
        {
            if (dv == null) { return null; }
            try
            {
                var wellName = dv.Row["wellname"].ToString().Replace(" ", "").Replace(",", "").Trim();

                if (string.IsNullOrEmpty(wellName.Trim()))
                {
                    return null;
                }

                var mInfo = new WellCompletion
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

                var year = dv.Row["datecompleted(yyyy/MM/dd)"].ToString().Trim();
                if (string.IsNullOrEmpty(year))
                {
                    msg = "Completion Date is empty.";
                    return null;
                }
                else
                {
                    DateTime outYear;
                    var yrResult = DateTime.TryParse(year, out outYear);
                    if (!yrResult)
                    {
                        msg = "Completion Date is empty.";
                        return null;
                    }
                    else
                    {
                        mInfo.DateCompleted = outYear;
                    }
                }
                // [lower_interval], [upper_interval],
                var completionStatus = dv.Row["isinitialcompletion(YES/NO)"].ToString().Trim();

                if (string.IsNullOrEmpty(completionStatus))
                {
                    msg = "Please indicate if Well Completion is Initial or ortherwise.";
                    return null;
                }
                else
                {
                    if (completionStatus.Trim().ToLower().Replace(" ", "") == "yes")
                    {
                        mInfo.IsInitial = true;
                    }
                    if (completionStatus.Trim().ToLower().Replace(" ", "") == "no")
                    {
                        mInfo.IsInitial = false;
                    }
                }
                double l1 = 0;
                double up1 = 0;
                var lowerInter = dv.Row["lower_interval"].ToString().Trim();
                if (string.IsNullOrEmpty(lowerInter))
                {
                    msg = "Please provide lower Interval.";
                    return null;
                }
                else
                {
                    double outLint;
                    var dtResult = double.TryParse(lowerInter, out outLint);
                    if (!dtResult && outLint < 1)
                    {
                        msg = "Please provide a valid lower Interval.";
                        return null;
                    }
                    else
                    {
                        l1 = outLint;
                    }
                }

                var upperInter = dv.Row["upper_interval"].ToString().Trim();
                if (string.IsNullOrEmpty(upperInter))
                {
                    msg = "Please provide Upper Interval.";
                    return null;
                }
                else
                {
                    double outUpint;
                    var dtResult = double.TryParse(upperInter, out outUpint);
                    if (!dtResult && outUpint < 1)
                    {
                        msg = "Please provide a valid Upper Interval.";
                        return null;
                    }
                    else
                    {
                        up1 = outUpint;
                    }
                }

                if (!string.IsNullOrEmpty(dv.Row["equipmentused"].ToString().Trim()))
                {
                    var dfv = dv.Row["equipmentused"].ToString().Trim();
                    var equipmentId = new EquipmentServices().GetEquipmentId(dfv);
                    if (equipmentId < 1)
                    {
                        msg = "Equipment Information could not be processed";
                        return null;
                    }

                    mInfo.EquipmentId = equipmentId;
                }
                else
                {
                    msg = "Equipment used empty.";
                    return null;
                }

                mInfo.WellCompletionTypeId = completionTypeId;

                var vvz = new WellCompletionServices().AddWellCompletionCheckDuplicate3(mInfo);
                if (vvz < 1)
                {
                    msg = "Well Completion Information could not be processed.";
                    return null; 
                }

                var wellCompInt = new WellCompletionInterval
                {
                    LowerInterval = l1,
                    WellCompletionId = vvz,
                    UpperInterval = up1,
                    DateCompleted = mInfo.DateCompleted.ToString(),
                    LastUpdatedTime = DateTime.Now.ToString("G")
                };

                var vvzk = new WellCompletionIntervalServices().AddWellCompletionIntervalCheckDuplicate(wellCompInt);
                if (vvzk < 1)
                {
                    msg = "Well Completion Information could not be processed.";
                    return null;
                }

                return  mInfo;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }

        }

        private WellCompletion ProcessRecord2(DataRowView dv, ref string msg, int completionTypeId)
        {
            if (dv == null) { return null; }
            try
            {
                var wellName = dv.Row["wellname"].ToString().Replace(" ", "").Replace(",", "").Trim();

                if (string.IsNullOrEmpty(wellName.Trim()))
                {
                    return null;
                }

                var mInfo = new WellCompletion
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

                var year = dv.Row["datecompleted(yyyy/MM/dd)"].ToString().Trim();
                if (string.IsNullOrEmpty(year))
                {
                    msg = "Completion Date is empty.";
                    return null;
                }
                else
                {
                    DateTime outYear;
                    var yrResult = DateTime.TryParse(year, out outYear);
                    if (!yrResult)
                    {
                        msg = "Completion Date is empty.";
                        return null;
                    }
                    else
                    {
                        mInfo.DateCompleted = outYear;
                    }
                }

                var completionStatus = dv.Row["isinitialcompletion(YES/NO)"].ToString().Trim();

                if (string.IsNullOrEmpty(completionStatus))
                {
                    msg = "Please indicate if Well Completion is Initial or ortherwise.";
                    return null;
                }
                else
                {
                    if (completionStatus.Trim().ToLower().Replace(" ", "") == "yes")
                    {
                        mInfo.IsInitial = true;
                    }
                    if (completionStatus.Trim().ToLower().Replace(" ", "") == "no")
                    {
                        mInfo.IsInitial = false;
                    }
                }
                double l1;
                double up1;
                double l2;
                double up2;
                var lowerInter1 = dv.Row["lower_interval_1"].ToString().Trim();
                if (string.IsNullOrEmpty(lowerInter1))
                {
                    msg = "Please provide lower Interval 1.";
                    return null;
                }
                else
                {
                    double outLint;
                    var dtResult = double.TryParse(lowerInter1, out outLint);
                    if (!dtResult && outLint < 1)
                    {
                        msg = "Please provide a valid lower Interval 1.";
                        return null;
                    }
                    else
                    {
                        l1 = outLint;
                    }
                }

                var upperInter = dv.Row["upper_interval_1"].ToString().Trim();
                if (string.IsNullOrEmpty(upperInter))
                {
                    msg = "Please provide Upper Interval 1.";
                    return null;
                }
                else
                {
                    double outUpint;
                    var dtResult = double.TryParse(upperInter, out outUpint);
                    if (!dtResult && outUpint < 1)
                    {
                        msg = "Please provide a valid Upper Interval 1.";
                        return null;
                    }
                    else
                    {
                        up1 = outUpint;
                    }
                }

                var lowerInter2 = dv.Row["lower_interval_2"].ToString().Trim();
                if (string.IsNullOrEmpty(lowerInter2))
                {
                    msg = "Please provide lower Interval 2.";
                    return null;
                }
                else
                {
                    double outLint2;
                    var dtResult = double.TryParse(lowerInter2, out outLint2);
                    if (!dtResult && outLint2 < 1)
                    {
                        msg = "Please provide a valid lower Interval 2.";
                        return null;
                    }
                    else
                    {
                        l2 = outLint2;
                    }
                }
                
                var upperInter2 = dv.Row["upper_interval_2"].ToString().Trim();
                if (string.IsNullOrEmpty(upperInter2))
                {
                    msg = "Please provide Upper Interval 2.";
                    return null;
                }
                else
                {
                    double outUpint2;
                    var dtResult = double.TryParse(upperInter2, out outUpint2);
                    if (!dtResult && outUpint2 < 1)
                    {
                        msg = "Please provide a valid Upper Interval 2.";
                        return null;
                    }
                    else
                    {
                        up2 = outUpint2;
                    }
                }


                if (!string.IsNullOrEmpty(dv.Row["equipmentused"].ToString().Trim()))
                {
                    var dfv = dv.Row["equipmentused"].ToString().Trim();
                    var equipmentId = new EquipmentServices().GetEquipmentId(dfv);
                    if (equipmentId < 1)
                    {
                        msg = "Equipment Information could not be processed";
                        return null;
                    }

                    mInfo.EquipmentId = equipmentId;
                }
                else
                {
                    msg = "Equipment used empty.";
                    return null;
                }

                mInfo.WellCompletionTypeId = completionTypeId;

                var vvz = new WellCompletionServices().AddWellCompletionCheckDuplicate3(mInfo); 
                if (vvz < 1)
                {
                    msg = "Well Completion Interval for the same Period, Reason, Completion Type and Equipment already exists for this well.";
                    return null;
                }

                var wellCompInt = new WellCompletionInterval
                {
                    LowerInterval = l1,
                    WellCompletionId = vvz,
                    UpperInterval = up1,
                    DateCompleted = mInfo.DateCompleted.ToString(),
                    LastUpdatedTime = DateTime.Now.ToString("G")
                };

                var vvzk = new WellCompletionIntervalServices().AddWellCompletionIntervalCheckDuplicate(wellCompInt);
                if (vvzk < 1)
                {
                    msg = "Well Completion Information could not be processed.";
                    return null;
                }

                var tdd = new WellCompletionInterval
                {
                    LowerInterval = l2,
                    WellCompletionId = vvz,
                    UpperInterval = up2,
                    DateCompleted = mInfo.DateCompleted.ToString(),
                    LastUpdatedTime = DateTime.Now.ToString("G")
                };

                var vvzk2 = new WellCompletionIntervalServices().AddWellCompletionIntervalCheckDuplicate(tdd);
                if (vvzk2 < 1)
                {
                    new WellCompletionIntervalServices().DeleteWellCompletionIntervalCheckReferences(vvzk);
                    msg = "Well Completion Information could not be processed.";
                    return null;
                }

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
