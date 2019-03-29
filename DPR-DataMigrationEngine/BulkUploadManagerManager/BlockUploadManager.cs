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
    public class BlockUploadManager
    {
        public bool Import(string filePath, string sheetName, ref List<Block> mList, ref string msg)
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

            var selectString = @"SELECT [BlockName(eg:OML150)],[CompanyName],[Area],[Lease_Type],[Year_Of_Award(eg:2010)] FROM [" + sheetName + "$]";
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
                    msg = "Invalid Block Template!";
                    return false;
                }
                var dv = new DataView(ds.Tables[0]);
                if (dv.Count < 1)
                {
                    msg = "Invalid Block Template!";
                    return false;
                }

                msg = string.Empty;
                var sb = new StringBuilder();
                sb.AppendLine("<table width=\"98%\" cellspacing=\"1px\" border=\"1\" cellpadding=\"2px\">");
                sb.AppendLine(string.Format("<tr><th width=\"45%\">Block Name</th><th width=\"55%\">Error</th></tr>"));
                var errorExist = false;
                for (var i = 0; i < dv.Count; i++)
                {
                    var mymsg = string.Empty;

                    var projectName = dv[i].Row["BlockName(eg:OML150)"].ToString().Trim();

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


        private Block ProcessRecord(DataRowView dv, ref string msg)
        {
            if (dv == null) { return null; }
            try
            {
                var mInfo = new Block
                {
                    Name = dv.Row["BlockName(eg:OML150)"].ToString().Trim()
                };
                
                if (mInfo.Name.Trim().Replace(" ", "").ToLower().Contains("opl"))
                {
                    mInfo.BlockTypeId = 2;
                }

                if (mInfo.Name.Trim().Replace(" ", "").ToLower().Contains("oml"))
                {
                    mInfo.BlockTypeId = 1;
                }

                var companyName = dv.Row["CompanyName"].ToString().Trim();

                if (string.IsNullOrEmpty(companyName))
                {
                    msg = "Company Name is empty";
                    return null;
                }

                var companyId = new CompanyServices().GetCompanyId(companyName);

                if (companyId < 1)
                {
                    msg = "Company information could not be processed.";
                    return null;
                }

                mInfo.CompanyId = companyId;

                var area = dv.Row["Area"].ToString().Trim();

                if (!string.IsNullOrEmpty(area))
                {
                    float outArea;
                    var areaResult = float.TryParse(area, out outArea);
                    if (areaResult && outArea > 0)
                    {
                        mInfo.Area = outArea;
                    }
                }

                var yearOfAward = dv.Row["Year_Of_Award(eg:2010)"].ToString().Trim();

                if (!string.IsNullOrEmpty(yearOfAward))
                {
                    long outyearOfAward;
                    var yearOfAwardResult = long.TryParse(yearOfAward, out outyearOfAward);
                    if (yearOfAwardResult)
                    {
                        mInfo.YearOfAward = outyearOfAward;
                    }
                }

                var leaseTypeName = dv.Row["Lease_Type"].ToString().Trim();

                if (string.IsNullOrEmpty(leaseTypeName))
                {
                    msg = "Lease Type Name is empty";
                    return null;
                }
                var leaseTypeId = new LeaseTypeServices().GetLeaseTypeIdByName(leaseTypeName);

                if (leaseTypeId < 1)
                {
                    return null;
                }
                mInfo.LeaseTypeId = leaseTypeId;
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
