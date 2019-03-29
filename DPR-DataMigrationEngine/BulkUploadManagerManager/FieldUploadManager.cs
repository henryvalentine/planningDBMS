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
    public class FieldUploadManager
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

            var selectString = @"SELECT [Field_Name],[Block_Name], [Company_Name],[Zone_Name], [Terrain_Name], [Technical_Allowable] FROM [" + sheetName + "$]";
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

                var zoneName = dv.Row["Zone_Name"].ToString().Trim();
                if (string.IsNullOrEmpty(zoneName))
                {
                    msg = "Zone Name is empty";
                    return null;
                }

                var zoneId = new ZoneServices().GetZoneIdByName(zoneName.Trim());
                if (zoneId < 1)
                {
                    msg = "Zone Information could not be processed.";
                    return null;
                }
                mInfo.ZoneId = zoneId;
                
                var terrainName = dv.Row["Terrain_Name"].ToString().Trim();
                if (string.IsNullOrEmpty(zoneName))
                {
                    msg = "Terrain Name is empty";
                    return null;
                }

                var terrainId = new TerrainServices().GetTerrainIdByName(terrainName.Trim());
                if (terrainId < 1)
                {
                    msg = "Terrain Information could not be processed.";
                    return null;
                }
                mInfo.TerrainId = terrainId;
                
                var blockName = dv.Row["Block_Name"].ToString().Trim();
                if (string.IsNullOrEmpty(blockName))
                {
                    msg = "Block Name is empty";
                    return null;
                }

                var blockId = new BlockServices().GetBlockId(blockName.Trim());
                if (blockId < 1)
                {
                    msg = "Block Information could not be processed.";
                    return null;
                }
                mInfo.BlockId = blockId;


                var companyName = dv.Row["Company_Name"].ToString().Trim();
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
