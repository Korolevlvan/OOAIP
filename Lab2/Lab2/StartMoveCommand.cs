using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hwdtech;
namespace Lab2
{
    internal class StartMoveCommand: ICommand
    {
        private readonly IStartMoveCommand _start;

        public StartMoveCommand(IStartMoveCommand start) => _start = start;

        public void Execute()
        {

            _start.Properties.ToList().ForEach(a => IoC.Resolve<ICommand>("Properties.Set", _start.Target, a.Key, a.Value).Execute());
            var cmd = IoC.Resolve<ICommand>("Operations.Movement", _start.Target);
            var injectable = IoC.Resolve<ICommand>("Commands.Injectable", cmd);
            IoC.Resolve<ICommand>("Properties.Set", _start.Target, "Operations.Movement", cmd).Execute();
            IoC.Resolve<IQueue>("Game.Queue").Put(injectable);
        }
    }
}
