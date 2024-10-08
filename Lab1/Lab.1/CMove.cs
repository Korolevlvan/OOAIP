using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    

    public class CMove : ICommand
    {
        private readonly IMove move;
        public CMove(IMove move)
        {
            this.move = move;
        }
        public void Execute()
        {
          move.Position += move.Velocity;
        }
        public void Check()
        {
          Vector.IsNotNull(move.Position);
          Vector.IsNotNull(move.Velocity);
        }
        public void MoveAbilityCheck()
        {
          if (move.MoveAbility == new MoveAbility(false))
          {
            throw new System.Exception();
          }
        }
  }
}
