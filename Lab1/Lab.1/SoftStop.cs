using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hwdtech;
namespace Lab1
{
  public class SoftStopCommand : ICommand
  {
    private readonly ServerThread _t;
    private readonly Action _a;
    public SoftStopCommand(ServerThread t, Action a)
    {
      _t = t;
      _a = a;
    }

    public void Execute()
    {
      if (_t.Equals(Thread.CurrentThread))
      {
        var old_behaviour = _t.GetBehaviour();
        Action new_behaviour = () =>
        {
          if (_t.IsNotEmpty())
          {
            old_behaviour();
          }
          else
          {
            IoC.Resolve<ICommand>("Server.Commands.HardStop", _t, _a).Execute();
          }
        };
        _t.SetBehaviour(new_behaviour);
      }
    }
  }
}
