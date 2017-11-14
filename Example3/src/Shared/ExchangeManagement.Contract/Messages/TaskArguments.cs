using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeManagement.Contract.Messages
{
    public class TaskCalculationResult
    {
        public long Id { get; set; }

        public long Result { get; set; }
    }

    public class TaskEntity
    {
        public long Id { get; set; }
        public long A { get; set; }
        public long B { get; set; }

        public long? Result { get; set; }
    }

    public class TaskArguments
    {
        public long Id { get; set; }
        public long A { get; set; }
        public long B { get; set; }
        public bool IsReady { get; set; }
    }

    public abstract class ExchangerNames
    {
        public const string Tasks = "CalculationTasks";
    }

    public abstract class MessageTopics
    {
        public const string Sum = "Sum";
    }

    public interface IFinishedProductService
    {
        bool IsReady(TaskArguments taskArguments);
    }

    public class StubFinishedProductService: IFinishedProductService
    {
        public bool IsReady(TaskArguments taskArguments)
        {
            if (taskArguments == null)
            {
                return false;
            }

            return taskArguments.IsReady;
        }
    }    
}
