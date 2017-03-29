using System;
using System.Data.Entity.ModelConfiguration;
using QM.Core.Model;

namespace QM.Data.Maps
{
    public class TaskMap : EntityTypeConfiguration<Tasks>
    {
        public TaskMap()
        {
            //primary key
            HasKey(p => p.idx);

            //table & column mappings
            ToTable("QM_TASK");
        }         
    }
}
