using employeeregister.Common.Models;
using employeeregister.Functions.Functions;
using employeeregister.Test.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace employeeregister.Test.Test
{
    public class RegisterApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void CreateRegister_Should_Return_200()
        {
            //arrenge

            MockCloudTableRegisters mocTodos = new MockCloudTableRegisters(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Register registerRequest = TestFactory.getTodoRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(registerRequest);

            //act

            IActionResult response = await TodoAPI.CreateEentry(request, mocTodos, logger);

            // assert

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void UpdateTodo_Should_Return_200()
        {
            //Arrenge

            MockCloudTableRegisters mocTodos = new MockCloudTableRegisters(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Register todoRequest = TestFactory.getTodoRequest();
            Guid todoId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoId, todoRequest);

            //act

            IActionResult response = await TodoAPI.UpdateRegister(request, mocTodos, todoId.ToString(), logger);


            // assert

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

        }

    }
}
