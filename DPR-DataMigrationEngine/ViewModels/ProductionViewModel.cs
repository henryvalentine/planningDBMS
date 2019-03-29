using System.Collections.Generic;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;

namespace DPR_DataMigrationEngine.ViewModels
{
    public class ProductionViewModel
    {
        public List<Product> Products { get; set; }
        public List<Field> Fields { get; set; }
        public List<Terrain> Terrains { get; set; }
        public List<Company> Companies { get; set; }
        public List<Zone> Zones { get; set; }
        public List<Block> Blocks { get; set; }
        public List<BlockType> BlockTypes { get; set; }
        public List<Production> Productions { get; set; }
        public List<DocObject> YearList { get; set; }
        public List<DocObject> MonthList { get; set; }
        public List<ProductionQuery> ProductionQueries { get; set; }
        public List<ProductionObject> ProductionObjects { get; set; }
    }
}
