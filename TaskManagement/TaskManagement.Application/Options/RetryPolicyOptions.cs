namespace TaskManagement.Application.Options
{
    public class RetryPolicyOptions
    {
        public int MaxRetryCount { get; set; }
        public int DelayMilliseconds { get; set; }
    }
}
