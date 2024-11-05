using Moq;
using Xunit.Abstractions;
namespace Lab1.Test
{
    public class UnitTest1
    {

        public UnitTest1()
        {
        }
        [Fact]
        public void Move_from_12_5_to_5_8_with_velocity_N7_3()
        {
            var move = new Mock<IMove>();
            int[] P = { 12, 5 };
            int[] V = { -7, 3 };
            int[] A = { 5, 8};
            move.SetupGet(m => m.Position).Returns(new Vector(P)).Verifiable();
            move.SetupGet(m => m.Velocity).Returns(new Vector(V)).Verifiable();

            CMove moveCommand = new CMove(move.Object);



            moveCommand.Execute();
            move.VerifySet(m => m.Position = new Vector(A), Times.Once);
            move.VerifyAll();
        }
        [Fact]
        public void there_isnt_Position()
        {

            var move = new Mock<IMove>();
            int[] V = { -7, 3};
            move.SetupGet(m => m.Position).Throws(() => new Exception()).Verifiable();
            move.SetupGet(m => m.Velocity).Returns(new Vector(V)).Verifiable();

            CMove moveCommand = new CMove(move.Object);


            Assert.Throws<Exception>(moveCommand.Execute);

            
        }
        [Fact]
        public void there_isnt_Velocity()
        {

            var move = new Mock<IMove>();
            
            
            int[] P = { 12, 5 };
            move.SetupGet(m => m.Position).Returns(new Vector(P)).Verifiable();
            move.SetupGet(m => m.Velocity).Throws(() => new Exception()).Verifiable();

            CMove moveCommand = new CMove(move.Object);

            Assert.Throws<Exception>(moveCommand.Execute);
            

        }
        [Fact]
        public void there_isnt_MoveAbility()
        {

            var move = new Mock<IMove>();
            
            int[] P = { 12, 5 };
            int[] V = { -7, 3 };
            int[] A = { 5, 8 };
            move.SetupGet(m => m.Position).Returns(new Vector(P)).Verifiable();
            move.SetupGet(m => m.Velocity).Returns(new Vector(V)).Verifiable();

            CMove moveCommand = new CMove(move.Object);

            move.SetupSet(m => m.Position = new Vector(A)).Throws(() => new Exception()).Verifiable();

            Assert.Throws<Exception>(moveCommand.Execute);
        }
    }
}