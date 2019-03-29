﻿using System.Collections.Generic;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;

namespace DPR_DataMigrationEngine.ViewModels
{
    public class WellViewModel
    {
        public List<WellType> WellTypes { get; set; }
        public List<WellObject> WellObjects { get; set; }
        public List<Company> Companies { get; set; }
        public List<Well> Wells { get; set; }
        public List<WellClass> WellClasses { get; set; }
        public List<Block> Blocks { get; set; }
        public List<Terrain> Terrains { get; set; }
        public List<Field> Fields { get; set; }
        public List<Zone> Zones { get; set; }
        public List<WellQuery> WellQueries { get; set; }
        public List<DocObject> SpudYears { get; set; }
        public List<WellReportObject> WellReportObjects { get; set; }
    } 
}
