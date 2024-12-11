using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace Lab1.Test
{
    public class DecisionTreeTests
    {
        readonly string path;

        public DecisionTreeTests()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();

            IoC.Resolve<ICommand>(
                "Scopes.Current.Set",
                IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")))
            .Execute();

            var tree = new Dictionary<int, object>();
            IoC.Resolve<ICommand>(
                "IoC.Register", "Game.DecisionTree",
                (object[] args) => tree
            ).Execute();

            path = @"../../../Vectors.txt";
        }

        [Fact]
        public void BuildDecisionTreeWithoutExceptions()
        {
            var decisionTreeCommand = new DecisionTree(path);
            decisionTreeCommand.Execute();

            var tree = IoC.Resolve<Dictionary<int, object>>("Game.DecisionTree");

            Assert.True(
                tree.ContainsKey(4)
            );

            Assert.True(
                ((Dictionary<int, object>)tree[4]).ContainsKey(6)
            );

            Assert.True(
                ((Dictionary<int, object>)((Dictionary<int, object>)tree[4])[6]).ContainsKey(4)
            );

            Assert.True(
                ((Dictionary<int, object>)((Dictionary<int, object>)((Dictionary<int, object>)tree[4])[6])[4]).ContainsKey(6)
            );
        }
        [Fact]
        public void ImpossibleToReadFileWhenBuildDecisionTree()
        {
            var wrongPath = "wrongVectors";
            var decisionTreeCommand = new DecisionTree(wrongPath);
            Assert.Throws<FileNotFoundException>(decisionTreeCommand.Execute);
        }
    }
}
