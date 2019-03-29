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
    public class ProductionUploadManager
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

            var selectString = @"SELECT [field], [product],[quantity(barrels)], [APIgravity], [month(eg: January)], [year(eg: 2010)], [remark] FROM [" + sheetName + "$]";
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
                    var fieldName = dv[i].Row["field"].ToString().Trim();
                    if (string.IsNullOrEmpty(fieldName.Trim()))
                    {
                        continue;
                    }
                    if (fieldName.Trim().Length < 2) { continue; }
                    var mInfo = ProcessRecord(dv[i], ref mymsg);
                    if (mInfo == null)
                    {
                        errorExist = true;
                        sb.AppendLine(mymsg.Length > 0
                                          ? string.Format(
                                              "<tr border=\"1\"><td width=\"45%\">{0}</td><td width=\"55%\">{1}</td></tr>", fieldName,
                                              mymsg)
                                          : string.Format(
                                              "<tr border=\"1\"><td width=\"45%\">{0}</td><td width=\"55%\">Unknown Error</td></tr>",
                                              fieldName));
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
                var quantity = dv.Row["quantity(barrels)"].ToString().Trim();
                double qt = 0;
                if (string.IsNullOrEmpty(quantity))
                {
                    msg = "Quantity produced is empty.";
                    return null; 
                }
                double outQt = 0;
                var dt = double.TryParse(quantity, out outQt);
                if (!dt && outQt < 1)
                {
                    msg = "Invalid value for Quantity produced.";
                    return null;
                }

                var mInfo = new Production
                {
                    Quantity = outQt.ToString(CultureInfo.InvariantCulture)
                            
                };

                var field = dv.Row["field"].ToString().Trim();
                if (string.IsNullOrEmpty(field))
                {
                    msg = "Production Field is empty.";
                    return null;
                }

                var fieldInfo = new FieldServices().GetFieldIdByFieldName(field);
                if (fieldInfo.FieldId < 1)
                {
                    msg = "Field Information could not be processed.";
                    return null;
                }

                mInfo.FieldId = fieldInfo.FieldId;

                var product = dv.Row["product"].ToString().Trim();
                if (string.IsNullOrEmpty(product))
                {
                    msg = "Product is empty.";
                    return null;
                }

                var productId = new ProductServices().GetProductIdId(product);
                if (productId < 1)
                {
                    msg = "Product Information could not be processed.";
                    return null;
                }

                mInfo.ProductId = productId;
                var month = dv.Row["month(eg: January)"].ToString().Trim();
                if (string.IsNullOrEmpty(month))
                {
                    msg = "Month of Production is empty";
                    return null;
                }
                var monthValue = 0;
                switch (month.Trim().ToLower())
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

                var year = dv.Row["year(eg: 2010)"].ToString().Trim();
                
                if (string.IsNullOrEmpty(month))
                {
                    msg = "Year of Production is empty";
                    return null;
                }

                long yearValue;
                var yearResult = long.TryParse(year, out yearValue);
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
          
                if (!string.IsNullOrEmpty(dv.Row["APIgravity"].ToString().Trim()))
                {
                    mInfo.APIGravity = dv.Row["APIgravity"].ToString().Trim();
                }

                if (!string.IsNullOrEmpty(dv.Row["remark"].ToString().Trim()))
                {
                    mInfo.Remark = dv.Row["remark"].ToString().Trim();
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
