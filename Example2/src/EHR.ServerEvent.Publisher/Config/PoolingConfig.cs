using EHR.ServerEvent.Publisher.Dequeue.Postgres;

namespace EHR.ServerEvent.Publisher.Config
{
    public class PoolingConfig
    {
        public PoolingConfig()
        {
            ReceiversCount = 1;
        }

        public PoolingOptions PoolingOptions { get; set; } 
        
        public int ReceiversCount { get; set; }
    }

}
