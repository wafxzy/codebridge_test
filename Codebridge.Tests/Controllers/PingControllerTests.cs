using Codebridge.API.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Codebridge.Tests.Controllers
{
    public class PingControllerTests
    {
        private readonly PingController _controller;

        public PingControllerTests()
        {
            _controller = new PingController();
        }

        [Fact]
        public void Ping_ShouldReturnOk_WithVersionString()
        {
            IActionResult result = _controller.Ping();

            OkObjectResult okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
            string value = okResult.Value.Should().BeOfType<string>().Subject;
            value.Should().Be("Dogshouseservice.Version1.0.1");
        }

        [Fact]
        public void Ping_ShouldAlwaysReturnSameMessage()
        {
            IActionResult result1 = _controller.Ping();
            IActionResult result2 = _controller.Ping();

            OkObjectResult okResult1 = result1.Should().BeOfType<OkObjectResult>().Subject;
            OkObjectResult okResult2 = result2.Should().BeOfType<OkObjectResult>().Subject;

            okResult1.Value.Should().Be(okResult2.Value);
        }

        [Fact]
        public void Ping_ShouldNotReturnNull()
        {
            IActionResult result = _controller.Ping();

            result.Should().NotBeNull();
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
        }
    }
}
