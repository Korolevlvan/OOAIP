using Moq;

namespace Lab1.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Move_from_12_5_to_5_8_with_velocity_N7_3()
        {
            var move = new Mock<IMove>();

            move.SetupGet(m => m.Position).Returns(new Vector(12, 5)).Verifiable();
            move.SetupGet(m => m.Velocity).Returns(new Vector(-7, 3)).Verifiable();

            ICommand moveCommand = new CMove(move.Object);

            moveCommand.Execute();
            move.VerifySet(m => m.Position = new Vector(5, 8), Times.Once);
            move.VerifyAll();
        }
        [Fact]
        public void there_isnt_Position()
        {

            var move = new Mock<IMove>();
            try
            {
                move.SetupGet(m => m.Position).Returns(new Vector()).Verifiable();
                move.SetupGet(m => m.Velocity).Returns(new Vector(-5, 3)).Verifiable();

                ICommand moveCommand = new CMove(move.Object);

                moveCommand.Execute();
                moveCommand.Check();
            }
            catch (System.Exception)
            {
                var result = false;
                Assert.False(result);
            }
        }
        [Fact]
        public void there_isnt_Velocity()
        {

            var move = new Mock<IMove>();
            try
            {
                move.SetupGet(m => m.Position).Returns(new Vector(12, 5)).Verifiable();
                move.SetupGet(m => m.Velocity).Returns(new Vector()).Verifiable();

                ICommand moveCommand = new CMove(move.Object);
                moveCommand.Execute();
                moveCommand.Check();
            }
            catch (System.Exception)
            {
                var result = false;
                Assert.False(result);
            }

        }
        [Fact]
        public void there_isnt_MoveAbility()
        {

            var move = new Mock<IMove>();
            try
            {
                move.SetupGet(m => m.Position).Returns(new Vector(12, 5)).Verifiable();
                move.SetupGet(m => m.Velocity).Returns(new Vector(-5, 3)).Verifiable();
                move.SetupGet(m => m.MoveAbility).Returns(new MoveAbility(false)).Verifiable();

                ICommand moveCommand = new CMove(move.Object);

                moveCommand.Execute();
                moveCommand.MoveAbilityCheck();
            }
            catch (System.Exception)
            {
                var result = false;
                Assert.False(result);
            }
        }
    }
}