using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{


  public class Vector
  {
    private int[] coordinates;
    private int coord_cont;
    private int[] a = { 0 };

    public Vector(int[] coordinates)
    {
      this.coordinates = coordinates;
      this.coord_cont = coordinates.Length;
    }
    public static Vector operator +(Vector a, Vector b)
    {
      Vector c = new(new int[a.coord_cont]);
      c.coordinates = (a.coordinates.Select((x, index) => x + b.coordinates[index]).ToArray());
      return c;
    }

    

    public override bool Equals(object? obj)
    {
        return coordinates.SequenceEqual(((Vector)obj).coordinates);
    }

    public Boolean IsNotNull()
    {
      if (this.coord_cont < 2)
      {
        return false;
      }
      return true;
    }
  }
}
