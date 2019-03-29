using System.Collections.Generic;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;

namespace DPR_DataMigrationEngine.ViewModels
{
    public class FieldViewModel
    {
        public List<Company> Companies { get; set; }
        public List<Terrain> Terrains { get; set; }
        public List<BlockType> BlockTypes { get; set; }
        public List<Field> Fields { get; set; }
        public List<Block> Blocks { get; set; }
        public List<Zone> Zones { get; set; }
        public List<FieldQuery> FieldQueries { get; set; }

       
    }
}
