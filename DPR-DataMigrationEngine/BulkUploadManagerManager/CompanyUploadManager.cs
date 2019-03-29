using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using CsvHelper;

namespace DPR_DataMigrationEngine.BulkUploadManagerManager
{

    public class CompanyUploadManager
    {
        public bool Import(string filePath, string sheetName, ref List<Company> mList, ref string msg)
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

            //var reader = new CsvReader(sr);
            //IEnumerable<Company> records = reader.GetRecords<Company>();

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

            var selectString = @"SELECT [RCNumber], [name],[CanonialName], [address], [contactfirstname] , [contactlastname], [contactemail] , [contactphone], [designation]  FROM [" + sheetName + "$]";
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
                    msg = "Invalid Company Template!";
                    return false;
                }
                var dv = new DataView(ds.Tables[0]);
                if (dv.Count < 1)
                {
                    msg = "Invalid Company Template!";
                    return false;
                }

                msg = string.Empty;
                var sb = new StringBuilder();
                sb.AppendLine("<table width=\"98%\" cellspacing=\"1px\" border=\"1\" cellpadding=\"2px\">");
                sb.AppendLine(string.Format("<tr><th width=\"45%\">Company Name</th><th width=\"55%\">Error</th></tr>"));
                var errorExist = false;
                for (var i = 0; i < dv.Count; i++)
                {
                    var mymsg = string.Empty;
                    var company = dv[i].Row["name"].ToString().Trim();
                    if (company.Trim().Length < 3) { continue; }
                    var mInfo = ProcessRecord(dv[i], ref mymsg);
                    if (mInfo == null)
                    {
                        errorExist = true;
                        sb.AppendLine(mymsg.Length > 0
                                          ? string.Format(
                                              "<tr border=\"1\"><td width=\"45%\">{0}</td><td width=\"55%\">{1}</td></tr>", company,
                                              mymsg)
                                          : string.Format(
                                              "<tr border=\"1\"><td width=\"45%\">{0}</td><td width=\"55%\">Unknown Error</td></tr>",
                                              company));
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
        private Company ProcessRecord(DataRowView dv, ref string msg)
        {
            if (dv == null) { return null; }
            try
            {
                if (string.IsNullOrEmpty(dv.Row["name"].ToString().Trim()))
                {
                    msg = "Please provide Company Name";
                    return null;
                }

                var mInfo = new Company
                {
                    Name = dv.Row["name"].ToString().Trim(),
                    
                };

                var address = dv.Row["address"].ToString().Trim();
                if (!string.IsNullOrEmpty(address))
                {
                    mInfo.Address = address;
                }

                var rcNumber = dv.Row["RCNumber"].ToString().Trim();
                if (!string.IsNullOrEmpty(rcNumber))
                {
                    mInfo.RCNumber = rcNumber;
                }

                var cxx = dv.Row["CanonialName"].ToString().Trim();
                if (!string.IsNullOrEmpty(cxx))
                {
                    mInfo.CanonialName = cxx.Trim();
                }
                
                mInfo.People = new Collection<Person>();

                var firstName = dv.Row["contactfirstname"].ToString().Trim();

                if (!string.IsNullOrEmpty(firstName))
                {
                    var mpInfo = new Person
                    {
                        FirstName = firstName,

                    };

                    var lastName = dv.Row["contactlastname"].ToString().Trim();

                    if (!string.IsNullOrEmpty(lastName))
                    {
                        mpInfo.LastName = lastName;
                    }

                    var email = dv.Row["contactemail"].ToString().Trim();

                    if (!string.IsNullOrEmpty(email))
                    {
                        mpInfo.Email = email;
                    }

                    var phoneNumber = dv.Row["contactphone"].ToString().Trim();

                    if (!string.IsNullOrEmpty(phoneNumber))
                    {
                        mpInfo.PhoneNumber = phoneNumber;
                    }

                    var designation = dv.Row["designation"].ToString().Trim();

                    if (!string.IsNullOrEmpty(designation))
                    {
                        mpInfo.Designation = designation;
                    }

                    mInfo.People.Add(mpInfo);
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
