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
    public Vector(params int[] coordinates)
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
      if (obj == null || obj is not Vector)
        return false;
      else
        return coordinates.SequenceEqual(((Vector)obj).coordinates);
    }

    public override int GetHashCode()
    {
      return coordinates.GetHashCode();
    }
    public static Vector IsNotNull(Vector a)
    {
      if (a.coordinates.Length == 0)
      {
        throw new System.Exception();
      }
      return a;
    }
  }
}
