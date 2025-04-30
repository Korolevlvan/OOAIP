using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
  public class ActionCommand : Hwdtech.ICommand
  {
    private readonly Action _action;
    public ActionCommand(Action action) => _action = action;
    public void Execute()
    {
      _action();
    }
  }
}
