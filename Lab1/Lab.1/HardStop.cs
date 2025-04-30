using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
  public class HardStopCommand : Hwdtech.ICommand
  {
    private readonly ServerThread _t;
    public HardStopCommand(ServerThread t)
    {
      _t = t;
    }

    public void Execute()
    {
      _t.Stop();
    }
  }
}
