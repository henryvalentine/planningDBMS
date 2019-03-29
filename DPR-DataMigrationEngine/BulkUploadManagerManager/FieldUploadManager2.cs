using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.Services.ServiceManager;
using WebGrease.Css.Extensions;

namespace DPR_DataMigrationEngine.BulkUploadManagerManager
{
    public class FieldUploadManager2
    {
        public bool Import(string filePath, string sheetName, ref List<Field> mList, ref string msg)
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

            var selectString = @"SELECT [Field_Name], [Block_Name], [Terrain_Name], [Technical_Allowable], [Company_Name], [Canonial_Name] FROM [" + sheetName + "$]";
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
                    msg = "Invalid Field Template!";
                    return false;
                }
                var dv = new DataView(ds.Tables[0]);
                if (dv.Count < 1)
                {
                    msg = "Invalid Field Template!";
                    return false;
                }

                msg = string.Empty;
                var sb = new StringBuilder();
                sb.AppendLine("<table width=\"98%\" cellspacing=\"1px\" border=\"1\" cellpadding=\"2px\">");
                sb.AppendLine(string.Format("<tr><th width=\"45%\">Field Name</th><th width=\"55%\">Error</th></tr>"));
                var errorExist = false;
                for (var i = 0; i < dv.Count; i++)
                {
                    var mymsg = string.Empty;

                    var projectName = dv[i].Row["Field_Name"].ToString().Trim();

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
        private Field ProcessRecord(DataRowView dv, ref string msg)
        {
            if (dv == null) { return null; }
            try
            {
                var mInfo = new Field
                {
                    Name = dv.Row["Field_Name"].ToString().Trim()
                };

                //[Field_Name], [Block_Name]

                var companyName = dv.Row["Company_Name"].ToString().Trim();
                if (string.IsNullOrEmpty(companyName))
                {
                    mInfo.CompanyId = 1;
                }
                else
                {

                    var cannonicalName = "";
                    if (string.IsNullOrEmpty(dv.Row["Canonial_Name"].ToString().Trim()))
                    {
                        cannonicalName = dv.Row["Canonial_Name"].ToString().Trim();
                    }
                    
                    var companyId = new CompanyServices().GetCompanyId(companyName, cannonicalName);
                    if (companyId < 1)
                    {
                        mInfo.CompanyId = 1;
                    }
                    mInfo.CompanyId = companyId;
                }

                var terrainName = dv.Row["Terrain_Name"].ToString().Trim();
                if (string.IsNullOrEmpty(terrainName))
                {
                    mInfo.TerrainId = 1;
                }
                else
                {
                    var terrainId = new TerrainServices().GetTerrainIdByName(terrainName.Trim());
                    if (terrainId < 1)
                    {
                        mInfo.TerrainId = 1;
                    }
                    mInfo.TerrainId = terrainId;
                }

                mInfo.ZoneId = 1;

                var techAllowed = dv.Row["Technical_Allowable"].ToString().Trim();
                if (!string.IsNullOrEmpty(techAllowed))
                {
                    float outTech = 0;
                    var techResult = float.TryParse(techAllowed, out outTech);
                    if (techResult && outTech > 0)
                    {
                        mInfo.TechnicalAllowable = outTech;
                    }

                }

                var blockName = dv.Row["Block_Name"].ToString().Trim();
                if (string.IsNullOrEmpty(blockName))
                {
                    msg = "Block Name is empty";
                    return mInfo;;
                }

                var ttsd = new FieldServices().AddFieldCheckDuplicate(mInfo);
                if (ttsd < 1)
                {
                    msg = "Field could not be processed.";
                    return null;
                }


               
                if (blockName.Contains("/"))
                {
                  var dxx =  blockName.Split('/');
                    dxx.ForEach(v =>
                    {
                        if (!string.IsNullOrEmpty(v))
                        {
                            var tId = 0;
                            if (v.Trim().ToLower().Contains("oml"))
                            {
                                tId = 1;
                            }
                            if (v.Trim().ToLower().Contains("opl"))
                            {
                                tId = 2;
                            }

                            var block = new Block
                            {
                                CompanyId = mInfo.CompanyId,
                                BlockTypeId = tId,
                                Name = blockName,
                                LeaseTypeId = 1,
                                YearOfAward = DateTime.Now.Year
                            };

                            var ssd = new BlockServices().AddBlockCheckDuplicate(block);
                            if (ssd < 1)
                            {
                                return;
                            }
                        }

                      
                    });
                }

                else
                {
                    var tId = 0;
                    if (blockName.Trim().ToLower().Contains("oml"))
                    {
                        tId = 1;
                    }
                    if (blockName.Trim().ToLower().Contains("opl"))
                    {
                        tId = 2;
                    }
                    var block = new Block
                    {
                        CompanyId = mInfo.CompanyId,
                        BlockTypeId = tId,
                        Name = blockName,
                        LeaseTypeId = 1,
                        YearOfAward = DateTime.Now.Year
                    };

                    var ssd = new BlockServices().AddBlockCheckDuplicate(block);
                    if (ssd < 1)
                    {
                        mInfo.FieldId = ttsd;
                        return mInfo;
                    }
                }
                mInfo.FieldId = ttsd;
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
