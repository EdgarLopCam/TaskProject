namespace TaskManagement.Tests.Services
{
    using Microsoft.Extensions.Options;
    using Moq;
    using TaskManagement.Application.Options;
    using TaskManagement.Application.Services;

    public class RetryPolicyTests
    {
        private readonly RetryPolicy _retryPolicy;
        private readonly Mock<IOptions<RetryPolicyOptions>> _optionsMock;

        public RetryPolicyTests()
        {
            _optionsMock = new Mock<IOptions<RetryPolicyOptions>>();
            _optionsMock.Setup(o => o.Value).Returns(new RetryPolicyOptions
            {
                MaxRetryCount = 3,
                DelayMilliseconds = 100
            });
            _retryPolicy = new RetryPolicy(_optionsMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Complete_On_First_Try()
        {
            // Arrange
            var actionMock = new Mock<Func<Task>>();
            actionMock.Setup(a => a()).Returns(Task.CompletedTask);

            // Act
            await _retryPolicy.ExecuteAsync(actionMock.Object);

            // Assert
            actionMock.Verify(a => a(), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Retry_On_Exception()
        {
            // Arrange
            var actionMock = new Mock<Func<Task>>();
            actionMock.SetupSequence(a => a())
                .ThrowsAsync(new Exception())
                .ThrowsAsync(new Exception())
                .Returns(Task.CompletedTask);

            // Act
            await _retryPolicy.ExecuteAsync(actionMock.Object);

            // Assert
            actionMock.Verify(a => a(), Times.Exactly(3));
        }

        [Fact]
        public async Task ExecuteAsync_Should_Throw_After_MaxRetries()
        {
            // Arrange
            var actionMock = new Mock<Func<Task>>();
            actionMock.Setup(a => a()).ThrowsAsync(new Exception());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _retryPolicy.ExecuteAsync(actionMock.Object));
            actionMock.Verify(a => a(), Times.Exactly(3));
        }
    }
}