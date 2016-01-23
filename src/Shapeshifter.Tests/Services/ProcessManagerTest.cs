﻿namespace Shapeshifter.WindowsDesktop.Services
{
    using System.ComponentModel;
    using System.Diagnostics;

    using Autofac;

    using Interfaces;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ProcessManagerTest: TestBase
    {
        [Ignore]
        [TestMethod]
        [TestCategory("Integration")]
        public void CanLaunchProcessesAndKillThemAfterDisposing()
        {
            var container = CreateContainer();

            var initialRunningProcesses = GetTimeoutProcesses()
                .Length;
            using (var processManager = container.Resolve<IProcessManager>())
            {
                processManager.LaunchCommand("timeout", "/t -1 /nobreak");

                var runningProcessesBeforeDisposal = GetTimeoutProcesses()
                    .Length;
                Extensions.AssertWait(
                    () =>
                    Assert.AreEqual(initialRunningProcesses + 1, runningProcessesBeforeDisposal));
            }

            var runningProcessesAfterDisposal = GetTimeoutProcesses()
                .Length;
            Extensions.AssertWait(
                () =>
                Assert.AreEqual(initialRunningProcesses, runningProcessesAfterDisposal));
        }

        static Process[] GetTimeoutProcesses()
        {
            return Process.GetProcessesByName("timeout");
        }

        [TestMethod]
        [ExpectedException(typeof (Win32Exception))]
        public void ThrowsExceptionWhenLaunchingFileThatDoesNotExist()
        {
            var container = CreateContainer();

            var processManager = container.Resolve<IProcessManager>();
            processManager.LaunchFile("foobar.txt");
        }
    }
}