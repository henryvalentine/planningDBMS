using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.EF.CustomizedModels
{
    public class WellCompletionObject
    {
        public string Error { get; set; }
        public int ErrorCode { get; set; }
        public string WellName { get; set; }
        public string EquipmentName { get; set; }
        public string WellCompletionTypeName { get; set; }
        public double L1 { get; set; }
        public double U1 { get; set; }
        public double L2 { get; set; }
        public double U2 { get; set; }
        public DateTime DateCompleted { get; set; }
        public String DatecomPletedString { get; set; }
        public int IntervalId1 { get; set; }
        public int IntervalId2 { get; set; }
        public int WellCompletionId { get; set; }
        public int WellId { get; set; }
        public int EquipmentId { get; set; }
        public int WellCompletionTypeId { get; set; }
        public bool IsInitial { get; set; }
        public virtual Equipment Equipment { get; set; }
        public virtual Well Well { get; set; }
        public virtual WellCompletionType WellCompletionType { get; set; }
        public virtual List<WellCompletionInterval> WellCompletionIntervals { get; set; }
    }
}
