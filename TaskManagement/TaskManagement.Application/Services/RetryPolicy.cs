namespace TaskManagement.Application.Services
{
    using TaskManagement.Application.Options;
    using Microsoft.Extensions.Options;

    public interface IRetryPolicy
    {
        Task ExecuteAsync(Func<Task> action);
    }

    public class RetryPolicy : IRetryPolicy
    {
        private readonly int _maxRetryCount;
        private readonly int _delayMilliseconds;

        public RetryPolicy(IOptions<RetryPolicyOptions> options)
        {
            _maxRetryCount = options.Value.MaxRetryCount;
            _delayMilliseconds = options.Value.DelayMilliseconds;
        }

        public async Task ExecuteAsync(Func<Task> action)
        {
            int retryCount = 0;
            while (true)
            {
                try
                {
                    await action();
                    break;
                }
                catch (Exception)
                {
                    retryCount++;
                    if (retryCount >= _maxRetryCount) throw;
                    await Task.Delay(_delayMilliseconds);
                }
            }
        }
    }
}