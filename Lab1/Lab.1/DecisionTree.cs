using Hwdtech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
  public class DecisionTree: ICommand
  {
    private readonly string _path;
    public DecisionTree(string path) => _path = path;

    public void Execute()
    {
      var vectors = File.ReadAllLines(_path).Select(
          line => line.Split().Select(int.Parse).ToList()
      ).ToList();

      var tree = IoC.Resolve<Dictionary<int, object>>("Game.DecisionTree");

      vectors.ForEach(vector => {
        var layer = tree;
        vector.ForEach(num => {
          layer.TryAdd(num, new Dictionary<int, object>());
          layer = (Dictionary<int, object>)layer[num];
        });
      });
    }
  }
}
