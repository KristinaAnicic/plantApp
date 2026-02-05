using Microsoft.Extensions.Logging;

namespace PlantApp.Domain.Utils.Exceptions;

public abstract class AppException : Exception
{
    public string UserMessage { get; }

    protected AppException(string userMessage, string? internalMessage = null, ILogger? logger = null)
        : base(internalMessage ?? userMessage)
    {
        UserMessage = userMessage;

        if (logger != null)
        {
            logger.LogWarning("{ExceptionType}: {InternalMessage}", GetType().Name, internalMessage ?? userMessage);
        }
    }
}

