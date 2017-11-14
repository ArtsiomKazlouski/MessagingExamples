using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangeManagement.Contract.Messages;

namespace ExchangeManagement.Host.WebApi.TasksDatabase
{
    public interface ITaskRepository
    {
        TaskEntity Create(TaskEntity task);
        TaskEntity Get(long taskId);
        TaskEntity Update(TaskEntity taskId);
    }

    public class InMemoryTaskRepository:ITaskRepository
    {
        private Dictionary<long,TaskEntity> _taskEntities = new Dictionary<long, TaskEntity>();

        public TaskEntity Create(TaskEntity task)
        {
            var id = _taskEntities.Count;
            task.Id = id;
            _taskEntities.Add(id,task);

            return task;
        }

        public TaskEntity Get(long taskId)
        {
            return _taskEntities[taskId];
        }

        public TaskEntity Update(TaskEntity task)
        {
            _taskEntities[task.Id] = task;
            return task;
        }
    }
}
