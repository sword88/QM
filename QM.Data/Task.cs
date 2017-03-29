using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.Core.Model;
using QM.Data.Repository;

namespace QM.Data
{
    public class Task 
    {
        private IRepository<Tasks> taskRepository = new Repository<Tasks>();

        public int Insert(Tasks t)
        {
            return taskRepository.Insert(t);
        }

        public List<Tasks> GetList()
        {
            return taskRepository.FindList("select * from qm_task");
        }

        public int Update(Tasks t)
        {
            return taskRepository.Update(t);
        }
    }
}
