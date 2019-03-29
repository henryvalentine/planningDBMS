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
    public class ProductionUploadManager2
    {
        public bool Import(string filePath, string sheetName, ref List<Production> mList, ref string msg)
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

            var selectString = @"SELECT [FieldName],[Product], [Quantity], [APIGravity], [Month], [Year] FROM [" + sheetName + "$]";
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
                    msg = "Invalid Production Template!";
                    return false;
                }
                var dv = new DataView(ds.Tables[0]);
                if (dv.Count < 1)
                {
                    msg = "Invalid Production Template!";
                    return false;
                }

                msg = string.Empty;
                var sb = new StringBuilder();
                sb.AppendLine("<table width=\"98%\" cellspacing=\"1px\" border=\"1\" cellpadding=\"2px\">");
                sb.AppendLine(string.Format("<tr><th width=\"45%\">Production Name</th><th width=\"55%\">Error</th></tr>"));
                var errorExist = false;
                for (var i = 0; i < dv.Count; i++)
                {
                    var mymsg = string.Empty;
                    var mInfo = ProcessRecord(dv[i], ref mymsg);
                    if (mInfo == null)
                    {
                        errorExist = true;
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
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }
        private Production ProcessRecord(DataRowView dv, ref string msg)
        {
            if (dv == null) { return null; }
            try
            {
               var quantity = dv.Row["Quantity"].ToString().Replace(" ", "").Replace(",", "").Trim();
                
               var qt = string.IsNullOrEmpty(quantity) ? "0" : quantity;

                var mInfo = new Production
                {
                    Quantity = qt,
                };

                var fieldName = dv.Row["FieldName"].ToString().Trim();
                mInfo.FieldId = new FieldServices().GetFieldIdByName(fieldName);
                var productName = dv.Row["Product"].ToString().Trim();
                mInfo.ProductId = new ProductServices().GetProductIdId(productName);

                var month = dv.Row["Month"].ToString().Trim();
                if (string.IsNullOrEmpty(month))
                {
                    mInfo.Month = DateTime.Now.Month;
                }
                else
                {
                    var monthValue = 0;
                    switch (month.Trim().Replace(" ", string.Empty).ToLower())
                    {

                        case "january":
                            monthValue = 1;
                            break;

                        case "february":
                            monthValue = 2;
                            break;

                        case "march":
                            monthValue = 3;
                            break;

                        case "april":
                            monthValue = 4;
                            break;

                        case "may":
                            monthValue = 5;
                            break;

                        case "june":
                            monthValue = 6;
                            break;

                        case "july":
                            monthValue = 7;
                            break;

                        case "august":
                            monthValue = 8;
                            break;

                        case "september":
                            monthValue = 9;
                            break;

                        case "october":
                            monthValue = 10;
                            break;

                        case "november":
                            monthValue = 11;
                            break;

                        case "december":
                            monthValue = 12;
                            break;

                    }

                    mInfo.Month = monthValue;
                }
                

                var year = dv.Row["Year"].ToString().Trim();

                if (!string.IsNullOrEmpty(year))
                {
                    long yearValue;
                    var yearResult = long.TryParse(dv.Row["Year"].ToString().Trim(), out yearValue);
                    if (!yearResult || yearValue < 1)
                    {
                        msg = "Invalid Year of Production";
                        return null;
                    }

                    if (yearValue < 1)
                    {
                        msg = "Invalid Year of Production";
                        return null;
                    }

                    mInfo.Year = yearValue;
                }

                else
                {
                    mInfo.Year = 2014;
                }

                if (!string.IsNullOrEmpty(dv.Row["APIGravity"].ToString().Trim()))
                {
                    mInfo.APIGravity = dv.Row["APIGravity"].ToString().Trim();
                }

                var status = new ProductionServices().AddProduction(mInfo);
                return status > 0 ? mInfo : null;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }

        }

    }

}
