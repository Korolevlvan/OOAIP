using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
  public interface IUObject
  {
    void set_property(string key, object value);
    object get_property(string key);
  }
}
