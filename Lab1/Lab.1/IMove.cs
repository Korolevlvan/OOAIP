using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
  public interface IMove
  {
    public Vector Position { get; set; }
    public Vector Velocity { get;  }
    public MoveAbility Ability { get; }
  }
}
