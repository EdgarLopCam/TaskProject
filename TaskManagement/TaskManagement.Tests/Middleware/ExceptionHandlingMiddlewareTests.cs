namespace TaskManagement.Tests.Middleware
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System.Net;
    using System.Text.Json;
    using TaskManagement.API.Middleware;

    public class ExceptionHandlingMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _nextMock;
        private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock;
        private readonly ExceptionHandlingMiddleware _middleware;

        public ExceptionHandlingMiddlewareTests()
        {
            _nextMock = new Mock<RequestDelegate>();
            _loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            _middleware = new ExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Invoke_Should_Call_NextDelegate()
        {
            // Arrange
            var context = new DefaultHttpContext();

            // Act
            await _middleware.Invoke(context);

            // Assert
            _nextMock.Verify(x => x(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_Should_Handle_Exception_And_Log_Error()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            _nextMock.Setup(x => x(It.IsAny<HttpContext>())).Throws(new Exception("Test exception"));

            // Act
            await _middleware.Invoke(context);

            // Assert
            _loggerMock.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Test exception")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);

            Assert.Equal("application/json", context.Response.ContentType);
            Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = new StreamReader(context.Response.Body).ReadToEnd();
            var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
            Assert.Equal("An unexpected error occurred. Please try again later.", response.GetProperty("error").GetString());
        }
    }
}
