using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.Core.Model;
using QM.Data.Repository;

namespace QM.Data
{
    public class UnitOfWork : IDisposable
    {        
        private Repository<Tasks> taskRepository;
        public Repository<Tasks> TaskRepository
        {
            get { return new Repository<Tasks>(); }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
