using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using QM.Data.Maps;

namespace QM.Data
{
    public class QMDBContext : DbContext
    {
        public QMDBContext() : base("QMDbContext")
        {
            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("WHFRONT");
            modelBuilder.Configurations.Add(new TaskMap());
            base.OnModelCreating(modelBuilder);
        }
    }
}
