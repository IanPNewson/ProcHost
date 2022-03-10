using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcHost.Model
{
    public class Configuration
    {
        public List<ChildProcess> Children { get; set; } = new List<ChildProcess>();

        public List<SerializedType> Loggers { get; set; }= new List<SerializedType>();
    }
}
