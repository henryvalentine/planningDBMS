﻿using System.Collections.Generic;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;

namespace DPR_DataMigrationEngine.ViewModels
{
    public class WellWorkoverViewModel
    {
        public List<WellWorkOverReason> WellWorkOverReasons { get; set; }
        public List<Equipment> Equipments { get; set; }
        public List<Zone> Zones { get; set; }
        public List<Terrain> Terrains { get; set; }
        public List<Company> Companies { get; set; }
        public List<Well> Wells { get; set; }
        public List<WellObject> WellObjects { get; set; }
        public List<WellClass> WellClasses { get; set; }
        public List<WellType> WellTypes { get; set; }
        public List<WellWorkover> WellWorkOvers { get; set; }
        public List<DocObject> YearList { get; set; }
        public List<DocObject> MonthList { get; set; }
        public List<WellWorkoverQuery> WellWorkoverQueries { get; set; }
        public List<WellWorkoverReportObject> WellWorkoverReportObjects { get; set; }
    }
}
