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

            int i = moveCommand.Check();


            moveCommand.Execute();
            move.VerifySet(m => m.Position = new Vector(A), Times.Once);
            move.VerifyAll();
        }
        [Fact]
        public void there_isnt_Position()
        {

            var result = true;
            var move = new Mock<IMove>();
            try
            {
                int[] P = { 0};
                int[] V = { -7, 3};
                move.SetupGet(m => m.Position).Returns(new Vector(P)).Verifiable();
                move.SetupGet(m => m.Velocity).Returns(new Vector(V)).Verifiable();

                CMove moveCommand = new CMove(move.Object);


                moveCommand.Execute();
                int i  = moveCommand.Check();
                if (i < 3) result = false;
            }
            catch (System.Exception)
            {
                result = false;
            }
            Assert.False(result);
        }
        [Fact]
        public void there_isnt_Velocity()
        {

            var result = true;
            var move = new Mock<IMove>();
            try
            {
                int[] P = { 12, 5 };
                int[] V = {0};
                move.SetupGet(m => m.Position).Returns(new Vector(P)).Verifiable();
                move.SetupGet(m => m.Velocity).Returns(new Vector(V)).Verifiable();
                move.SetupGet(m => m.Ability).Returns(new MoveAbility(true)).Verifiable();

                CMove moveCommand = new CMove(move.Object);
                int j = moveCommand.MoveAbilityCheck();
                if (j < 2) result = false;
                int i  = moveCommand.Check();
                if (i < 3) result = false;
            }
            catch (System.Exception)
            {
                result = false;
                Assert.False(result);
            }

        }
        [Fact]
        public void there_isnt_MoveAbility()
        {

            var result = true;
            var move = new Mock<IMove>();
            try
            {
                int[] P = { 12, 5 };
                int[] V = { -7, 3 };
                move.SetupGet(m => m.Position).Returns(new Vector(P)).Verifiable();
                move.SetupGet(m => m.Velocity).Returns(new Vector(V)).Verifiable();
                move.SetupGet(m => m.Ability).Returns(new MoveAbility(false)).Verifiable();

                CMove moveCommand = new CMove(move.Object);

                moveCommand.Execute();
                int j = moveCommand.MoveAbilityCheck();
                if (j < 2) result = false;
            }
            catch (System.Exception)
            {
                result = false;
                Assert.False(result);
            }
        }
    }
}