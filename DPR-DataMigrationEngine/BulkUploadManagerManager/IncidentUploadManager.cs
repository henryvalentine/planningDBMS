using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.Services.ServiceManager;

namespace DPR_DataMigrationEngine.BulkUploadManagerManager
{

    public class IncidentUploadManager
    {
        public bool Import(string filePath, string sheetName, ref List<IncidentHistory> mList, ref string msg)
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

            var selectString = @"SELECT [CompanyName],[IncidentType],[IncidentTitle], [Location], [Description], [Date(yyyy/MM/dd)], [ReportedBy] FROM [" + sheetName + "$]";
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
                    msg = "Invalid Incident Template!";
                    return false;
                }
                var dv = new DataView(ds.Tables[0]);
                if (dv.Count < 1)
                {
                    msg = "Invalid Incident Template!";
                    return false;
                }

                msg = string.Empty;
                var sb = new StringBuilder();
                sb.AppendLine("<table width=\"98%\" cellspacing=\"1px\" border=\"1\" cellpadding=\"2px\">");
                sb.AppendLine(string.Format("<tr><th width=\"45%\">Incident Name</th><th width=\"55%\">Error</th></tr>"));
                var errorExist = false;
                for (var i = 0; i < dv.Count; i++)
                {
                    var mymsg = string.Empty;
                    var incidentHistoryTitle = dv[i].Row["IncidentTitle"].ToString().Trim();
                    if (string.IsNullOrEmpty(incidentHistoryTitle.Trim()))
                    {
                        continue;
                    }
                    var mInfo = ProcessRecord(dv[i], ref mymsg);
                    if (mInfo == null)
                    {
                        errorExist = true;
                        sb.AppendLine(mymsg.Length > 0
                                          ? string.Format(
                                              "<tr border=\"1\"><td width=\"45%\">{0}</td><td width=\"55%\">{1}</td></tr>", incidentHistoryTitle,
                                              mymsg)
                                          : string.Format(
                                              "<tr border=\"1\"><td width=\"45%\">{0}</td><td width=\"55%\">Unknown Error</td></tr>",
                                              incidentHistoryTitle));
                        continue;
                    }
                    mList.Add(mInfo);
                }
                sb.AppendLine("</table>");
                if (errorExist)
                {
                    var sbb = new StringBuilder();
                    sbb.AppendLine("Following error occurred while loading Incident data template:");
                    sbb.AppendLine(sb.ToString());
                    msg = sbb.ToString();
                }
               myCon.Close();
                return true;
            }
            catch (Exception ex)
            {
                myCon.Close();
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                msg = "Process failed";
                return false;
            }
        }
        private IncidentHistory ProcessRecord(DataRowView dv, ref string msg)
        {
            if (dv == null) { return null; }
            try
            {
               var mInfo = new IncidentHistory
                {
                    Title = dv.Row["IncidentTitle"].ToString().Trim()
                            
                };

               //[CompanyName],[IncidentType],[IncidentTitle] GetIncidentTypeId


               var incidentName = dv.Row["IncidentType"].ToString().Trim();
               if (string.IsNullOrEmpty(incidentName))
               {
                   msg = "Incident Name is empty";
                   return null;
               }

               var incidentId = new IncidentTypeServices().GetIncidentTypeId(incidentName.Trim());
               if (incidentId < 1)
               {
                   msg = "Incident Information could not be processed.";
                   return null;
               }
                mInfo.IncidentTypeId = incidentId;

               var companyName = dv.Row["CompanyName"].ToString().Trim();
               if (string.IsNullOrEmpty(companyName))
               {
                   msg = "Company Name is empty";
                   return null;
               }

               var companyId = new CompanyServices().GetCompanyId(companyName.Trim());
               if (companyId < 1)
               {
                   msg = "Company Information could not be processed.";
                   return null;
               }
               mInfo.CompanyId = companyId;

               var incidentDate = dv.Row["Date(yyyy/MM/dd)"].ToString().Trim();
                if (!ValidateDateCompleted(incidentDate))
                {
                    msg = "Invalid input : Incident Date";
                    return null;
                }
                DateTime d1;
                var realDate = DateTime.TryParse(incidentDate, out d1);
                if (!realDate || d1.Year == 0001)
                {
                    msg = "Invalid input : Incident Date";
                    return null;
                }

                mInfo.IncidentDate = d1;

                var location = dv.Row["Location"].ToString().Trim();
                if (string.IsNullOrEmpty(location.Trim()))
                {
                    msg = "Incident Location is empty.";
                    return null;
                }
               
                mInfo.Location = location;

                var description = dv.Row["Description"].ToString().Trim();

                if (string.IsNullOrEmpty(description.Trim()))
                {
                    msg = "Incident Description is empty.";
                    return null;
                }
                mInfo.Description = description;

                var incidentReporter = dv.Row["ReportedBy"].ToString().Trim();
                if (!string.IsNullOrEmpty(incidentReporter.Trim()))
                {
                    mInfo.ReportedBy = incidentReporter;
                }
               
                return mInfo;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
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
