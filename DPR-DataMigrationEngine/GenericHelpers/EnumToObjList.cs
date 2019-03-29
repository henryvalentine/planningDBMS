using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DPR_DataMigrationEngine.GenericHelpers;

namespace DPR_DataMigrationEngine.GenericHelpers
{
   public class EnumToObjList
    {
        public  List<DocObject> ConvertEnumToList(Type enumType)
        {
            if (enumType == null)
                return new List<DocObject>();
            var numArray = (int[])Enum.GetValues(enumType);
            var names = Enum.GetNames(enumType);
            var arrayList = new List<DocObject>();
            try
            {
                for (int index = 0; index < numArray.GetLength(0); ++index)
                {
                    var doc = new DocObject
                    {
                        DocId = numArray[index]
                    };
                    if (names[index].IndexOf("_", StringComparison.Ordinal) > -1)
                        names[index] = names[index].Replace("_", " ");
                    doc.DocName = names[index];
                    arrayList.Add(doc);
                }
            }
            catch (Exception ex)
            {
                return new List<DocObject>();
            }
            return arrayList;
        }

        public List<DocObject> GeneratYearList(int startYear, int stopYear)
        {
            try
            {
                if (startYear < 1 || stopYear < 1 || (stopYear < startYear))
                {
                    return new List<DocObject>();
                }

                var yearList = new List<DocObject>();

                for (long i = startYear; i < stopYear + 1; i++)
                {
                    var doc = new DocObject
                    {
                        DocId = i,
                        DocName = i.ToString(CultureInfo.InvariantCulture)
                    };
                    yearList.Add(doc); 
                } 
                return yearList;
            }
            catch (Exception ex)
            {
                return new List<DocObject>();
            }
        }

        public List<DocObject> GetMonthList()
        {
            try
            {
                var monthList = new EnumToObjList().ConvertEnumToList(typeof(MonthList));

                if (monthList == null || !monthList.Any())
                {
                    return new List<DocObject>();
                }

                return monthList;
            }
            catch (Exception ex)
            {
                return new List<DocObject>();
            }
        }

        public List<DocObject> GetWellReportFields()
        {
            try
            {
                var reportFields = new EnumToObjList().ConvertEnumToList(typeof(WellReportFields));

                if (reportFields == null || !reportFields.Any())
                {
                    return new List<DocObject>();
                }

                return reportFields;
            }
            catch (Exception ex)
            {
                return new List<DocObject>();
            }
        }

       public List<DocObject> GetWellWorkoverReportFields()
        {
            try
            {
                var reportFields = new EnumToObjList().ConvertEnumToList(typeof(WellWorkoverReportFields));

                if (reportFields == null || !reportFields.Any())
                {
                    return new List<DocObject>();
                }

                return reportFields;
            }
            catch (Exception ex)
            {
                return new List<DocObject>();
            }
        }
       

        public List<DocObject> GetWellCompletionReportFields()
        {
            try
            {
                var reportFields = new EnumToObjList().ConvertEnumToList(typeof(WellCompletionReportFields));

                if (reportFields == null || !reportFields.Any())
                {
                    return new List<DocObject>();
                }

                return reportFields;
            }
            catch (Exception ex)
            {
                return new List<DocObject>();
            }
        }

        public List<DocObject> GetFieldReportColumns()
        {
            try
            {
                var reportFields = new EnumToObjList().ConvertEnumToList(typeof(FieldReportColumns));

                if (reportFields == null || !reportFields.Any())
                {
                    return new List<DocObject>();
                }

                return reportFields;
            }
            catch (Exception ex)
            {
                return new List<DocObject>();
            }
        }
        public List<DocObject> GetProductionReportFields()
        {
            try
            {
                var reportFields = new EnumToObjList().ConvertEnumToList(typeof(ProductionReportFields));

                if (reportFields == null || !reportFields.Any())
                {
                    return new List<DocObject>();
                }

                return reportFields;
            }
            catch (Exception ex)
            {
                return new List<DocObject>();
            }
        }


        public List<DocObject> GetIncidentHistoryReportFields()
        {
            try
            {
                var reportFields = new EnumToObjList().ConvertEnumToList(typeof(IncidentHistoryReportFields));

                if (reportFields == null || !reportFields.Any())
                {
                    return new List<DocObject>();
                }

                return reportFields;
            }
            catch (Exception ex)
            {
                return new List<DocObject>();
            }
        }

        public List<DocObject> GetCompletionStatuses()
        {
            try
            {
                var completionStatuses = new EnumToObjList().ConvertEnumToList(typeof(CompletionStatus));

                if (completionStatuses == null || !completionStatuses.Any())
                {
                    return new List<DocObject>();
                }

                return completionStatuses;
            }
            catch (Exception ex)
            {
                return new List<DocObject>();
            }
        }
    }
    
}
