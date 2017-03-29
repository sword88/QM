using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using QM.Core.Exception;
using System.Reflection;

namespace QM.Data.Repository
{
    public class Repository<TEntity> : IDisposable,IRepository<TEntity> where TEntity : class
    {
        private QMDBContext dbcontext = new QMDBContext();
        private DbTransaction dbtrans { get; set; }

        public void BeginTrans()
        {
            DbConnection dbcon = ((IObjectContextAdapter)dbcontext).ObjectContext.Connection;
            if (dbcon.State == ConnectionState.Closed)
            {
                dbcon.Open();
            }
            dbtrans = dbcon.BeginTransaction();            
        }

        public int Commit()
        {
            int result = 0;
            try
            {
                result = dbcontext.SaveChanges();
                if (dbtrans != null)
                {
                    dbtrans.Commit();
                }                
            }
            catch (QMException ex)
            {
                if (dbtrans != null)
                {
                    dbtrans.Rollback();
                }
            }
            finally
            {
                Dispose();
            }

            return result;
        }

        public void Dispose()
        {
            if (dbtrans != null)
            {
                dbtrans.Dispose();
            }
            dbcontext.Dispose();
        }

        public int Delete(TEntity entity)
        {
            dbcontext.Set<TEntity>().Attach(entity);
            dbcontext.Entry<TEntity>(entity).State = System.Data.Entity.EntityState.Deleted;

            return dbtrans == null ? Commit() : 0;
        }

        public TEntity Find(object key)
        {
            return dbcontext.Set<TEntity>().Find(key);
        }

        public List<TEntity> FindList(string sql)
        {
            return dbcontext.Database.SqlQuery<TEntity>(sql).ToList<TEntity>();
        }

        public int Insert(List<TEntity> entitys)
        {
            foreach (var entity in entitys)
            {
                dbcontext.Entry<TEntity>(entity).State = System.Data.Entity.EntityState.Added;
            }

            return dbtrans == null ? Commit() : 0;
        }

        public int Insert(TEntity entity)
        {
            dbcontext.Entry<TEntity>(entity).State = System.Data.Entity.EntityState.Added;
            return dbtrans == null ? Commit() : 0;
        }

        public IQueryable<TEntity> IQueryable()
        {
            return dbcontext.Set<TEntity>();
        }

        public IQueryable<TEntity> IQueryable(Expression<Func<TEntity, bool>> predicate)
        {
            return dbcontext.Set<TEntity>().Where(predicate);
        }        

        public int Update(TEntity entity)
        {
            dbcontext.Set<TEntity>().Attach(entity);
            PropertyInfo[] props = entity.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (prop.GetValue(entity, null) != null)
                {
                    if (prop.GetValue(entity, null).ToString() == "&nbsp;")
                        dbcontext.Entry(entity).Property(prop.Name).CurrentValue = null;
                    dbcontext.Entry(entity).Property(prop.Name).IsModified = true;
                }
            }
            return Commit();
        }
    }
}
