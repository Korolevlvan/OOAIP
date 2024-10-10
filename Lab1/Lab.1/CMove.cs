using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    

    public class CMove : ICommand
    {
        public readonly IMove move;
        public CMove(IMove move)
        {
            this.move = move;
        }
        public void Execute()
        {
          move.Position += move.Velocity;
        }
        public int Check()
        {
          if (this.move.Position.IsNotNull() == false)
          {
              return 1;
          }
          if (this.move.Velocity.IsNotNull() == false)
          {
              return 2;
           }
            return 3;
          }
        public int MoveAbilityCheck()
        {
          if (move.Ability.moveability != true)
          {
            return 1;
          }
          return 2;
        }
  }
}
