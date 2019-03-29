using System.ComponentModel;

// ReSharper disable InconsistentNaming
namespace DPR_DataMigrationEngine.GenericHelpers
{
    public enum MonthList
    {
        [Description("January")]
        Janurary = 1,
        [Description("February")]
        February = 2,
        [Description("March")]
        March = 3,
        [Description("April")]
        April = 4,
        [Description("May")]
        May = 5,
        [Description("June")]
        June = 6,
        [Description("July")]
        July = 7,
        [Description("August")]
        August = 8,
        [Description("September")]
        September = 9,
        [Description("October")]
        October = 10,
        [Description("November")]
        November = 11,
        [Description("December")]
        December = 12
    }
    
    public enum WellReportFields
    {
        [Description("Company")]
        Company = 1,
        [Description(" Well Name")]
        Well_Name,
        [Description("Well Type")]
        Well_Type,
         [Description("Well Class")]
        Well_Class,
        [Description("Terrain")]
         Terrain,
        [Description("Zone")]
        Zone,
        [Description("Total Depth(FT)")]
        Total_Depth,
        [Description("SPU Date")]
        SPU_Date,
        //[Description("Rig Name")]
        //Rig_Name
    }
    
    public enum CompletionStatus
    {
        Completed = 1,
        Uncompleted
    }

    public enum ProjectMilestoneNotAvailable
    {
        Not_Available = 7
    }

    public enum OtherNotAvailable
    {
        Not_Available = 1
    }

    public enum WellCompletionTypeEnum
    {
        Single = 1,
        Dual = 2
    }
    public enum WellTypeEnum
    {
        Exploratory = 1,
        Appraisal,
        Development
    }
    public enum ProductionReportFields
    {

        [Description("Company")]
        Company = 1,
        [Description("Product")]
        Product,
        [Description("Quantity")]
        Quantity,
        [Description("Field")]
        Field,
        [Description("Block")]
        Block,
        [Description("Terrain")]
        Terrain,
        //[Description("Zone")]
        //Zone,
        [Description("Technical Allowable")]
        Technical_Allowable,
        [Description("Remark")]
        Remark
    }

    public enum IncidentHistoryReportFields
    {
        [Description("Title")]
        Title = 1,
        [Description("Type")]
        Type,
        [Description("Location")]
        Location,
        [Description("Description")]
        Description,
        [Description("Date")]
        Date,
        [Description("Company")]
        Company,
        [Description("Reported By")]
        Reported_By
    }

    public enum WellWorkoverReportFields
    {
        Company = 1,
        Well,
        Completion_Date,
        Terrain,
        Workover_Reason,
        Equipment_Used,
      
    }

    public enum WellCompletionReportFields
    {
        Company = 1,
        Well,
        Well_Type,
        Completion_Type,
        Completion_Interval,
        Date_Completed,
        Equipment_Used,
        Terrain
    }


    public enum FieldReportColumns
    {
        Field = 1,
        Block,
        Company,
        Technical_Allowable,
        Area,
        zone
    }

}
