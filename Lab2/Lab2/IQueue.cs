using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lab2
{
    public interface IQueue
    {
        void Put(ICommand cmd);
        ICommand Take();
    }
}
