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
    public class ProjectUploadManager
    {
        public bool Import(string filePath, string sheetName, ref List<Project> mList, ref string msg)
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

            var selectString = @"SELECT [CompanyName], [ProjectName], [ProjectType], [Terrain], [Sector], [Duration(Months)], [Cost(Million)], [Description], [Objectives], [CompletionStatus(Completed/UnCompleted)], [DateCompleted(yyyy/MM/dd)] FROM [" + sheetName + "$]";
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
                    msg = "Invalid Project Template!";
                    return false;
                }
                var dv = new DataView(ds.Tables[0]);
                if (dv.Count < 1)
                {
                    msg = "Invalid Project Template!";
                    return false;
                }

                msg = string.Empty;
                var sb = new StringBuilder();
                sb.AppendLine("<table width=\"98%\" cellspacing=\"1px\" border=\"1\" cellpadding=\"2px\">");
                sb.AppendLine(string.Format("<tr><th width=\"45%\">Project Name</th><th width=\"55%\">Error</th></tr>"));
                var errorExist = false;
                for (var i = 0; i < dv.Count; i++)
                {
                    var mymsg = string.Empty;
                  
                    var projectName = dv[i].Row["ProjectName"].ToString().Trim();

                    if (string.IsNullOrEmpty(projectName))
                    {
                        continue;
                    }
                    var mInfo = ProcessRecord(dv[i], ref mymsg);
                    if (mInfo == null)
                    {
                        errorExist = true;
                        sb.AppendLine(mymsg.Length > 0
                                          ? string.Format(
                                              "<tr border=\"1\"><td width=\"45%\">{0}</td><td width=\"55%\">{1}</td></tr>", projectName,
                                              mymsg)
                                          : string.Format(
                                              "<tr border=\"1\"><td width=\"45%\">{0}</td><td width=\"55%\">Unknown Error</td></tr>",
                                              projectName));
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
                msg = "Bulk upload failed";
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }
        private Project ProcessRecord(DataRowView dv, ref string msg)
        {
            if (dv == null) { return null; }
            try
            {
                var mInfo = new Project
                {
                     Name  = dv.Row["ProjectName"].ToString().Trim()
                };
                
               var companyName = dv.Row["CompanyName"].ToString().Trim();
                if (string.IsNullOrEmpty(companyName))
                {
                    msg = "Company Name is empty";
                    return null;
                }
                var compId = new CompanyServices().GetCompanyId(companyName);

                if(compId < 1)
                {
                    msg = "Company information could not be processed.";
                    return null;
                }

                mInfo.CompanyId = compId;



                var projectType = dv.Row["ProjectType"].ToString().Trim();
                if (string.IsNullOrEmpty(projectType))
                {
                    msg = "project Type is empty";
                    return null;
                }
                var projectTypeId = new ProjectTypeServices().GetProjectTypeId(projectType);

                if (projectTypeId < 1)
                {
                    msg = "Project Type information could not be processed.";
                    return null;
                }

                mInfo.ProjectTypeId = projectTypeId;

                var terrain = dv.Row["Terrain"].ToString().Trim();
                if (string.IsNullOrEmpty(terrain))
                {
                    msg = "Terrain is empty";
                    return null;
                }
                var terrainId = new TerrainServices().GetTerrainId(terrain);

                if (terrainId < 1)
                {
                    msg = "Terrain information could not be processed.";
                    return null;
                }

                mInfo.TerrainId = terrainId;

                var sector = dv.Row["Sector"].ToString().Trim();
                if (string.IsNullOrEmpty(sector))
                {
                    msg = "Sector is empty";
                    return null;
                }
                var sectorId = new SectorServices().GetSectorId(sector);

                if (sectorId < 1)
                {
                    msg = "Sector information could not be processed.";
                    return null;
                }

                mInfo.SectorId = sectorId;

                var duration = dv.Row["Duration(Months)"].ToString().Trim();
                if (string.IsNullOrEmpty(duration))
                {
                    msg = "Project Duration is empty";
                    return null;
                }

                int outDur;
                var durResult = int.TryParse(duration, out outDur);
                if (!durResult || outDur < 1)
                {
                    msg = "Project Duration is not Valid";
                    return null;
                }

                mInfo.Duration = outDur;

                double cost;
                var costStr = double.TryParse(dv.Row["Cost(Million)"].ToString().Trim(), out cost);
                if (!costStr || cost <= 0)
                {
                    msg = "Invalid Project Cost" ;
                    return null;
                }

                mInfo.Cost = cost;

                if (!string.IsNullOrEmpty(dv.Row["Description"].ToString().Trim()))
                {
                    mInfo.Description = dv.Row["Description"].ToString().Trim();
                }

                if (!string.IsNullOrEmpty(dv.Row["Objectives"].ToString().Trim()))
                {
                    mInfo.ProjectObjectives = dv.Row["Objectives"].ToString().Trim();
                }
                
                if (!string.IsNullOrEmpty(dv.Row["CompletionStatus(Completed/UnCompleted)"].ToString().Trim()))
                {
                    var tts = dv.Row["CompletionStatus(Completed/UnCompleted)"].ToString().Trim().ToLower();
                    if (tts == "completed")
                    {
                        mInfo.CompletionStatus = 1;
                    }
                    if (tts == "uncompleted")
                    {
                        mInfo.CompletionStatus = 0;
                    }
                    
                }

                if (!string.IsNullOrEmpty(dv.Row["DateCompleted(yyyy/MM/dd)"].ToString().Trim()))
                {
                    DateTime ddt;
                    var dtResult = DateTime.TryParse(dv.Row["DateCompleted(yyyy/MM/dd)"].ToString().Trim(), out ddt);

                    if (!dtResult)
                    {
                        msg = "Invalid Project Completion Date";
                        return null;
                    }

                    mInfo.DateCompleted = ddt;
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
