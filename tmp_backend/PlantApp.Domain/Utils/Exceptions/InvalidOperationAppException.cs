using Microsoft.Extensions.Logging;

namespace PlantApp.Domain.Utils.Exceptions;

public class InvalidOperationAppException : AppException
{
    public InvalidOperationAppException(string userMessage, string? internalMessage = null, ILogger? logger = null)
            : base(userMessage, internalMessage ?? userMessage, logger)
    { }
}
