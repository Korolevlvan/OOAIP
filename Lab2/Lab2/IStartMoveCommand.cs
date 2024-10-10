using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    public interface IStartMoveCommand
    {
        IUObject Target { get; }
        Dictionary<string, object> Properties { get; }
    }
}
