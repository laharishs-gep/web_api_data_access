using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace data_breach.Models
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string Collection { get; set; }
    }

    public interface IDatabaseSettings
    { 
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string Collection { get; set; }
    }
}
