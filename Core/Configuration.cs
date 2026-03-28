namespace Sts2ModAIAgentCommunicator.Core;

public static class Configuration
{
    public const int WaitTimeBetweenReadMessagesInMilliseconds = 300;
    public const int WaitTimeBetweenWriteMessagesInMilliseconds = 300;
    public const int ConnectionTimeoutInMilliseconds = 5000;
    public const int WaitBetweenReconnectInMilliseconds = 1000;
    public const int WaitBetweenSendRetriesInMilliseconds = 1000;
    public const int MaxSendRetryCount = 3;
}