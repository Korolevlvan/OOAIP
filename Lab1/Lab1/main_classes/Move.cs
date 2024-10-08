using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.main_classes
{
    public interface IMovable
    {
        public int[] Position { get; set; }
        public int[] Velocity { get; }
    }

    public class CMove : ICommand
    {
        private readonly IMovable movable;
        public CMove(IMovable movable)
        {
            this.movable = movable;
        }
        public void Execute()
        {
            movable.Position = new int[]{
            movable.Position[0] +  movable.Velocity[0],
            movable.Position[1] +  movable.Velocity[1],
        };
        }
    }
}
