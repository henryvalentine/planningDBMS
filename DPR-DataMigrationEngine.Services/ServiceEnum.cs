using System.ComponentModel;

// ReSharper disable InconsistentNaming
namespace DPR_DataMigrationEngine.Services
{

    public enum WellCompletionTypeEnum
    {
        Single = 1,
        Dual = 2
    }

    public enum Months
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
  

}
