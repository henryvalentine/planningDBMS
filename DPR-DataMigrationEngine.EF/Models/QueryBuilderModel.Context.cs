﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DPR_DataMigrationEngine.EF.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class QueryBuilderEntities : DbContext
    {
        public QueryBuilderEntities()
            : base("name=QueryBuilderEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<FieldQuery> FieldQueries { get; set; }
        public virtual DbSet<ProductionQuery> ProductionQueries { get; set; }
        public virtual DbSet<WellCompletionQuery> WellCompletionQueries { get; set; }
        public virtual DbSet<WellWorkoverQuery> WellWorkoverQueries { get; set; }
        public virtual DbSet<WellQuery> WellQueries { get; set; }
        public virtual DbSet<IncidentQuery> IncidentQueries { get; set; }
    }
}
