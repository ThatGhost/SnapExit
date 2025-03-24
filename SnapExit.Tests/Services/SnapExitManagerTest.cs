﻿using SnapExit.Interfaces;
using SnapExit.Services;
using SnapExit.Tests.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapExit.Tests.Services
{
    class SnapExitManagerTest : SnapExitManager<SnapExitReponse, object>
    {
        private readonly IExecutionControlService _executionControlService;

        // use to Assert in the unit test
        public SnapExitReponse? response;
        public object? enviroument;

        public SnapExitManagerTest(IExecutionControlService executionControlService) : base(executionControlService)
        {
            _executionControlService = executionControlService;
        }

        public void SetupSnapExit(SnapExitReponse response)
        {
            _executionControlService.EnviroumentData = new { };
            RegisterSnapExit(SomeLongTask(response));
        }

        private Task SomeLongTask(SnapExitReponse response)
        {
            _executionControlService.StopExecution(response);
            return Task.CompletedTask;
        }

        protected override Task SnapExitResponse(SnapExitReponse? responseData, object? enviroumentData)
        {
            response = responseData;
            enviroument = enviroumentData;
            return Task.CompletedTask;
        }
    }
}
