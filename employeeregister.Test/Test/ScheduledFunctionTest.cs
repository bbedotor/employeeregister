using employeeregister.Functions.Functions;
using employeeregister.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace employeeregister.Test.Test
{
    public class ScheduledFunctionTest
    {
        [Fact]
        public void ScheduledFunction_Should_Log_Message()
        {
            // Arrange
            MockCloudTableRegisters mockTodos = new MockCloudTableRegisters(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);

            // Act
            ScheduledFunction.Run(null, mockTodos, logger);
            string message = logger.Logs[0];

            // Assert
            Assert.Contains("Deleting completed", message);
        }

    }
}

