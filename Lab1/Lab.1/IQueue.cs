using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
  public interface IQueue
  {
    void Put(ICommand a);
    ICommand Take();
  }
}
