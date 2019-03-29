using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;

namespace DPR_DataMigrationEngine.BulkUploadManagerManager
{

    public class WellUploadManager2
    {
        public bool Import(string filePath, string sheetName, ref List<Well> mList, ref string msg)
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

            var selectString = @"SELECT [Well_Name],[WellType_Name],[WellClass_Name],[Total_Dept],[Technical_Allowed], [SPud_Date], [Remarks], [block] FROM [" + sheetName + "$]";
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
                    msg = "Invalid Well Template!";
                    return false;
                }
                var dv = new DataView(ds.Tables[0]);
                if (dv.Count < 1)
                {
                    msg = "Invalid Well Template!";
                    return false;
                }

                msg = string.Empty;
                var sb = new StringBuilder();
                sb.AppendLine("<table width=\"98%\" cellspacing=\"1px\" border=\"1\" cellpadding=\"2px\">");
                sb.AppendLine(string.Format("<tr><th width=\"45%\">Well Name</th><th width=\"55%\">Error</th></tr>"));
                var errorExist = false;
                for (var i = 0; i < dv.Count; i++)
                {
                    var mymsg = string.Empty;
                    var wellName = dv[i].Row["Well_Name"].ToString().Trim();
                    if (wellName.Trim().Length < 3) { continue; }
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
                myCon.Close();
                msg = ex.Message;
                return false;
            }
        }
        private Well ProcessRecord(DataRowView dv, ref string msg)
        {
            if (dv == null) { return null; }
            try
            {
                var mInfo = new Well
                {
                    Name = dv.Row["Well_Name"].ToString().Trim(),
                };
                
                var spuDate = dv.Row["SPud_Date"].ToString().Trim();
                DateTime d1;
                if (!string.IsNullOrEmpty(spuDate))
                {
                    var realDate = DateTime.TryParse(spuDate, out d1);
                    if (!realDate)
                    {
                        mInfo.SpudDate = DateTime.Now;
                    }
                    mInfo.SpudDate = d1;
                }

                else
                {
                    mInfo.SpudDate = DateTime.Now;
                }
               

                double dept = 0;
                if (!string.IsNullOrEmpty(dv.Row["Total_Dept"].ToString().Trim()))
                {
                    var totalDeptStr = dv.Row["Total_Dept"].ToString().Trim();

                    var deptResult = double.TryParse(totalDeptStr, out dept);
                    if (!deptResult || dept < 1)
                    {
                        dept = 0;
                    }
                }

                mInfo.TotalDept = dept;

                var remarks = dv.Row["Remarks"].ToString().Trim();

                if (!string.IsNullOrEmpty(remarks))
                {
                    mInfo.Remarks = remarks.Trim();
                }

                var wellTypeName = dv.Row["WellType_Name"].ToString().Trim();

                if (!string.IsNullOrEmpty(wellTypeName))
                {
                    var wellTypeId = new WellTypeServices().GetWellTypeId(wellTypeName);
                    mInfo.WellTypeId = wellTypeId;
                }
                else
                {
                    mInfo.WellTypeId = (int)OtherNotAvailable.Not_Available;
                }

                var techAllowed = dv.Row["Technical_Allowed"].ToString().Trim();
                if (!string.IsNullOrEmpty(techAllowed))
                {
                    double outRes;
                    var techAllowedResult = double.TryParse(techAllowed, out outRes);
                    if (techAllowedResult && outRes > 0)
                    {
                        mInfo.TechnicalAllowable = outRes;
                    }

                }


                //var blockName = dv.Row["Field_Name"].ToString().Trim();

                //if (!string.IsNullOrEmpty(blockName))
                //{
                //    var blockId = new WellTypeServices().GetWellTypeId(wellTypeName);
                //    if (blockId < 1)
                //    {
                //        mInfo.WellTypeId = (int)OtherNotAvailable.Not_Available;
                //    }
                //    else
                //    {
                //        mInfo.BlockId = blockId;
                //    }
                    
                //}
                //else
                //{
                //    mInfo.WellTypeId = (int)OtherNotAvailable.Not_Available;
                //}
               
                var wellId = new WellServices().AddWellCheckDuplicate2(mInfo);

                if (wellId > 0)
                {
                    var wellClassName = dv.Row["WellClass_Name"].ToString().Trim();

                    if (!string.IsNullOrEmpty(wellClassName))
                    {
                        var status = new WellClasServices().CreateWellClassAddClassification(wellClassName, wellId);
                        if (status < 1)
                        {
                           msg = "Well could not be classified";
                            return null;
                        }
                    }
                }
                mInfo.WellId = wellId;
                return mInfo;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace,ex.Source,ex.Message);
                return null;
            }

        }
       
        private bool ValidateDateCompleted(string date)
        {
            DateTime d1;
            var realDate = DateTime.TryParse(date, out d1);
            if (!realDate)
            {
                return false;
            }

            return true;
        }

    }

}
