using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.BulkUploadManagerManager
{
    public class ProjectMilestoneUploadManager
    {
        public bool Import(string filePath, string sheetName, ref List<ProjectMileStone> mList, ref string msg)
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

            var selectString = @"SELECT [ProjectMilestoneTitle], [Description], [DateDue(dd/MM/yy)] FROM [" + sheetName + "$]";
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
                    msg = "Invalid Project Milestone Template!";
                    return false;
                }
                var dv = new DataView(ds.Tables[0]);
                if (dv.Count < 1)
                {
                    msg = "Invalid Project Milestone Template!";
                    return false;
                }

                msg = string.Empty;
                var sb = new StringBuilder();
                sb.AppendLine("<table width=\"98%\" cellspacing=\"1px\" border=\"1\" cellpadding=\"2px\">");
                sb.AppendLine(string.Format("<tr><th width=\"45%\">Project Milestone Name</th><th width=\"55%\">Error</th></tr>"));
                var errorExist = false;
                for (var i = 0; i < dv.Count; i++)
                {
                    var mymsg = string.Empty;

                    var projectMilestoneTitle = dv[i].Row["ProjectMilestoneTitle"].ToString().Trim();

                    if (string.IsNullOrEmpty(projectMilestoneTitle))
                    {
                        continue;
                    }
                    var mInfo = ProcessRecord(dv[i], ref mymsg);
                    if (mInfo == null)
                    {
                        errorExist = true;
                        sb.AppendLine(mymsg.Length > 0
                                          ? string.Format(
                                              "<tr border=\"1\"><td width=\"45%\">{0}</td><td width=\"55%\">{1}</td></tr>", projectMilestoneTitle,
                                              mymsg)
                                          : string.Format(
                                              "<tr border=\"1\"><td width=\"45%\">{0}</td><td width=\"55%\">Unknown Error</td></tr>",
                                              projectMilestoneTitle));
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
        private ProjectMileStone ProcessRecord(DataRowView dv, ref string msg)
        {
            if (dv == null) { return null; }
            try
            {
                var mInfo = new ProjectMileStone
                {
                    Title = dv.Row["ProjectMilestoneTitle"].ToString().Trim()
                };
                
                if (!string.IsNullOrEmpty(dv.Row["Description"].ToString().Trim()))
                {
                    mInfo.Description = dv.Row["Description"].ToString().Trim();
                }
                
                if (!string.IsNullOrEmpty(dv.Row["DateDue(dd/MM/yy)"].ToString().Trim()))
                {
                    DateTime ddt;
                    var dtResult = DateTime.TryParse(dv.Row["DateDue(dd/MM/yy)"].ToString().Trim(), out ddt);

                    if (!dtResult)
                    {
                        msg = "Invalid Date Due";
                        return null;
                    }

                    mInfo.DateDue = ddt;
                }
                
                return mInfo;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

    }

}
