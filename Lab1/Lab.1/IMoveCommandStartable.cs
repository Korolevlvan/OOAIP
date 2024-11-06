using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
  public interface IMoveCommandStartable
  {
    IUObject Target { get; }
    Dictionary<string, object> Properties { get; }
  }
}
