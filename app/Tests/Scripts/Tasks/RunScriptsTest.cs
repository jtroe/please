﻿using System;
using Library.Scripts;
using Library.Scripts.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Scripts.Tasks
{
    [TestFixture]
    public class RunScriptsTest
    {
        [Test]
        public void should_run_sql_if_given_sql_scripts()
        {
            // Arrange
            var runScripts = Task.New<RunScripts>();
            runScripts.In.ConnectionName = Database.Name;
            runScripts.In.Scripts = new[] {new Script {FileName = @"whatever.sql"}};
            runScripts.RunSql = Fake.Task<RunSql>();
            runScripts.RunProcess = Fake.Task<RunProcess>();

            // Act
            runScripts.Execute();

            // Assert
            Assert.That(runScripts.RunSql.Stats.ExecuteCount, Is.EqualTo(1));
            Assert.That(runScripts.RunProcess.Stats.ExecuteCount, Is.EqualTo(0));
        }

        [Test]
        public void should_run_py_if_given_python_scripts()
        {
            // Arrange
            var runScripts = Task.New<RunScripts>();
            runScripts.In.ConnectionName = Database.Name;
            runScripts.In.Scripts = new[] {new Script {FileName = @"whatever.py"}};
            runScripts.RunSql = Fake.Task<RunSql>();
            runScripts.RunProcess = Fake.Task<RunProcess>();

            // Act
            runScripts.Execute();

            // Assert
            Assert.That(runScripts.RunSql.Stats.ExecuteCount, Is.EqualTo(0));
            Assert.That(runScripts.RunProcess.Stats.ExecuteCount, Is.EqualTo(1));
        }

        [Test]
        public void should_run_sql_and_py_scripts_if_given_both()
        {
            // Arrange
            var runScripts = Task.New<RunScripts>();
            runScripts.In.ConnectionName = Database.Name;
            runScripts.In.Scripts =
                new[] { new Script { FileName = @"whatever.sql" }, new Script { FileName = @"whatever.py" } };
            runScripts.RunSql = Fake.Task<RunSql>();
            runScripts.RunProcess = Fake.Task<RunProcess>();

            // Act
            runScripts.Execute();

            // Assert
            Assert.That(runScripts.RunSql.Stats.ExecuteCount, Is.EqualTo(1));
            Assert.That(runScripts.RunProcess.Stats.ExecuteCount, Is.EqualTo(1));
        }

        [Test]
        public void should_throw_if_given_unknown_script()
        {
            // Arrange
            var runScripts = Task.New<RunScripts>();
            runScripts.In.ConnectionName = Database.Name;
            runScripts.In.Scripts =
                new[] { new Script { FileName = @"whatever.rb" } };
            runScripts.RunSql = Fake.Task<RunSql>();
            runScripts.RunProcess = Fake.Task<RunProcess>();

            // Act & Assert
            Assert.Throws<RunException>(runScripts.Execute);
        }

        [Test]
        public void should_quit_if_sql_script_throws_exception()
        {
            // Arrange
            var runScripts = Task.New<RunScripts>();
            runScripts.In.ConnectionName = Database.Name;
            runScripts.In.Scripts =
                new[] { new Script { FileName = @"whatever.sql" }, new Script { FileName = @"whatever.py" } };
            runScripts.RunSql = Fake.Task<RunSql>(rs => { throw new RunException("test"); });
            runScripts.RunProcess = Fake.Task<RunProcess>();

            // Act & Assert
            Assert.Throws<RunException>(runScripts.Execute);
            Assert.That(runScripts.RunProcess.Stats.ExecuteCount, Is.EqualTo(0));
        }

        [Test]
        public void should_quit_if_py_script_returns_non_zero_exit_code()
        {
            // Arrange
            var runScripts = Task.New<RunScripts>();
            runScripts.In.ConnectionName = Database.Name;
            runScripts.In.Scripts =
                new[] { new Script { FileName = @"whatever.py" }, new Script { FileName = @"whatever.sql" } };
            runScripts.RunSql = Fake.Task<RunSql>();
            runScripts.RunProcess = Fake.Task<RunProcess>(rs => { throw new RunException("test"); });

            // Act & Assert
            Assert.Throws<RunException>(runScripts.Execute);
            Assert.That(runScripts.RunSql.Stats.ExecuteCount, Is.EqualTo(0));
        }
    }
}
