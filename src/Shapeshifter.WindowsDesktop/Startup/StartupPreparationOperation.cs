﻿using System;
using System.Threading.Tasks;

namespace Shapeshifter.WindowsDesktop.Startup
{
    using Interfaces;

    using Services.Arguments.Interfaces;
    using Services.Interfaces;

    class StartupPreparationOperation: IStartupPreparationOperation
    {
        readonly IProcessManager processManager;
        readonly IAggregateArgumentProcessor aggregateArgumentProcessor;

        bool hasRunBefore;

        public bool ShouldTerminate
            => aggregateArgumentProcessor.ShouldTerminate;

        public string[] Arguments { get; set; }

        public StartupPreparationOperation(
            IProcessManager processManager,
            IAggregateArgumentProcessor aggregateArgumentProcessor)
        {
            this.processManager = processManager;
            this.aggregateArgumentProcessor = aggregateArgumentProcessor;
        }

        public async Task RunAsync()
        {
            if (hasRunBefore)
            {
                throw new InvalidOperationException(
                    "Can't run the startup preparation twice.");
            }

            if (Arguments == null)
            {
                throw new ArgumentNullException(
                    nameof(Arguments));
            }

            hasRunBefore = true;

            processManager.CloseAllDuplicateProcessesExceptCurrent();
            aggregateArgumentProcessor.ProcessArguments(Arguments);
        }
    }
}
