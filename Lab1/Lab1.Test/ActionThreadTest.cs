using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Reflection.Metadata;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace Lab1.Test
{
    public class ServerThreadTest
    {
        public ServerThreadTest()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();

            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
                IoC.Resolve<object>("Scopes.New",
                    IoC.Resolve<object>("Scopes.Root")
                    )
                ).Execute();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Commands.HardStop", (object[] args) =>
            {
                if (args.Count() == 2)
                {
                    return new ActionCommand(() =>
                    {
                        if (((ServerThread)args[0]).Equals(Thread.CurrentThread))
                        {
                            new HardStopCommand((ServerThread)args[0]).Execute();
                            new ActionCommand((Action)args[1]).Execute();
                        }
                    });
                }
                return new ActionCommand(() =>
                {
                    if (((ServerThread)args[0]).Equals(Thread.CurrentThread))
                    {
                        new HardStopCommand((ServerThread)args[0]).Execute();
                    }
                });
            }).Execute();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Commands.SoftStop", (object[] args) =>
            {
                if (args.Count() == 2)
                {
                    return new SoftStopCommand((ServerThread)args[0], (Action)args[1]);
                }
                return new SoftStopCommand((ServerThread)args[0], () => { });
            }).Execute();

            var pill = new ActionCommand(() =>
            {
                IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Commands.HardStop", (object[] args) =>
                {
                    if (args.Count() == 2)
                    {
                        return new ActionCommand(() =>
                        {
                            if (((ServerThread)args[0]).Equals(Thread.CurrentThread))
                            {
                                new HardStopCommand((ServerThread)args[0]).Execute();
                                new ActionCommand((Action)args[1]).Execute();
                            }
                        });
                    }
                    return new ActionCommand(() =>
                    {
                        if (((ServerThread)args[0]).Equals(Thread.CurrentThread))
                        {
                            new HardStopCommand((ServerThread)args[0]).Execute();
                        }
                    });
                }).Execute();
            });

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "pill", (object[] args) => { return pill; }).Execute();

            var queueCollection = new Dictionary<Guid, BlockingCollection<Hwdtech.ICommand>>();
            var threadCollection = new Dictionary<Guid, ServerThread>();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Commands.RegisterThread", (object[] args) =>
            {
                return new ActionCommand(() =>
                {
                    queueCollection.Add((Guid)args[0], (BlockingCollection<Hwdtech.ICommand>)args[1]);
                    threadCollection.Add((Guid)args[0], (ServerThread)args[2]);
                });
            }).Execute();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Command.GetThreadQueue", (object[] args) =>
            {
                return queueCollection[(Guid)args[0]];
            }).Execute();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Commands.CreateStartThread", (object[] args) =>
            {
                var id = (Guid)args[0];
                Action action;
                if (args.Count() == 1)
                {
                    action = (() => { });
                }
                else
                {
                    action = (Action)args[1];
                }

                return new ActionCommand(() =>
                {
                    var q = new BlockingCollection<Hwdtech.ICommand>(100);
                    var t = new ServerThread(q);
                    t.Start();
                    IoC.Resolve<Hwdtech.ICommand>("Server.Commands.RegisterThread", id, q, t).Execute();
                    action();
                });
            }).Execute();


            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Commands.SendCommand", (object[] args) =>
            {
                var id = (Guid)args[0];
                var cmd = (Hwdtech.ICommand)args[1];
                var q = IoC.Resolve<BlockingCollection<Hwdtech.ICommand>>("Server.Command.GetThreadQueue", id);
                return new ActionCommand(() =>
                {
                    q.Add(cmd);
                });
            }).Execute();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetQueueCollection", (object[] args) => { return queueCollection; }).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetThreadCollection", (object[] args) => { return threadCollection; }).Execute();
        }

        [Fact]

        public void AnExceptionShouldNotStopServerThread()
        {
            var mre = new ManualResetEvent(false);
            var q = new BlockingCollection<Hwdtech.ICommand>(100);
            var t = new ServerThread(q);

            var usualCommand = new Mock<Hwdtech.ICommand>();
            usualCommand.Setup(m => m.Execute()).Verifiable();

            var exceptionCommand = new Mock<Hwdtech.ICommand>();
            exceptionCommand.Setup(m => m.Execute()).Throws<Exception>().Verifiable();

            var hardStopCommand = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.HardStop", t, () => { mre.Set(); });

            q.Add(IoC.Resolve<Hwdtech.ICommand>("pill"));

            var exceptionHandler = new Mock<Hwdtech.ICommand>();
            exceptionHandler.Setup(m => m.Execute()).Verifiable();

            q.Add(
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
                "ExceptionHandler.Handle", (object[] args) => exceptionHandler.Object
                ));

            q.Add(usualCommand.Object);
            q.Add(exceptionCommand.Object);
            q.Add(usualCommand.Object);
            q.Add(hardStopCommand);
            q.Add(usualCommand.Object);

            t.Start();
            mre.WaitOne();

            exceptionHandler.Verify(m => m.Execute(), Times.Once());
            usualCommand.Verify(m => m.Execute(), Times.Exactly(2));
            Assert.Single(q);
        }
        [Fact]

        public void HardStopCommandShouldStopServer()
        {
            var mre = new ManualResetEvent(false);
            var q = new BlockingCollection<Hwdtech.ICommand>(100);
            var t = new ServerThread(q);

            var hardStop = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.HardStop", t, () => { mre.Set(); });

            q.Add(IoC.Resolve<Hwdtech.ICommand>("pill"));
            q.Add(new ActionCommand(() => { Thread.Sleep(3000); }));
            q.Add(hardStop);
            q.Add(new ActionCommand(() => { }));

            t.Start();
            mre.WaitOne();

            Assert.Single(q);
        }
        [Fact]
        public void HardStopCommandDoesNotStopAnotherThread()
        {
            var mre = new ManualResetEvent(false);
            var q1 = new BlockingCollection<Hwdtech.ICommand>(100);
            var t1 = new ServerThread(q1);
            var q2 = new BlockingCollection<Hwdtech.ICommand>(100);
            var t2 = new ServerThread(q2);

            var endcmd = new Mock<Hwdtech.ICommand>();

            var wrongHardStop = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.HardStop", t2, () => { endcmd.Object.Execute(); });
            var hardStop1 = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.HardStop", t1);
            var hardStop2 = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.HardStop", t2, () => { mre.Set(); });

            q1.Add(IoC.Resolve<Hwdtech.ICommand>("pill"));
            q1.Add(new ActionCommand(() => { Thread.Sleep(1000); }));
            q1.Add(wrongHardStop);
            q1.Add(hardStop1);
            q1.Add(new ActionCommand(() => { }));

            q2.Add(IoC.Resolve<Hwdtech.ICommand>("pill"));
            q2.Add(new ActionCommand(() => { Thread.Sleep(3000); }));
            q2.Add(hardStop2);

            t1.Start();
            t2.Start();

            mre.WaitOne();

            endcmd.Verify(endcmd => endcmd.Execute(), Times.Never());
            Assert.Single(q1);
        }

        [Fact]
        public void SoftStopCommandShouldStopServer()
        {
            var mre = new ManualResetEvent(false);
            var q = new BlockingCollection<Hwdtech.ICommand>(100);
            var t = new ServerThread(q);

            var softStopCommand = new SoftStopCommand(t, () => { mre.Set(); });

            q.Add(IoC.Resolve<Hwdtech.ICommand>("pill"));
            q.Add(new ActionCommand(() => { Thread.Sleep(3000); }));
            q.Add(softStopCommand);
            q.Add(new ActionCommand(() => { }));

            t.Start();
            mre.WaitOne();

            Assert.Empty(q);
        }
        [Fact]
        public void SoftStop()
        {
            var mre = new ManualResetEvent(false);
            var q = new BlockingCollection<Hwdtech.ICommand>(100);
            var t = new ServerThread(q);
            var usualCommand = new Mock<Hwdtech.ICommand>();
            usualCommand.Setup(m => m.Execute()).Verifiable();
            var SoftStop = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SoftStop", t);

            q.Add(IoC.Resolve<Hwdtech.ICommand>("pill"));
            q.Add(usualCommand.Object);
            q.Add(SoftStop);
            q.Add(usualCommand.Object);
            q.Add(new ActionCommand(() => { mre.Set(); }));
            t.Start();
            mre.WaitOne();
            usualCommand.Verify(m => m.Execute(), Times.Exactly(2));
            Assert.Empty(q);
        }

        [Fact]
        public void SoftStopCommandDoesNotStopAnotherThread()
        {
            var mre = new ManualResetEvent(false);
            var q1 = new BlockingCollection<Hwdtech.ICommand>(100);
            var t1 = new ServerThread(q1);
            var q2 = new BlockingCollection<Hwdtech.ICommand>(100);
            var t2 = new ServerThread(q2);

            var endcmd = new Mock<Hwdtech.ICommand>();

            var wrongSoftStop = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SoftStop", t2, () => { endcmd.Object.Execute(); });
            var softStop1 = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SoftStop", t1);
            var softStop2 = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SoftStop", t2, () => { mre.Set(); });

            q1.Add(IoC.Resolve<Hwdtech.ICommand>("pill"));
            q1.Add(wrongSoftStop);

            q2.Add(IoC.Resolve<Hwdtech.ICommand>("pill"));
            q2.Add(new ActionCommand(() => { Thread.Sleep(3000); }));
            q2.Add(softStop2);

            t1.Start();
            t2.Start();

            q1.Add(new ActionCommand(() => { Thread.Sleep(1000); }));
            q1.Add(softStop1);
            q1.Add(new ActionCommand(() => { }));

            q2.Add(new ActionCommand(() => { Thread.Sleep(1000); }));
            q2.Add(softStop2);

            mre.WaitOne();

            endcmd.Verify(endcmd => endcmd.Execute(), Times.Never());
            Assert.Empty(q1);
        }
        [Fact]
        public void SendCommandTest()
        {
            Assert.Empty(IoC.Resolve<Dictionary<Guid, BlockingCollection<Hwdtech.ICommand>>>("GetQueueCollection"));

            var mre = new ManualResetEvent(false);
            var cmd = new Mock<Hwdtech.ICommand>();
            cmd.Setup(cmd => cmd.Execute()).Verifiable();
            var startcmd = new Mock<Hwdtech.ICommand>();
            startcmd.Setup(cmd => cmd.Execute()).Verifiable();

            Guid first = Guid.NewGuid();
            Guid second = Guid.NewGuid();
            IoC.Resolve<Hwdtech.ICommand>("Server.Commands.CreateStartThread", first, () => { startcmd.Object.Execute(); }).Execute();
            IoC.Resolve<Hwdtech.ICommand>("Server.Commands.CreateStartThread", second).Execute();

            Assert.True(IoC.Resolve<Dictionary<Guid, BlockingCollection<Hwdtech.ICommand>>>("GetQueueCollection").Count() == 2);

            IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SendCommand", first, cmd.Object).Execute();
            IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SendCommand", second, cmd.Object).Execute();

            var _threadCollection = IoC.Resolve<Dictionary<Guid, ServerThread>>("GetThreadCollection");

            var hardStopCommand1 = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.HardStop", _threadCollection[first], () => { cmd.Object.Execute(); });
            var hardStopCommand2 = IoC.Resolve<Hwdtech.ICommand>("Server.Commands.HardStop", _threadCollection[second], () =>
            {
                cmd.Object.Execute();
                mre.Set();
            });

            IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SendCommand", first, hardStopCommand1).Execute();

            IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SendCommand", second, new ActionCommand(() => { Thread.Sleep(1000); })).Execute();
            IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SendCommand", second, hardStopCommand2).Execute();

            mre.WaitOne();

            startcmd.Verify(cmd => cmd.Execute(), Times.Once());
            cmd.Verify(cmd => cmd.Execute(), Times.Exactly(4));
        }
        [Fact]

        public void EqualsTest()
        {
            var q = new BlockingCollection<Hwdtech.ICommand>(100);
            var t = new ServerThread(q);
            t.GetHashCode();

            t.Equals(null);

        }

    }
}
